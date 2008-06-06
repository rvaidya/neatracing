using System;
using System.Collections.Generic;
using System.Text;
using SharpNeatLib.Experiments;
using SharpNeatLib.NeuralNetwork;

namespace NEAT4XNA.NEAT
{
    class NXNetwork
    {
        int stepCount, inputCount, outputCount;
        delegate double FitnessFunction(double[] d);
        public FitnessFunction fitnessFunction;
        INetwork network;

        public NXNetwork(int ic, int oc)
        {
            inputCount = ic;
            outputCount = oc;
        }

        public void stepNetwork()
        {
            {
                network.MultipleSteps(stepCount);
                double[] output = new double[outputCount];
                for (int i = 0; i < outputCount; i++)
                {
                    output[i] = network.GetOutputSignal(i);
                }
                network.ClearSignals();
                fitnessFunction(output);
            }
        }
    }
}
