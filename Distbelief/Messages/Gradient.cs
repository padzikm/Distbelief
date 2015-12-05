namespace DistBelief.Messages
{
    public class Gradient
    {
        public double[,] Value { get; set; }
        public int ReplicaId { get; set; }
        public int LayerId { get; set; }
    }
}