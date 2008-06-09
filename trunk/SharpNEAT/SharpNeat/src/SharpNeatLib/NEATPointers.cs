using System;
using System.Collections.Generic;
using System.Text;
using SharpNeatLib.Evolution;
using SharpNeatLib.NeuralNetwork;

namespace SharpNeatLib
{
    public static class NEATPointers
    {
        public const int inputCount = 8;
        public const int outputCount = 2;
        private static double speed = 0;
        private static IGenome bestGenome = null;
        public static IActivationFunction activationFunction = null;










        public static object speedLock = new object();
        public static double getSpeed()
        {
            lock (speedLock)
            {
                return speed;
            }
        }
        
        public static void setSpeed(double s)
        {
            lock (speedLock)
            {
                speed = s;
            }
        }
        public static object genomeLock = new object();
        public static IGenome getGenome()
        {
            lock (genomeLock)
            {
                return bestGenome;
            }
        }
        public static void setGenome(IGenome bg)
        {
            lock (genomeLock)
            {
                bestGenome = bg;
            }
        }
    }
}
