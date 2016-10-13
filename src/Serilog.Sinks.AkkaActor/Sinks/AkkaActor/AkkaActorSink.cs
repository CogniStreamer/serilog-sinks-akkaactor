// Copyright 2016 Serilog Contributors
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using System.Collections.Generic;
using Akka.Actor;
using Serilog.Events;
using Serilog.Sinks.PeriodicBatching;

namespace Serilog.Sinks.AkkaActor
{
    /// <summary>
    /// Writes log events as messages to an Akka actor.
    /// </summary>
    public class AkkaActorSink : PeriodicBatchingSink
    {
        /// <summary>
        /// A reasonable default for the number of events posted in each batch.
        /// </summary>
        public const int DefaultBatchPostingLimit = 5;

        /// <summary>
        /// A reasonable default time to wait between checking for event batches.
        /// </summary>
        public static readonly TimeSpan DefaultPeriod = TimeSpan.FromSeconds(2);

        private readonly ActorSelection _actor;
        private readonly IFormatProvider _formatProvider;

        /// <summary>
        /// Construct a sink posting to the specified Akka actor.
        /// </summary>
        /// <param name="actorSystem">The actor system.</param>
        /// <param name="actorPath">The actor path.</param>
        /// <param name="batchPostingLimit">The maximum number of events to post in a single batch.</param>
        /// <param name="period">The time to wait between checking for event batches.</param>
        /// <param name="formatProvider">Supplies culture-specific formatting information, or null.</param>
        public AkkaActorSink(ActorSystem actorSystem, string actorPath, int batchPostingLimit, TimeSpan period, IFormatProvider formatProvider)
            : base(batchPostingLimit, period)
        {
            if (actorSystem == null) throw new ArgumentNullException(nameof(actorSystem));
            if (string.IsNullOrEmpty(actorPath)) throw new ArgumentNullException(nameof(actorPath));
            _actor = actorSystem.ActorSelection(actorPath);
            _formatProvider = formatProvider;
        }

        /// <summary>
        /// Emit a batch of log events, running asynchronously.
        /// </summary>
        /// <param name="events">The events to emit.</param>
        protected override void EmitBatch(IEnumerable<LogEvent> events)
        {
            foreach (var logEvent in events)
            {
                _actor.Tell(logEvent.RenderMessage(_formatProvider));
            }
        }
    }
}
