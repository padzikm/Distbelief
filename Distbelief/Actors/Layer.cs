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
                //        activatedInput = parentLayer match {
                //case Some(p) => DenseVector.vertcat(DenseVector(1.0), activationFunction(inputs))
                //          case _ => inputs
                //        }
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
}
