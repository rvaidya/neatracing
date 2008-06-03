using System;
using System.Collections;

using SharpNeatLib.Evolution;
using SharpNeatLib.NeuralNetwork;
using SharpNeatLib.Experiments.Views;

namespace SharpNeatLib.Experiments
{
    public class NEATRacingExperiment : IExperiment
    {
        IPopulationEvaluator populationEvaluator;
        IActivationFunction activationFunction = new SteepenedSigmoid();

        #region Constructor

        public NEATRacingExperiment()
        {
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
            populationEvaluator = new SingleFilePopulationEvaluator(new NEATRacingNetworkEvaluator(), activationFn);
        }

        public int InputNeuronCount
        {
            get
            {
                return 2;
            }
        }

        public int OutputNeuronCount
        {
            get
            {
                return 6;
            }
        }

        public NeatParameters DefaultNeatParameters
        {
            get
            {
                NeatParameters np = new NeatParameters();
                np.populationSize = 1000;
                np.pOffspringAsexual = 0.8;
                np.pOffspringSexual = 0.2;

                np.targetSpeciesCountMin = 40;
                np.targetSpeciesCountMax = 50;

                return np;
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
            return new NEATRacingExperimentView();
        }

        public string ExplanatoryText
        {
            get
            {
                return @"6-Racing game neural network with 2 input nodes and 6 output nodes.";
            }
        }

        #endregion
    }
}
