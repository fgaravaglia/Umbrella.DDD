using System.Diagnostics.CodeAnalysis;

namespace Umbrella.DDD.Configuration
{
    [ExcludeFromCodeCoverage]
    public static class PublisherNames
    {
        public const string IN_MEMEORY = "InMemory";

        public const string GOOGLE_PUB_SUB = "PubSub";
    }
}