using System.Collections.Generic;
using Akka.Actor;
using DistBelief.Messages;

namespace DistBelief.Actors
{
    public class Layer : ReceiveActor
    {
        public IActorRef ParameterShardId { get; set; }
        public double[,] LatestWeights { get; set; }
        public int LayerId { get; set; }
        public List<double> ActivatedInput { get; set; }
        public List<double> Activations { get; set; }
        public IActorRef ChildLayer { get; set; }
        public IActorRef ParentLayer { get; set; }
        public int ReplicaId { get; set; }
        public IActorRef OutputActor { get; set; }

        public Layer()
        {
            Receive<FetchParameters>(parameters =>
            {
                ParameterShardId.Tell(new ParameterRequest());
                Become(WaitForParameters);
            });
            Receive<MyChild>(child =>
            {
                ChildLayer = child.Child;
            });
            Receive<ForwardPass>(pass =>
            {
                if (ChildLayer != null)
                {
                    ChildLayer.Tell(pass);
                }
                else
                {
                    //deltas, gradient and initialize backward pass
                }
            });
            Receive<BackwardPass>(pass =>
            {

            });

        }

        private void WaitForParameters()
        {
            Receive<LatestParameters>(parameters =>
            {
                LatestWeights = parameters.Weights;
                Context.Parent.Tell(new DoneFetchingParameters
                {
                    LayerId = LayerId
                });
                Context.UnbecomeStacked();
            });
        }

    }

    public class BackwardPass
    {
    }
}
