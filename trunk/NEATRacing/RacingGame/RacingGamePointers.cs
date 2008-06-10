using System;
using System.Collections.Generic;
using System.Text;
using SharpNeatLib.Evolution;
using RacingGame.GameLogic;
using Microsoft.Xna.Framework;
using RacingGame.Tracks;
using SharpNeat;

namespace RacingGame
{
    public static class RacingGamePointers
    {
        public static RacingGameManager racingGameManager;
        public static CarPhysics physics;
        public static Vector3[] splinePoints;
        public static int currentControlPoint = 0;
        public static Vector3 trackPos = new Vector3(0,0,0);
        public static Vector3 trackDir = new Vector3(0, 0, 0);
        public static Form1 ui;

        public static TrackVertex getClosestTrackPoint()
        {
            float distance = float.MaxValue;
            int smallestIndex = 0;
            Vector3 carPosition = physics.carPos;
            for (int i = 0; i < splinePoints.Length; i++)
            {
                float newDistance = Vector3.Distance(splinePoints[i], carPosition);
                if (newDistance < distance)
                {
                    smallestIndex = i;
                    distance = newDistance;
                }
            }
            float d1 = Vector3.Distance(splinePoints[smallestIndex - 1 < 0 ? splinePoints.Length - 1 : smallestIndex - 1],carPosition);
            float d2 = Vector3.Distance(splinePoints[(smallestIndex + 1) % splinePoints.Length],carPosition);
            if(d1 < d2)
            {
                //smallestIndex = smallestIndex - 1 < 0 ? splinePoints.Length - 1 : smallestIndex - 1;
            }
            currentControlPoint = smallestIndex;
            Vector3 p1 = splinePoints[currentControlPoint - 1 < 0 ? splinePoints.Length - 1 : currentControlPoint - 1];
            Vector3 p2 = splinePoints[currentControlPoint];
            Vector3 p3 = splinePoints[(currentControlPoint + 1) % splinePoints.Length];
            Vector3 p4 = splinePoints[(currentControlPoint + 2) % splinePoints.Length];

            // Calculate number of iterations we use here based
            // on the distance of the 2 points we generate new points from.
            distance = Vector3.Distance(p2, p3);
            int numberOfIterations =
                (int)(40 * (distance / 100.0f));
            if (numberOfIterations <= 0)
                numberOfIterations = 1;
            smallestIndex = 0;
            distance = 0;
            TrackVertex vertex = null;
            for (int iter = 0; iter < numberOfIterations; iter++)
            {
                TrackVertex newVertex = new TrackVertex(
                    Vector3.CatmullRom(p1, p2, p3, p4,
                    iter / (float)numberOfIterations));
                if (vertex == null)
                {
                    vertex = newVertex;
                    smallestIndex = iter;
                    distance = Vector3.Distance(vertex.pos,carPosition);
                }
                else
                {
                    float newDist = Vector3.Distance(newVertex.pos, carPosition);
                    if (newDist < distance)
                    {
                        distance = newDist;
                        vertex = newVertex;
                        smallestIndex = iter;
                    }
                }
            }
            return vertex;
        }
    }
}
