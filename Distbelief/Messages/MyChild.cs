using Akka.Actor;

namespace DistBelief.Messages
{
    public class MyChild
    {
        public IActorRef Child { get; set; }
    }
}