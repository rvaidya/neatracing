using System;
using System.Collections.Generic;
using System.Text;
using SharpNeatLib;
using Microsoft.Xna.Framework;

namespace SharpNeatExperiments.RacingGame
{
    public class RacingGameStateObject
    {
        public double speed, rotationChange, acceleration, moveFactor, virtualRotationAmount, rotateCarAfterCollision;
        public double carMass, maxSpeed;
        public Vector3 carPos, carDir, carUp, carForce, trackPosition, trackDirection;
        public int parseString(String s)
        {
            int len = NEATPointers.getParameterCount() + NEATPointers.getVectorCount() * 3;
            String[] split = s.Trim().Split(" ".ToCharArray());
            if (split.Length != len) return split.Length;
            speed = Double.Parse(split[0]);
            rotationChange = Double.Parse(split[1]);
            acceleration = Double.Parse(split[2]);
            moveFactor = Double.Parse(split[3]);
            virtualRotationAmount = Double.Parse(split[4]);
            rotateCarAfterCollision = Double.Parse(split[5]);
            carMass = Double.Parse(split[6]);
            maxSpeed = Double.Parse(split[7]);
            carPos = new Vector3(float.Parse(split[8]),float.Parse(split[9]),float.Parse(split[10]));
            carDir = new Vector3(float.Parse(split[11]),float.Parse(split[12]),float.Parse(split[13]));
            carUp = new Vector3(float.Parse(split[14]),float.Parse(split[15]),float.Parse(split[16]));
            carForce = new Vector3(float.Parse(split[17]),float.Parse(split[18]),float.Parse(split[19]));
            trackPosition = new Vector3(float.Parse(split[20]),float.Parse(split[21]),float.Parse(split[22]));
            trackDirection = new Vector3(float.Parse(split[23]), float.Parse(split[24]), float.Parse(split[25]));
            return -1;
        }
    }
}
