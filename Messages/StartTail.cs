using System;
using System.Collections.Generic;
using System.Text;
using Akka.Actor;

namespace WinTail.Messages
{
    /// <summary>
    /// Start tailing the file at user-specified path.
    /// </summary>
    public class StartTail
    {
        public StartTail(string filePath, IActorRef reporterActor)
        {
            FilePath = filePath;
            ReporterActor = reporterActor;
        }

        public string FilePath { get; }

        public IActorRef ReporterActor { get; }
    }
}
