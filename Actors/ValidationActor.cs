using System;
using Akka.Actor;
using WinTail.Messages;

namespace WinTail.Actors
{ 
    /// <summary>
    /// Actor that validates user input and signals result to others.
    /// </summary>
    public class ValidationActor : UntypedActor
    {
        private readonly IActorRef _consoleWriterActor;

        public ValidationActor(IActorRef consoleWriterActor)
        {
            _consoleWriterActor = consoleWriterActor;
        }

        /// <summary>
        /// Reads input from console, validates it, then signals appropriate response
        /// (continue processing, error, success, etc.).
        /// </summary>
        protected override void OnReceive(object message)
        {
            var msg = message as string;
            if (string.IsNullOrEmpty(msg))
            {
                // signal that the user needs to supply an input, as previously
                _consoleWriterActor.Tell(new NullInputError("No input received."));
            }
            else
            {
                if (IsValid(msg))
                    _consoleWriterActor.Tell(new InputSuccess("Thank you! Message was valid."));
                else
                    _consoleWriterActor.Tell(new ValidationError("Invalid: input had odd number of characters."));
            }

            // continue reading messages from console
            Sender.Tell(new ContinueProcessing());
        }

        /// <summary>
        /// Validates <see cref="message"/>.
        /// Currently says messages are valid if contain even number of characters.
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        private static bool IsValid(string message)
            => message.Length % 2 == 0;
    }
}
