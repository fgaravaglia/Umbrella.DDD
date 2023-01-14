using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Umbrella.DDD.Abstractions;

[assembly: InternalsVisibleTo("Umbrella.DDD.Tests")]

namespace Umbrella.DDD
{
    /// <summary>
    /// Simplest implementation of publisher, where configuration is stored in memory
    /// </summary>
    public class MessageBus : IMessageBus
    {
        #region Fields
        readonly ILogger _Logger;
        readonly IEventPublisher _Publisher;
        readonly IServiceProvider _ServiceProvider;
        #endregion

        public MessageBus(ILogger logger, IEventPublisher publisher, IServiceProvider serviceProvider)
        {
            this._Logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this._Publisher = publisher ?? throw new ArgumentNullException(nameof(publisher));
            this._ServiceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        }

        /// <summary>
        /// <inheritdoc cref="IEventPublisher.PublishMessage"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="msg"></param>
        /// <returns></returns>
        public string PublishMessage(IMessage msg)
        {
            if(msg == null)
                throw new ArgumentNullException(nameof(msg));

            // publis current message
            this._Logger.LogInformation("Publishing Message {eventMessage}", msg);
            var msgId = this._Publisher.PublishMessage(msg);

            var targetMessageType = msg.GetType();
            Type handlerType = typeof(IMessageHandler<>).MakeGenericType(targetMessageType);
            this._Logger.LogInformation("Resolving handlers of type {handlerType}", handlerType);

            // check if there are any hanldler of this message
            var handlers = this._ServiceProvider.GetServices(handlerType)
                                                .Select(x => (IMessageHandler)x)
                                                .Where(x => x != null && x.CanHandleThisMessage(msg))
                                                .ToList();
            this._Logger.LogInformation("Found {handlersCount} to handle the message {eventId} of {type}", handlers.Count, msg.ID, targetMessageType);
            Parallel.ForEach(handlers, x =>
            {
                if(x != null)
                {
                    var index = handlers.IndexOf(x);
                    RunHandler(x, msg, index);
                }
            });

            // Check if saga need to be run
            var sagas = this._ServiceProvider.GetServices<ISaga>().Where(x => x.CanHandleThisMessage(msg)).ToList();
            this._Logger.LogInformation("Found {sagaCounter} sagas to verify", sagas.Count);
            Parallel.ForEach(sagas, x =>
            {
                var index = sagas.IndexOf(x);
                RunSaga(x, msg, index);
            });
            return msgId;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="queueName"></param>
        public void UsingThisQueueFor<T>(string queueName) where T : IMessage
        {
            this._Publisher.UsingThisQueueFor<T>(queueName);
        }

        private void RunHandler(IMessageHandler handler, IMessage msg, int zeroBasedIndex)
        {
            if (handler.CanHandleThisMessage(msg))
            {
                var occurredEx = handler.TryHandleMessage(msg);
                if (occurredEx is null)
                    this._Logger.LogError("Handler #{handlerIndex}: consumed message of {type} wiht no errors", zeroBasedIndex, msg.GetType());
                else
                    this._Logger.LogError(occurredEx, "Handler #{handlerIndex}: consumed message of {type} with errors", zeroBasedIndex, msg.GetType());
            }
        }

        private void RunSaga(ISaga saga, IMessage msg, int zeroBasedIndex)
        {
            var msgType = msg.GetType();
            try
            {
                // restore status
                saga.RefreshStatus();
                if (saga.Status == null)
                    throw new NullReferenceException("Unexpcted Saga status null after refresh");
                if (saga.Status.IsRunning)
                {
                    // continue the saga
                    IMessageHandler? handler = saga as IMessageHandler;
                    if(handler != null)
                    {
                        var ex = handler.TryHandleMessage(msg);
                        if(ex != null)
                            this._Logger.LogError(ex, "Saga {sagaId} Failed!", saga.Id);
                    }
                    else
                        this._Logger.LogWarning("Saga {sagaId} [{sagaType}] cannot handle messages of type {messageType}",
                                                saga.Id, saga.GetType(), msgType);
                }
                else if (!saga.Status.IsCompleted)
                {
                    // start the saga if reguired
                    if (saga.StarterEventType == msgType)
                    {
                        saga.Start(msg);
                    }
                    else
                        this._Logger.LogWarning("Saga #{sagaIndex}: it is not completed but I cannot start it with {messageType}", zeroBasedIndex, msgType);
                }
                saga.PersistStatus();
            }
            catch (Exception ex)
            {
                this._Logger.LogError(ex, "Saga #{sagaIndex}: consumed message of {type} with errors", zeroBasedIndex, msgType);
            }
        }
    }
}