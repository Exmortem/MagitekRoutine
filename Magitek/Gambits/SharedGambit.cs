using PropertyChanged;

namespace Magitek.Gambits
{
    [AddINotifyPropertyChangedInterface]
    public class SharedGambit
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Job { get; set; }
        public bool IsGambitGroup { get; set; }
        public string File { get; set; }
        public string Created { get; set; }
        public string PosterId { get; set; }
        public int NumberOfGambits { get; set; }
        public string Zone { get; set; }
    }
}
