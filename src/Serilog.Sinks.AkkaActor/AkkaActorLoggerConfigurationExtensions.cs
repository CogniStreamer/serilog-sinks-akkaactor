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
using Akka.Actor;
using Serilog.Configuration;
using Serilog.Events;
using Serilog.Sinks.AkkaActor;

namespace Serilog
{
    /// <summary>
    /// Adds the WriteTo.Akka() extension method to <see cref="LoggerConfiguration"/>.
    /// </summary>
    public static class AkkaActorLoggerConfigurationExtensions
    {
        /// <summary>
        /// Adds a sink that writes log events as string messages to a Akka actor.
        /// </summary>
        /// <param name="loggerConfiguration">The logger configuration.</param>
        /// <param name="actorSystem">The Akka actor system.</param>
        /// <param name="actorPath">The Akka actor path.</param>
        /// <param name="restrictedToMinimumLevel">The minimum log event level required in order to write an event to the sink.</param>
        /// <param name="batchPostingLimit">The maximum number of events to post in a single batch.</param>
        /// <param name="period">The time to wait between checking for event batches.</param>
        /// <param name="formatProvider">Supplies culture-specific formatting information, or null.</param>
        /// <returns>Logger configuration, allowing configuration to continue.</returns>
        /// <exception cref="ArgumentNullException">A required parameter is null.</exception>
        public static LoggerConfiguration AkkaActor(
            this LoggerSinkConfiguration loggerConfiguration,
            ActorSystem actorSystem,
            string actorPath,
            LogEventLevel restrictedToMinimumLevel = LevelAlias.Minimum,
            int batchPostingLimit = AkkaActorSink.DefaultBatchPostingLimit,
            TimeSpan? period = null,
            IFormatProvider formatProvider = null)
        {
            if (actorSystem == null) throw new ArgumentNullException(nameof(actorSystem));
            if (string.IsNullOrEmpty(actorPath)) throw new ArgumentNullException(nameof(actorPath));

            var defaultedPeriod = period ?? AkkaActorSink.DefaultPeriod;
            return loggerConfiguration.Sink(
                new AkkaActorSink(actorSystem, actorPath, batchPostingLimit, defaultedPeriod, formatProvider),
                restrictedToMinimumLevel);
        }
    }
}
