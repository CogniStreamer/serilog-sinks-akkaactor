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
    /// Writes log events to an Akka actor.
    /// </summary>
    public class AkkaActorSink<TMessage> : PeriodicBatchingSink
    {
        private readonly ActorSelection _actor;
        private readonly Func<LogEvent, TMessage> _convertLogEvent;

        /// <summary>
        /// Construct a sink posting to the specified Akka actor.
        /// </summary>
        /// <param name="actorSystem">The actor system.</param>
        /// <param name="actorPath">The actor path.</param>
        /// <param name="batchPostingLimit">The maximum number of events to post in a single batch.</param>
        /// <param name="period">The time to wait between checking for event batches.</param>
        /// <param name="convertLogEvent">User supplied conversion method to convert from <see cref="LogEvent"/> to TMessage.</param>
        public AkkaActorSink(ActorSystem actorSystem, string actorPath, int batchPostingLimit, TimeSpan period, Func<LogEvent, TMessage> convertLogEvent = null)
            : base(batchPostingLimit, period)
        {
            if (actorSystem == null) throw new ArgumentNullException(nameof(actorSystem));
            if (string.IsNullOrEmpty(actorPath)) throw new ArgumentNullException(nameof(actorPath));
            _actor = actorSystem.ActorSelection(actorPath);
            _convertLogEvent = convertLogEvent;
        }

        /// <summary>
        /// Emit a batch of log events, running asynchronously.
        /// </summary>
        /// <param name="events">The events to emit.</param>
        protected override void EmitBatch(IEnumerable<LogEvent> events)
        {
            foreach (var logEvent in events)
            {
                _actor.Tell(_convertLogEvent != null ? (object)_convertLogEvent(logEvent) : logEvent);
            }
        }
    }
}
