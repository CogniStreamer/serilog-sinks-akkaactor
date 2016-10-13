using System;

namespace Serilog.Sinks.AkkaActor
{
    /// <summary>
    /// Defaults values used in <see cref="AkkaActorSink{TMessage}"/> configuration.
    /// </summary>
    public static class AkkaActorSinkDefaults
    {
        /// <summary>
        /// A reasonable default for the number of events posted in each batch.
        /// </summary>
        public const int DefaultBatchPostingLimit = 5;

        /// <summary>
        /// A reasonable default time to wait between checking for event batches.
        /// </summary>
        public static readonly TimeSpan DefaultPeriod = TimeSpan.FromSeconds(2);
    }
}
