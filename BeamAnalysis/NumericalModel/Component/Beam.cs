using System;
using System.Linq;
using System.Collections.Generic;
using BeamAnalysis.Abstract;

namespace BeamAnalysis.NumericalModel
{
    internal class Beam : IComponentBuilder<Beam>, IBeam
    {
        private readonly Model BeamModel;

        public List<double[,]> Matrix { get; private set; }
        public List<IBeamProperty> Properties { get; set; }

        public Beam(Model model)
        {
            BeamModel = model;

            Matrix = new List<double[,]>();
            Properties = new List<IBeamProperty>();
        }

        public void SetComponent()
        {
            for (int i = 0; i < BeamModel.IntModelElements; i++)
            {
                IElement currentElement;
                currentElement = BeamModel.AbstractIScheme.ListIElement[i];
                
                double E = GetCurrentE(currentElement.IntMaterial);
                double Iy = GetCurrentIy(currentElement.IntCrossSection);
                double L = GetCurrentL(currentElement.IntNodeLeft, currentElement.IntNodeRight);

                IBeamProperty beamProperty = new BeamProperty(currentElement.IntNumber, E, Iy, L);
                Properties.Add(beamProperty);

                double[,] elementStiffnessMatrix = CalculateElementStiffnessMatrix(E, Iy, L);
                Matrix.Add(elementStiffnessMatrix);
            }
        }

        public void SetDedicatedData() { }

        public Beam Get()
        {
            return this;
        }

        private double[,] CalculateElementStiffnessMatrix(double E, double Iy, double L)
        {
            double[,] currentMatrix = new double[4, 4];

            currentMatrix[0, 0] = 12 * E * Iy / Math.Pow(L, 3);
            currentMatrix[0, 1] = 6 * E * Iy / Math.Pow(L, 2);
            currentMatrix[0, 2] = -12 * E * Iy / Math.Pow(L, 3);
            currentMatrix[0, 3] = 6 * E * Iy / Math.Pow(L, 2);

            currentMatrix[1, 0] = 6 * E * Iy / Math.Pow(L, 2);
            currentMatrix[1, 1] = 4 * E * Iy / Math.Pow(L, 1);
            currentMatrix[1, 2] = -6 * E * Iy / Math.Pow(L, 2);
            currentMatrix[1, 3] = 2 * E * Iy / Math.Pow(L, 1);

            currentMatrix[2, 0] = -12 * E * Iy / Math.Pow(L, 3);
            currentMatrix[2, 1] = -6 * E * Iy / Math.Pow(L, 2);
            currentMatrix[2, 2] = 12 * E * Iy / Math.Pow(L, 3);
            currentMatrix[2, 3] = -6 * E * Iy / Math.Pow(L, 2);

            currentMatrix[3, 0] = 6 * E * Iy / Math.Pow(L, 2);
            currentMatrix[3, 1] = 2 * E * Iy / Math.Pow(L, 1);
            currentMatrix[3, 2] = -6 * E * Iy / Math.Pow(L, 2);
            currentMatrix[3, 3] = 4 * E * Iy / Math.Pow(L, 1);

            return currentMatrix;
        }

        private double GetCurrentE(int materialNumber)
        {
            IMaterial currentMaterial = BeamModel.AbstractIScheme.ListIMaterial.Where(mat => mat.IntNumber == materialNumber).FirstOrDefault();
            return currentMaterial.DoubleYoungsModulus;
        }

        private double GetCurrentIy(int crossSectionNumber)
        {
            ICrossSection currentCrossSection = BeamModel.AbstractIScheme.ListICrossSection.Where(cs => cs.IntNumber == crossSectionNumber).FirstOrDefault();
            return currentCrossSection.DoubleAreaMomentOfInertia;
        }

        private double GetCurrentL(int startNodeNumber, int endNodeNumber)
        {
            INode startNode = BeamModel.AbstractIScheme.ListINode.Where(node => node.IntNumber == startNodeNumber).SingleOrDefault();
            INode endNode = BeamModel.AbstractIScheme.ListINode.Where(node => node.IntNumber == endNodeNumber).SingleOrDefault();
            double L = Math.Abs(endNode.DoubleNodePosition - startNode.DoubleNodePosition);
            return L;
        }
    }
}
