using System;
using System.Collections.Generic;
using System.Text;
using Akka.Actor;
using WinTail.Messages;

namespace WinTail.Actors
{
    public class TailCoordinatorActor : UntypedActor
    {
        protected override void OnReceive(object message)
        {
            if (message is StartTail msg)
                Context.ActorOf(Props.Create(() => new TailActor(msg.ReporterActor, msg.FilePath)));
        }

        protected override SupervisorStrategy SupervisorStrategy()
            => new OneForOneStrategy(10, TimeSpan.FromSeconds(30),
                exception =>
                    exception switch
                    {
                        //Maybe we consider ArithmeticException to not be application critical
                        //so we just ignore the error and keep going.
                        //Error that we cannot recover from, stop the failing actor
                        ArithmeticException _ => Directive.Resume,
                        //In all other cases, just restart the failing actor
                        NotSupportedException _ => Directive.Stop,
                        _ => Directive.Restart
                    });
    }
}
