using System.Collections.Generic;
using System.Dynamic;
using Akka.Actor;

namespace DistBelief.Actors
{
    public class OutputActor : ReceiveActor
    {
        public Dictionary<int, List<double>> LatestOutputs { get; set; }

        public OutputActor()
        {
            LatestOutputs = new Dictionary<int, List<double>>();
            Receive<Output>(output =>
            {
                LatestOutputs.Add(output.ReplicaId, output.OutputValues);
            });
        }
    }

    public class Output
    {
        public int ReplicaId { get; set; }
        public List<double> Target { get; set; }
        public List<double> OutputValues { get; set; }
    }
}