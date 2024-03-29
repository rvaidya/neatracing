using System;
using SharpNeatLib.NeuralNetwork;

namespace SharpNeatLib.Experiments
{
    public class NXNetworkEvaluator : INetworkEvaluator
    {
        #region INetworkEvaluator

        public int stepCount = 3;

        public double EvaluateNetwork(INetwork network)
        {
            double fitness = 0.0;
            bool success = true;

            // 64 test cases.
            for (int i = 0; i < 64; i++)
            {
                // Apply bitmask to i and shift left to generate the input signals.
                // In addition we scale 0->1 to be 0.1->0.9.
                // Note. We can eliminate all the maths by pre-building a table of test signals. 
                // Using this approach instead makes the code easier to scale up to the 11 and 20 
                // multiplexer problems.
                int tmp = i;
                for (int j = 0; j < network.InputNeuronCount; j++)
                {
                    network.SetInputSignal(j, ((tmp & 0x1) * 0.9) + 0.1);
                    tmp >>= 1;
                }

                // Activate the network.
                network.MultipleSteps(stepCount);

                // Get network's answer.
                double output = network.GetOutputSignal(0);

                // Determine the correct answer by using highly cryptic bit manipulation :)
                // The condition is true if the correct answer is true (1.0).
                if (((1 << (2 + (i & 0x3))) & i) != 0)
                {	// correct answer = true.
                    // Assign fitness on sliding scale between 0.0 and 1.0.
                    fitness += output;

                    if (output < 0.5)
                        success = false;
                }
                else
                {	// correct answer = false.
                    // Assign fitness on sliding scale between 0.0 and 1.0.
                    fitness += 1.0 - output;

                    if (output >= 0.5)
                        success = false;
                }

                network.ClearSignals();
            }

            if (success)
                fitness += 1000.0;

            return fitness;
        }

        public string EvaluatorStateMessage
        {
            get
            {
                return string.Empty;
            }
        }

        #endregion
    }
}
