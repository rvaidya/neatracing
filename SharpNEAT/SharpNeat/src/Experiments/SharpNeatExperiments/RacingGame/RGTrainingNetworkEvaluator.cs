using System;
using SharpNeatLib.NeuralNetwork;
using Microsoft.Xna.Framework;
using RacingGame.Helpers;
using SharpNeatExperiments.RacingGame;
using System.Collections;
using System.IO;
using SharpNeatLib.Evolution;

namespace SharpNeatLib.Experiments
{
    public class RGTrainingNetworkEvaluator : INetworkEvaluator
    {

        public string EvaluatorStateMessage
        {
            get
            {
                return string.Empty;
            }
        }
        double speed, rotationChange, acceleration, moveFactor, virtualRotationAmount, rotateCarAfterCollision;
        double carMass, maxSpeed;
        float DefaultMaxAccelerationPerSec = 2.5f,
            MaxAcceleration = 5.75f,
            MinAcceleration = -3.25f,
            MaxRotationPerSec = 1.3f,
            AirFrictionPerSpeed = 0.66f,
            MaxAirFriction = 0.66f * 200.0f,
            CarFrictionOnRoad = 17.523456789f,
            Gravity = 9.81f,
            maxAccelerationPerSec = 2.5f * 0.85f,
            MeterPerSecToMph = 1.609344f * ((60.0f * 60.0f) / 1000.0f);
        Vector3 carPos, carDir, carUp, carForce, trackPosition, trackDirection;

        public ArrayList getStates()
        {
            if (NEATPointers.states != null) return NEATPointers.states;
            NEATPointers.states = new ArrayList();
            TextReader tfr = new StreamReader("data/2008.06.09 07.03.49.txt");
            String line = tfr.ReadLine();
            while (line != null)
            {
                RacingGameStateObject o = new RacingGameStateObject();
                bool success = o.parseString(line);
                if(success) NEATPointers.states.Add(o);
                line = tfr.ReadLine();
            }
            return NEATPointers.states;
        }

        public double EvaluateNetwork(INetwork network)
        {
            ArrayList states = getStates();
            double averageSpeed = 0.0;
            for (int i = 0; i < states.Count; i++)
            {
                averageSpeed += GetSpeed(network, (RacingGameStateObject) states[i]);
            }
            averageSpeed /= states.Count;
            return averageSpeed;
        }
        public double GetSpeed(INetwork network, RacingGameStateObject o)
        {
            /*double[] parameters = NEATPointers.getParameters();
            speed = parameters[0];
            rotationChange = parameters[1];
            acceleration = parameters[2];
            moveFactor = parameters[3];
            virtualRotationAmount = parameters[4];
            rotateCarAfterCollision = parameters[5];
            carMass = parameters[6];
            maxSpeed = parameters[7];

            Vector3[] vectors = NEATPointers.getVectors();
            carPos = vectors[0];
            carDir = vectors[1];
            carUp = vectors[2];
            carForce = vectors[3];
            trackPosition = vectors[4];
            trackDirection = vectors[5];*/

            speed = o.speed;
            rotationChange = o.rotationChange;
            acceleration = o.acceleration;
            moveFactor = o.moveFactor;
            virtualRotationAmount = o.virtualRotationAmount;
            rotateCarAfterCollision = o.rotateCarAfterCollision;
            carMass = o.carMass;
            maxSpeed = o.maxSpeed;

            carPos = o.carPos;
            carDir = o.carDir;
            carUp = o.carUp;
            carForce = o.carForce;
            trackPosition = o.trackPosition;
            trackDirection = o.trackDirection;

            //EXECUTE NEAT NEURAL NETWORK

            rotationChange *= 0.95f;
            network.SetInputSignal(0, trackPosition.X);
            network.SetInputSignal(1, trackPosition.Y);
            network.SetInputSignal(2, trackPosition.Z);
            network.SetInputSignal(3, trackPosition.X);
            network.SetInputSignal(4, trackPosition.Y);
            network.SetInputSignal(5, trackPosition.Z);
            network.SetInputSignal(6, carPos.X);
            network.SetInputSignal(7, carPos.Y);
            network.SetInputSignal(8, carPos.Z);
            network.MultipleSteps(NEATPointers.stepCount);
            rotationChange = network.GetOutputSignal(0);
            double testRotationRange = Math.Abs(1.0 *
                         MaxRotationPerSec * moveFactor / 2.5f);
            double absoluteRotation = Math.Abs(rotationChange);
            if (absoluteRotation > testRotationRange)
            {
                return EvolutionAlgorithm.MIN_GENOME_FITNESS;
            }
            double newAccelerationForce = network.GetOutputSignal(1);
            double absoluteAcceleration = Math.Abs(newAccelerationForce);
            if (absoluteAcceleration > maxAccelerationPerSec)
            {
                return EvolutionAlgorithm.MIN_GENOME_FITNESS;
            }
            float slowdown = (float)network.GetOutputSignal(2);
            //END NEAT NEURAL NETWORK CODE



            // Make sure this is never below 0.001f and never above 0.5f
            // Else our formulars below might mess up or carSpeed and carForce!
            if (moveFactor < 0.001f)
                moveFactor = 0.001f;
            if (moveFactor > 0.5f)
                moveFactor = 0.5f;


            // First handle rotations (reduce last value)

            float maxRot = (float)(MaxRotationPerSec * moveFactor * 1.25);

            // Handle car rotation after collision
            if (rotateCarAfterCollision != 0)
            {
                if (rotateCarAfterCollision > maxRot)
                {
                    rotationChange += maxRot;
                    rotateCarAfterCollision -= maxRot;
                }
                else if (rotateCarAfterCollision < -maxRot)
                {
                    rotationChange -= maxRot;
                    rotateCarAfterCollision += maxRot;
                }
                else
                {
                    rotationChange += rotateCarAfterCollision;
                    rotateCarAfterCollision = 0;
                }
            }
            else
            {
                // If we are staying or moving very slowly, limit rotation!
                if (speed < 10.0f)
                    rotationChange *= 0.67f + 0.33f * speed / 10.0f;
                else
                    rotationChange *= 1.0f + (speed - 10) / 100.0f;
            }

            // Limit rotation change to MaxRotationPerSec * 1.5 (usually for mouse)
            if (rotationChange > maxRot)
                rotationChange = maxRot;
            if (rotationChange < -maxRot)
                rotationChange = -maxRot;

            // Rotate dir around up vector
            // Interpolate rotatation amount.
            virtualRotationAmount += rotationChange;
            // Smooth over 200ms
            float interpolatedRotationChange = (float)
                ((rotationChange + virtualRotationAmount) *
                moveFactor / 0.225);
            virtualRotationAmount -= interpolatedRotationChange;
            if (true)
                carDir = Vector3.TransformNormal(carDir,
                    Matrix.CreateFromAxisAngle(carUp, interpolatedRotationChange));


            // With keyboard, do heavy changes, but still smooth over 200ms
            // Up or left mouse button accelerates
            // Also support ASDW (querty) and AOEW (dvorak) shooter like controlling!

            // Limit acceleration (but drive as fast forwards as possible if we
            // are moving backwards)
            if (speed > 0 &&
                newAccelerationForce > MaxAcceleration)
                newAccelerationForce = MaxAcceleration;
            if (newAccelerationForce < MinAcceleration)
                newAccelerationForce = MinAcceleration;

            // Add acceleration force to total car force, but use the current carDir!
            if (true)
                carForce +=
                    carDir * (float)(newAccelerationForce * (moveFactor * 85));

            // Change speed with standard formula, use acceleration as our force
            float oldSpeed = (float)speed;
            Vector3 speedChangeVector = carForce / (float)carMass;
            // Only use the amount important for our current direction (slower rot)
            if (true &&
                speedChangeVector.Length() > 0)
            {
                float speedApplyFactor =
                    Vector3.Dot(Vector3.Normalize(speedChangeVector), carDir);
                if (speedApplyFactor > 1)
                    speedApplyFactor = 1;
                speed += speedChangeVector.Length() * speedApplyFactor;
            }

            // Apply friction. Basically we have 2 frictions that slow us down:
            // The friction from the contact of the wheels with the road (rolling
            // friction) and the air friction, which becomes bigger as we drive
            // faster. We need more force to overcome the resistances if we drive
            // faster. Our engine is strong enough to overcome the initial
            // car friction and air friction, but we want simulate that we need
            // more force to overcome the resistances at high speeds.
            // Usually this would require a more complex formula and the car
            // should need more fuel and force at high speeds, we just simulate that
            // by reducing the force depending on the frictions to get the same
            // effect while having our constant forces that are calculated above.

            // Max. air friction to MaxAirFiction, else driving very fast becomes
            // too hard.
            float airFriction = (float)(AirFrictionPerSpeed * Math.Abs(speed));
            if (airFriction > MaxAirFriction)
                airFriction = MaxAirFriction;
            // Don't use ground friction if we are not on the ground.
            float groundFriction = CarFrictionOnRoad;

            carForce *= 1.0f - (0.275f * 0.02125f *
                0.2f * // 20% for force slowdown
                (groundFriction + airFriction));
            // Reduce the speed, but use very low values to make the game more fun!
            float noFrictionSpeed = (float)speed;
            speed *= 1.0f - (0.01f *
                0.1f * 0.02125f *
                (groundFriction + airFriction));
            // Never change more than by 1
            if (speed < noFrictionSpeed - 1)
                speed = noFrictionSpeed - 1;

            if (true)
            {

                if (true)
                {
                    /*float slowdown =
                        1.0f - moveFactor *
                        // Use only half if we just decelerate
                        (downPressed ? BrakeSlowdown / 2 : BrakeSlowdown) *
                        // Don't brake so much if we are already driving backwards
                        (speed < 0 ? 0.33f : 1.0f);*/
                    speed *= Math.Max(0, slowdown);
                    // Limit to max. 100 mph slowdown per sec
                    if (speed > oldSpeed + 100 * moveFactor)
                        speed = (oldSpeed + 100 * moveFactor);
                    if (speed < oldSpeed - 100 * moveFactor)
                        speed = (oldSpeed - 100 * moveFactor);

                    // Remember that we slowed down for generating tracks.
                    //downPressed = true;
                }

                // Calculate pitch depending on the force
                float speedChange = (float)(speed - oldSpeed);


                // Limit speed change, never apply more than 5 per sec.
                if (speedChange < -8 * moveFactor)
                    speedChange = (float)(-8 * moveFactor);
                if (speedChange > 8 * moveFactor)
                    speedChange = (float)(8 * moveFactor);
                //carPitchPhysics.ChangePos(speedChange);
            }

            // Limit speed
            if (speed > maxSpeed)
                speed = maxSpeed;
            if (speed < -maxSpeed)
                speed = -maxSpeed;



            // Finally check for collisions with the guard rails.
            // Also handle gravity.
            ApplyGravityAndCheckForCollisions();

            double speedNEAT = 0;
            float dot = Vector3.Dot(carDir, trackDirection);
            if (dot < 0) speedNEAT = -1 * speed;
            else speedNEAT = speed;
            /*Vector3 diff = carPos - trackPosition;
            double len = diff.Length();
            return speedNEAT * dot * (1.0 / len);*/
            if (speedNEAT == 0) return EvolutionAlgorithm.MIN_GENOME_FITNESS;
            return speedNEAT * MeterPerSecToMph;
        }

        /// <summary>
        /// Current gravity speed, increases as we fly around ^^
        /// </summary>
        float gravitySpeed = 0.0f;
        /// <summary>
        /// Apply gravity to our car in case any of our wheels is in the air.
        /// Check for collisions, we only have the road and the guard rails
        /// as colision objects for this game. This way we can simplify
        /// the physics quite a lot. Usually it would be much better to have
        /// a fullblown physics engine, but thats a lot of work and goes beyond
        /// this starter kit game :)
        /// </summary>
        public void ApplyGravityAndCheckForCollisions()
        {
            // Don't do it in the menu

            // Calc normals for the guard rail with help of the next guard rail
            // position and the ground normal.
            Vector3 guardrailLeftVec =
                Vector3.Normalize(nextGuardrailLeft - guardrailLeft);
            Vector3 guardrailRightVec =
                Vector3.Normalize(nextGuardrailRight - guardrailRight);
            Vector3 guardrailLeftNormal =
                Vector3.Cross(guardrailLeftVec, groundPlaneNormal);
            Vector3 guardrailRightNormal =
                Vector3.Cross(groundPlaneNormal, guardrailRightVec);
            float roadWidth = (guardrailLeft - guardrailRight).Length();

            // Calculate position we will have NEXT frame!
            Vector3 pos = carPos;

            // Check all 4 corner points of our car.
            Vector3 carRight = Vector3.Cross(carDir, carUp);
            Vector3 carLeft = -carRight;
            // Car dimensions are 2.6m (width) x 5.6m (length) x 1.8m (height)
            // Note: This could be improved by using more points or using
            // the actual car geometry.
            // Note: We ignore the height, this way the collision is simpler.
            // We then check the height above the road to see if we are flying
            // above the guard rails out into the landscape.
            Vector3[] carCorners = new Vector3[]
            {
                // Top left
                pos + carDir * 5.6f/2.0f - carRight * 2.6f/2.0f,
                // Top right
                pos + carDir * 5.6f/2.0f + carRight * 2.6f/2.0f,
                // Bottom right
                pos - carDir * 5.6f/2.0f + carRight * 2.6f/2.0f,
                // Bottom left
                pos - carDir * 5.6f/2.0f - carRight * 2.6f/2.0f,
            };

            float applyGravity = 0;

            // Check for each corner if we collide with the guard rail
            for (int num = 0; num < carCorners.Length; num++)
            {
                // Apply gravity if we are flying, do this for each wheel.
                if (carCorners[num].Z > groundPlanePos.Z)
                    applyGravity += Gravity / 4;

                // Hit any guardrail?
                float leftDist = Vector3Helper.DistanceToLine(
                    carCorners[num], guardrailLeft, nextGuardrailLeft);
                float rightDist = Vector3Helper.DistanceToLine(
                    carCorners[num], guardrailRight, nextGuardrailRight);

                // If we are closer than 0.1f, thats a collision!
                //TODO: ignore if we are too high (higher than guardrail).
                if (leftDist < 0.1f ||
                    // Also include the case where we are father away from rightDist
                    // than the road is width.
                    rightDist > roadWidth)
                {


                    // Force car back on the road, for that calculate impulse and
                    float collisionAngle =
                        Vector3Helper.GetAngleBetweenVectors(
                        carRight, guardrailLeftNormal);
                    // Flip at 180 degrees (if driving in wrong direction)
                    if (collisionAngle > MathHelper.Pi / 2)
                        collisionAngle -= MathHelper.Pi;
                    // Just correct rotation if 0-45 degrees (slowly)
                    if (Math.Abs(collisionAngle) < MathHelper.Pi / 4.0f)
                    {
                        // Play crash sound

                        // For front wheels to full collision rotation, for back half!
                        if (num < 2)
                        {
                            rotateCarAfterCollision = -collisionAngle / 1.5f;

                            speed *= 0.93f;
                        }
                        else
                        {
                            rotateCarAfterCollision = -collisionAngle / 2.5f;

                            speed *= 0.96f;
                        }
                    }

                    // If 90-45 degrees (in either direction), make frontal crash
                    // + stop car + wobble camera
                    else if (Math.Abs(collisionAngle) < MathHelper.Pi * 3.0f / 4.0f)
                    {
                        // Also rotate car if less than 60 degrees
                        if (Math.Abs(collisionAngle) < MathHelper.Pi / 3.0f)
                            rotateCarAfterCollision = +collisionAngle / 3.0f;

                        // Play crash sound

                        // Just stop car!
                        speed = 0;
                    }

                    // For all collisions, kill the current car force
                    carForce = Vector3.Zero;

                    // Always make sure we are OUTSIDE of the collision range for
                    // the next frame. But first find out how much we have to move.
                    float speedDistanceToGuardrails =
                        (float)(speed * Math.Abs(Vector3.Dot(carDir, guardrailLeftNormal)));

                    if (leftDist > 0)
                    {
                        float correctCarPosValue = (float)((leftDist + 0.01f +
                            0.1f * speedDistanceToGuardrails * moveFactor));
                        carPos += correctCarPosValue * guardrailLeftNormal;
                    }
                }

                if (rightDist < 0.1f ||
                    // Also include the case where we are father away from rightDist
                    // than the road is width.
                    leftDist > roadWidth)
                {
                    // Force car back on the road
                    float collisionAngle =
                        Vector3Helper.GetAngleBetweenVectors(
                        carLeft, guardrailRightNormal);
                    // Flip at 180 degrees (if driving in wrong direction)
                    if (collisionAngle > MathHelper.Pi / 2)
                        collisionAngle -= MathHelper.Pi;
                    // Just correct rotation if 0-45 degrees (slowly)
                    if (Math.Abs(collisionAngle) < MathHelper.Pi / 4.0f)
                    {


                        // For front wheels to full collision rotation, for back half!
                        if (num < 2)
                        {
                            rotateCarAfterCollision = +collisionAngle / 1.5f;

                            speed *= 0.935f;
                        }
                        else
                        {
                            rotateCarAfterCollision = +collisionAngle / 2.5f;

                            speed *= 0.96f;
                        }
                    }

                    // If 90-45 degrees (in either direction), make frontal crash
                    // + stop car + wobble camera
                    else if (Math.Abs(collisionAngle) < MathHelper.Pi * 3.0f / 4.0f)
                    {
                        // Also rotate car if less than 60 degrees
                        if (Math.Abs(collisionAngle) < MathHelper.Pi / 3.0f)
                            rotateCarAfterCollision = +collisionAngle / 3.0f;


                        // Just stop car!
                        speed = 0;
                    }
                }
            }
        }

        /// <summary>
        /// Ground plane and guardrail positions.
        /// We update this every frame!
        /// </summary>
        protected Vector3 groundPlanePos, groundPlaneNormal,
            guardrailLeft, nextGuardrailLeft,
            guardrailRight, nextGuardrailRight;

        /// <summary>
        /// Set guard rails. We only calculate collisions to the current left
        /// and right guard rails, not with the complete level!
        /// </summary>
        /// <param name="setGroundPlanePos">Set ground plane position</param>
        /// <param name="setGroundPlaneNormal">Set ground plane normal</param>
        /// <param name="setGuardrailLeft">Set guardrail left</param>
        /// <param name="setNextGuardrailLeft">Set next guardrail left</param>
        /// <param name="setGuardrailRight">Set guardrail right</param>
        /// <param name="setNextGuardrailRight">Set next guardrail right</param>
        public void SetGroundPlaneAndGuardRails(
            Vector3 setGroundPlanePos, Vector3 setGroundPlaneNormal,
            Vector3 setGuardrailLeft, Vector3 setNextGuardrailLeft,
            Vector3 setGuardrailRight, Vector3 setNextGuardrailRight)
        {
            groundPlanePos = setGroundPlanePos;
            groundPlaneNormal = setGroundPlaneNormal;
            guardrailLeft = setGuardrailLeft;
            nextGuardrailLeft = setNextGuardrailLeft;
            guardrailRight = setGuardrailRight;
            nextGuardrailRight = setNextGuardrailRight;
        }

        /// <summary>
        /// Update car matrix and camera
        /// </summary>
    }
}