using Akka.Actor;
using DistBelief.Messages;

namespace DistBelief.Actors
{
    public class ParameterShard : ReceiveActor
    {
        private readonly int shardId;
        private readonly double learningRate;
        private readonly double[,] initialWeight;

        private readonly double[,] latestParameter;

        public ParameterShard(int shardId, double learningRate, double[,] initialWeight)
        {
            this.shardId = shardId;
            this.learningRate = learningRate;
            this.initialWeight = initialWeight;
            latestParameter = initialWeight;
            ReceiveParameterRequest();
            ReceiveGradient();
        }

        public ParameterShard()
        {
            
        }

        private void ReceiveGradient()
        {
            Receive<Gradient>(gradient =>
            {
                //update parameter
                //latestParameter = latestParameter + (g.t * learningRate)
            });
        }

        private void ReceiveParameterRequest()
        {
            Receive<ParameterRequest>(request =>
            {
                Context.Sender.Tell(new LatestParameters
                {
                    Weights = latestParameter
                });
            });
        }

    }
}
