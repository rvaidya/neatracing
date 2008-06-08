using System;
using System.Collections.Generic;
using System.Text;
using SharpNeatLib.Evolution;
using RacingGame.GameLogic;
using Microsoft.Xna.Framework;
using RacingGame.Tracks;

namespace RacingGame
{
    public static class RacingGamePointers
    {
        public static RacingGameManager racingGameManager;
        public static CarPhysics physics;
        public static Vector3[] splinePoints;
        public static int currentControlPoint = 0;

        public static TrackVertex getClosestTrackPoint()
        {
            Vector3 carPosition = physics.carPos;
            Vector3 p1 = splinePoints[currentControlPoint - 1 < 0 ? splinePoints.Length - 1 : currentControlPoint - 1];
            Vector3 p2 = splinePoints[currentControlPoint];
            Vector3 p3 = splinePoints[(currentControlPoint + 1) % splinePoints.Length];
            Vector3 p4 = splinePoints[(currentControlPoint + 2) % splinePoints.Length];

            // Calculate number of iterations we use here based
            // on the distance of the 2 points we generate new points from.
            float distance = Vector3.Distance(p2, p3);
            int numberOfIterations =
                (int)(40 * (distance / 100.0f));
            if (numberOfIterations <= 0)
                numberOfIterations = 1;
            int smallestIndex = 0;
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
            if (smallestIndex >= numberOfIterations-1)
            {
                currentControlPoint = (currentControlPoint + 1) % splinePoints.Length;
                return getClosestTrackPoint();
            }
            else return vertex;
        }
    }
}
