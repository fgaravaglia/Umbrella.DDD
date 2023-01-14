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
        /// <inheritdoc cref="IEventPublisher.PublishEvent{T}(T)"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="msg"></param>
        /// <returns></returns>
        public string PublishEvent<T>(T msg) where T : IMessage
        {
            // publis current message
            this._Logger.LogInformation("Publishing Message {eventMessage}", msg);
            var msgId = this._Publisher.PublishEvent(msg);

            // check if there are any hanldler of this message
            var handlers = this._ServiceProvider.GetServices<IMessageHandler<IMessage>>()
                                                .Where(x => x.CanHandleThisMessage(msg))
                                                .ToList();
            this._Logger.LogInformation("Found {handlersCount} to handle the message {eventId} of {type}", handlers.Count, msg.ID, typeof(T));
            Parallel.ForEach(handlers, x =>
            {
                var index = handlers.IndexOf(x);
                RunHandler(x, msg, index);
            });

            // Check if saga need to be run
            var sagas = this._ServiceProvider.GetServices<ISaga>().Where(x => x.CanHandleThis<T>()).ToList();
            this._Logger.LogInformation("Found {sagaCounter} sagas to verify", sagas.Count);
            Parallel.ForEach(sagas, x =>
            {
                var index = sagas.IndexOf(x);
                RunSaga<T>(x, msg, index);
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

        private void RunHandler<T>(IMessageHandler<T> handler, T msg, int zeroBasedIndex) where T : IMessage
        {
            if (handler.CanHandleThisMessage(msg))
            {
                var occurredEx = handler.TryHandleThisMessage(msg);
                if (occurredEx is null)
                    this._Logger.LogError("Handler #{handlerIndex}: consumed message of {type} wiht no errors", zeroBasedIndex, typeof(T));
                else
                    this._Logger.LogError(occurredEx, "Handler #{handlerIndex}: consumed message of {type} with errors", zeroBasedIndex, typeof(T));
            }
        }

        private void RunSaga<T>(ISaga saga, T msg, int zeroBasedIndex) where T : IMessage
        {
            try
            {
                // restore status
                saga.RefreshStatus();
                if (saga.Status == null)
                    throw new NullReferenceException("Unexpcted Saga status null after refresh");
                if (saga.Status.IsRunning)
                {
                    // continue the saga
                    IMessageHandler<T>? handler = saga as IMessageHandler<T>;
                    if(handler != null)
                    {
                        var ex = handler.TryHandleThisMessage(msg);
                        if(ex != null)
                            this._Logger.LogError(ex, "Saga {sagaId} Failed!", saga.Id);
                    }
                    else
                        this._Logger.LogWarning("Saga {sagaId} [{sagaType}] cannot handle messages of type {messageType}",
                                                saga.Id, saga.GetType(), typeof(T));
                }
                else if (!saga.Status.IsCompleted)
                {
                    // start the saga if reguired
                    if (saga.StarterEventType == typeof(T))
                    {
                        saga.Start(msg);
                    }
                    else
                        this._Logger.LogWarning("Saga #{sagaIndex}: it is not completed but I cannot start it with {messageType}", zeroBasedIndex, typeof(T));
                }
                saga.PersistStatus();
            }
            catch (Exception ex)
            {
                this._Logger.LogError(ex, "Saga #{sagaIndex}: consumed message of {type} with errors", zeroBasedIndex, typeof(T));
            }
        }
    }
}