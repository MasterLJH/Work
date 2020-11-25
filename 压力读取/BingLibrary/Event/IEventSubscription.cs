using System;

namespace BingLibrary.hjb.events
{
    public interface IEventSubscription
    {
        SubscriptionToken SubscriptionToken { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
        Action<object[]> GetExecutionStrategy();
    }
}