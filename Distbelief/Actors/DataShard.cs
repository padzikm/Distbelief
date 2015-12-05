using System.Collections.Generic;
using System.Linq;
using Akka.Actor;
using DistBelief.Messages;

namespace DistBelief.Actors
{
    public class DataShard : ReceiveActor
    {
        public List<Model> TrainingData { get; set; }
        public List<double> Activation { get; set; }
        public List<double> ActivationDerivative { get; set; }
        public List<IActorRef> ParameterShards { get; set; }
        public int ShardId { get; set; }
        public int LayersCount { get; set; }
        public HashSet<int> LayersNotUpdated { get; set; }
        public DataShard()
        {
            LayersNotUpdated = new HashSet<int>();
            LayersNotUpdated.UnionWith(Enumerable.Range(0, LayersCount - 1).ToList());
            Receive<ReadyToProcess>(process =>
            {
                Become(WaitForAllLayerUpdates);
            });
            // Create Layer Actors

        }

        private void WaitForAllLayerUpdates()
        {
            Receive<DoneFetchingParameters>(parameters =>
            {
                LayersNotUpdated.Remove(parameters.LayerId);
                if (LayersNotUpdated.Count == 0)
                {
                    Context.Parent.Tell(new Done { ShardId = ShardId});
                    Context.Stop(Self);
                }
                LayersNotUpdated = new HashSet<int>();
                LayersNotUpdated.UnionWith(Enumerable.Range(0, LayersCount - 1).ToList());
                UnbecomeStacked();
            });
        }
    }
}
