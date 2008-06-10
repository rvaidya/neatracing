using System;
using System.Collections;

using SharpNeatLib.Evolution;
using SharpNeatLib.NeuralNetwork;
using System.IO;
using SharpNeatExperiments.RacingGame;

namespace SharpNeatLib.Experiments
{
    public class RGTrainingExperiment : IExperiment
    {
        IPopulationEvaluator populationEvaluator;
        IActivationFunction activationFunction = new SteepenedSigmoid();

        #region Constructor

        public RGTrainingExperiment()
        {
            NEATPointers.activationFunction = activationFunction;
        }

        #endregion

        #region IExperiment Members

        /// <summary>
        /// This method is called immediately following instantiation of an experiment. It is used
        /// to pass in a hashtable of string key-value pairs from the 'experimentParameters' 
        /// block of the experiment configuration block within the application config file.
        /// 
        /// If no parameters where specified then an empty Hashtable is used.
        /// </summary>
        /// <param name="parameterTable"></param>
        public void LoadExperimentParameters(Hashtable parameterTable)
        {
        }

        public IPopulationEvaluator PopulationEvaluator
        {
            get
            {
                if (populationEvaluator == null)
                    ResetEvaluator(activationFunction);

                return populationEvaluator;
            }
        }

        public void ResetEvaluator(IActivationFunction activationFn)
        {
            populationEvaluator = new SingleFilePopulationEvaluator(new RGTrainingNetworkEvaluator(), activationFn);
        }

        public int InputNeuronCount
        {
            get
            {
                return 6;
            }
        }

        public int OutputNeuronCount
        {
            get
            {
                return 3;
            }
        }

        public NeatParameters DefaultNeatParameters
        {
            get
            {
                return new NeatParameters();
            }
        }

        public IActivationFunction SuggestedActivationFunction
        {
            get
            {
                return activationFunction;
            }
        }

        public AbstractExperimentView CreateExperimentView()
        {
            return null;
        }

        public string ExplanatoryText
        {
            get
            {
                return @"3-Racing game experiment with eight inputs and three outputs.  The inputs are, in order:
track x, track y, track z, car position x, car position y, car position z.  Optional
inputs include track tangent x, track tangent y, track tangent z.
The outputs are, in order:
Turn angle, acceleration amount.

The fitness of the system is determined by the speed of the car.";
            }
        }

        #endregion
    }
}
