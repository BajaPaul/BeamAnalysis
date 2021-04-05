using System.Collections.Generic;
using BeamAnalysis.Abstract;

namespace BeamAnalysis.NumericalModel
{
    internal class Topology: IComponentBuilder<Topology>, ITopology
    {
        private readonly Model TopologyModel;
        public List<int[]> Matrix { get; private set; }

        public Topology(Model model)
        {
            TopologyModel = model;
            Matrix = new List<int[]>();
        }

        public void SetComponent()
        {
            foreach(IElement currentElement in TopologyModel.AbstractIScheme.ListIElement)
            {
                Matrix.Add(new int[] { currentElement.IntNodeLeft, currentElement.IntNodeRight });
            }
        }

        public void SetDedicatedData() { }

        public Topology Get()
        {
            return this;
        }
    }
}
