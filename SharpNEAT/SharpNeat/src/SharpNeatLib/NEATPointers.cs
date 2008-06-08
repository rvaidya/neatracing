using System;
using System.Collections.Generic;
using System.Text;
using SharpNeatLib.Evolution;
using SharpNeatLib.NeuralNetwork;

namespace SharpNeatLib
{
    public static class NEATPointers
    {
        public static IGenome bestGenome = null;
        public static IActivationFunction activationFunction = null;
    }
}
