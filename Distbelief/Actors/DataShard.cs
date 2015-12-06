using System.Collections.Generic;
using System.Linq;
using Akka.Actor;
using Akka.Actor.Dsl;
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
        public IActorRef[] Layers { get; set; }
        public DataShard()
        {
            CreateLayerActors();
            Receive<ReadyToProcess>(process =>
            {
                Become(WaitForAllLayerUpdates);
            });
        }

        private void CreateLayerActors()
        {
            var layersCount = ParameterShards.Count;
            var outputActor = Context.ActorOf(new Props(typeof (OutputActor), new[] {new OutputActor()}));
            Layers = new IActorRef[layersCount];
            for (int i = 0; i < layersCount; i++)
            {
                Layers[i] = Context.ActorOf(new Props(typeof (Layer), new[]
                {
                    new Layer
                    {
                        ReplicaId = ShardId,
                        LayerId = i,
                        Activations = Activation,
                        ParentLayer = i > 0 ? Layers[i - 1] : null,
                        ParameterShardId = ParameterShards[i],
                        OutputActor = i == layersCount - 1 ? outputActor : null
                    }
                }));
                if (i > 0)
                    Layers[i - 1].Tell(new MyChild {Child = Layers[i]});
            }

            LayersNotUpdated = new HashSet<int>();
            LayersNotUpdated.UnionWith(Enumerable.Range(0, LayersCount - 1).ToList());
        }

        private void WaitForAllLayerUpdates()
        {
            Receive<DoneFetchingParameters>(parameters =>
            {
                LayersNotUpdated.Remove(parameters.LayerId);
                if (LayersNotUpdated.Count == 0)
                {
                    if (TrainingData.Count != 0)
                    {
                        TrainingData.Remove(TrainingData.First());
                        var datapoint = TrainingData.First();
                        Layers.First().Tell(new ForwardPass());
                    }
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
