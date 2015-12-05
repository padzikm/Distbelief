using System.Collections.Generic;

namespace DistBelief.Messages
{
    public class ForwardPass
    {
        public List<double> Inputs { get; set; }
        public List<double> Targets { get; set; }
    }
}