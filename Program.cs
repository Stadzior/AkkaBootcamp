﻿using Akka.Actor;
using WinTail.Actors;

namespace WinTail
{
    #region Program

    internal class Program
    {
        public static ActorSystem MyActorSystem;

        private static void Main()
        {
            // initialize MyActorSystem
            MyActorSystem = ActorSystem.Create("MyActorSystem");
            
            // time to make your first actors!
            var consoleWriterActor = MyActorSystem.ActorOf(Props.Create(() => new ConsoleWriterActor()), "consoleWriterActor");
            var validationActor = MyActorSystem.ActorOf(Props.Create(() => new ValidationActor(consoleWriterActor)), "validationActor");
            var consoleReaderActor = MyActorSystem.ActorOf(Props.Create(() =>  new ConsoleReaderActor(validationActor)), "consoleReaderActor");

            // tell console reader to begin
            consoleReaderActor.Tell(ConsoleReaderActor.StartCommand);

            // blocks the main thread from exiting until the actor system is shut down
            MyActorSystem.WhenTerminated.Wait();
        }
    }

    #endregion
}
