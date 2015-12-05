using System;
using System.Collections.Generic;
using System.Linq;
using Akka.Actor;
using DistBelief.Messages;

namespace DistBelief.Actors
{
    public class Master : ReceiveActor
    {
        public int DataPerReplica { get; set; }
        public List<int> LayerDimentions { get; set; }
        public List<double> Activation { get; set; }
        public List<double> ActivationDerivative { get; set; }
        public double LearningRate { get; set; }
        public List<Model> DataSet { get; set; }
        public Master()
        {
            var layersCount = LayerDimentions.Count;
            var dataShards = Enumerable.Repeat(DataSet, DataPerReplica).ToList();
            var parameterShardActors = new IActorRef[layersCount - 1];
            for (int i = 0; i < layersCount -1; i++)
            {
                parameterShardActors[i] = Context.ActorOf(new Props(typeof(ParameterShard), new object[]{ new ParameterShard(i, LearningRate, new double[3,3])}));
            }
            var dataShardActors = new List<IActorRef>();
            foreach (var dataShard in dataShards)
            {
                dataShardActors.Add(Context.ActorOf(new Props(typeof(DataShard), new []{new DataShard
                {
                    
                } })));
            }
            var shardsFinishedCount = 0;
            Receive<Start>(start =>
            {
                foreach (var dataShardActor in dataShardActors)
                {
                    dataShardActor.Tell(new ReadyToProcess());
                }
            });
            Receive<Done>(done =>
            {
                shardsFinishedCount++;
                if (shardsFinishedCount == dataShards.Count)
                    Console.WriteLine("Done");
            });

        }
    }
}
