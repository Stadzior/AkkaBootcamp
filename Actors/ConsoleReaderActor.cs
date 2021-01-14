using System;
using Akka.Actor;
using WinTail.Messages;

namespace WinTail.Actors
{
    /// <summary>
    /// Actor responsible for reading FROM the console. 
    /// Also responsible for calling <see cref="ActorSystem.Terminate"/>.
    /// </summary>
    internal class ConsoleReaderActor : UntypedActor
    {
        public const string StartCommand = "start";
        public const string ExitCommand = "exit";

        private readonly IActorRef _fileValidatorActor;

        public ConsoleReaderActor(IActorRef fileValidatorActor)
        {
            _fileValidatorActor = fileValidatorActor;
        }

        protected override void OnReceive(object message)
        {
            if (message.Equals(StartCommand))
                Console.WriteLine("Please provide the URI of a log file on disk.\n");

            ValidateInput();
        }

        #region Internal methods

        /// <summary>
        /// Reads input from console, validates it, then signals appropriate response
        /// (continue processing, error, success, etc.).
        /// </summary>
        private void ValidateInput()
        {
            var read = Console.ReadLine();
            if (!string.IsNullOrEmpty(read) && string.Equals(read, ExitCommand, StringComparison.OrdinalIgnoreCase))
            {
                // if user typed ExitCommand, shut down the entire actor
                // system (allows the process to exit)
                Context.System.Terminate();
                return;
            }

            // otherwise, just hand message off to validation actor
            // (by telling its actor ref)
            _fileValidatorActor.Tell(read);
        }

        #endregion
    }
}