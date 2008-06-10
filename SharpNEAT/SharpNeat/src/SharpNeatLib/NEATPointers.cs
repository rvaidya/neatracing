using System;
using System.Collections.Generic;
using System.Text;
using SharpNeatLib.Evolution;
using SharpNeatLib.NeuralNetwork;
using Microsoft.Xna.Framework;
using System.IO;
using System.Collections;

namespace SharpNeatLib
{
    public static class NEATPointers
    {
        public const int inputCount = 8;
        public const int outputCount = 3;
        public const int stepCount = 1;
        public static ArrayList states;

        /*
         * Parameters:
         * 0: speed
         * 1: rotationChange
         * 2: acceleration
         * 3: moveFactor
         * 4: virtualRotationAmount
         * 5: rotateCarAfterCollision
         * 6: carMass
         * 7: maxSpeed
         */
        private static double[] parameters = new double[8];
        public static int getParameterCount() { return parameters.Length; }
        /*
         * Vector Parameters:
         * 0: carPos
         * 1: carDir
         * 2: carUp
         * 3: carForce
         * 4: trackPosition
         * 5: trackDirection
         */
        private static Vector3[] vectorParameters = new Vector3[6];
        public static int getVectorCount() { return vectorParameters.Length; }
        private static IGenome bestGenome = null;
        public static IActivationFunction activationFunction = null;


        public static object parameterLock = new object();
        public static double[] getParameters()
        {
            lock (parameterLock)
            {
                return parameters;
            }
        }
        public static double getParameter(int i)
        {
            lock (parameterLock)
            {
                return parameters[i];
            }
        }
        public static void setParameters(double[] p)
        {
            lock (parameterLock)
            {
                parameters = p;
            }
        }
        public static Vector3[] getVectors()
        {
            lock (parameterLock)
            {
                return vectorParameters;
            }
        }
        public static Vector3 getVector(int i)
        {
            lock (parameterLock)
            {
                return vectorParameters[i];
            }
        }
        public static void setVectors(Vector3[] p)
        {
            lock (parameterLock)
            {
                vectorParameters = p;
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
