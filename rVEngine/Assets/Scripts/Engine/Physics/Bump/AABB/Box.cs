using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bump
{
    public struct Box
    {
        public Vector3 Scale;
        public Vector3 Position;
        public Vector3 Velocity;

        public Box(Vector3 position, Vector3 scale)
        {
            Position = position;
            Scale = scale;

            Velocity = Vector3.zero;
        }

        public Box(Vector3 position, Vector3 scale, Vector3 veclocity)
        {
            Position = position;
            Scale = scale;
            Velocity = veclocity;
        }

        public bool IsOverlaping(Box other)
        {
            return IsOverlaping(Box.Clone(this, float.Epsilon), other);
        }

        public static Box Clone(Box other)
        {
            Box box = new Box();

            box.Position = other.Position;
            box.Scale = other.Scale;
            box.Velocity = other.Velocity;

            return box;
        }

        public float GetCrossX(Box other, float epsilon, ref bool isOverlaping)
        {
            Box box = Box.Clone(this, epsilon);

            bool isOverlapingZ = ((box.Position.z + (box.Scale.z)) > other.Position.z && box.Position.z < (other.Position.z + (other.Scale.z)));
            bool isOverlapingY = ((box.Position.y + (box.Scale.y)) > other.Position.y && box.Position.y < (other.Position.y + (other.Scale.y)));

            isOverlaping = isOverlapingY && isOverlapingZ;

            if (isOverlaping)
            {
                isOverlaping = false;

                if (this.Velocity.x > 0 && this.GetCenter().x < other.GetCenter().x)
                {
                    isOverlaping = true;
                    return (other.Position.x - this.Scale.x) - epsilon;
                }

                if (this.Velocity.x < 0 && this.GetCenter().x > other.GetCenter().x)
                {
                    isOverlaping = true;
                    return (other.Position.x + other.Scale.x) + epsilon;
                }
            }

            return 0f;
        }

        public float GetCrossY(Box other, float epsilon, ref bool isOverlaping)
        {
            Box box = Box.Clone(this, epsilon);

            bool isOverlapingX = ((box.Position.x + (box.Scale.x)) > other.Position.x && box.Position.x < (other.Position.x + (other.Scale.x)));
            bool isOverlapingZ = ((box.Position.z + (box.Scale.z)) > other.Position.z && box.Position.z < (other.Position.z + (other.Scale.z)));

            isOverlaping = isOverlapingX && isOverlapingZ;

            if (isOverlaping)
            {
                isOverlaping = false;

                if (this.Velocity.y > 0 && this.GetCenter().y < other.GetCenter().y)
                {
                    isOverlaping = true;
                    return (other.Position.y - this.Scale.y) - epsilon;
                }

                if (this.Velocity.y < 0 && this.GetCenter().y > other.GetCenter().y)
                {
                    isOverlaping = true;
                    return (other.Position.y + other.Scale.y) + epsilon;
                }
            }

            return 0f;
        }

        public float GetCrossZ(Box other, float epsilon, ref bool isOverlaping)
        {
            Box box = Box.Clone(this, epsilon);

            bool isOverlapingY = ((box.Position.y + (box.Scale.y)) > other.Position.y && box.Position.y < (other.Position.y + (other.Scale.y)));
            bool isOverlapingX = ((box.Position.x + (box.Scale.x)) > other.Position.x && box.Position.x < (other.Position.x + (other.Scale.x)));

            isOverlaping = isOverlapingY && isOverlapingX;

            if (isOverlaping)
            {
                isOverlaping = false;

                if (this.Velocity.z > 0 && this.GetCenter().z < other.GetCenter().z)
                {
                    isOverlaping = true;
                    return (other.Position.z - this.Scale.z) - epsilon;
                }

                if (this.Velocity.z < 0 && this.GetCenter().z > other.GetCenter().z)
                {
                    isOverlaping = true;
                    return (other.Position.z + other.Scale.z) + epsilon;
                }
            }

            return 0f;
        }

        public float GetCrossX(Box[] others, float epsilon, ref bool isOverlaping)
        {
            float distanceGlobal = float.PositiveInfinity;
            float crossGHit = 0;

            foreach (var other in others)
            {
                bool isLOverlaping = false;
                float crossLHit = GetCrossX(other, epsilon, ref isLOverlaping);

                float distanceLocal = Mathf.Abs(this.Position.x - crossLHit);

                if (isLOverlaping)
                {
                    if (distanceGlobal > distanceLocal)
                    {
                        isOverlaping = true;

                        crossGHit = crossLHit;
                        distanceGlobal = distanceLocal;
                    }
                }
            }

            return crossGHit;
        }

        public float GetCrossY(Box[] others, float epsilon, ref bool isOverlaping)
        {
            float distanceGlobal = float.PositiveInfinity;
            float crossGHit = 0;

            foreach (var other in others)
            {
                bool isLOverlaping = false;
                float crossLHit = GetCrossY(other, epsilon, ref isLOverlaping);

                float distanceLocal = Mathf.Abs(this.Position.y - crossLHit);

                if (isLOverlaping)
                {
                    if (distanceGlobal > distanceLocal)
                    {
                        isOverlaping = true;

                        crossGHit = crossLHit;
                        distanceGlobal = distanceLocal;
                    }
                }
            }

            return crossGHit;
        }

        public float GetCrossZ(Box[] others, float epsilon, ref bool isOverlaping)
        {
            float distanceGlobal = float.PositiveInfinity;
            float crossGHit = 0;

            foreach (var other in others)
            {
                bool isLOverlaping = false;
                float crossLHit = GetCrossZ(other, epsilon, ref isLOverlaping);

                float distanceLocal = Mathf.Abs(this.Position.z - crossLHit);

                if (isLOverlaping)
                {
                    if (distanceGlobal > distanceLocal)
                    {
                        isOverlaping = true;

                        crossGHit = crossLHit;
                        distanceGlobal = distanceLocal;
                    }
                }
            }

            return crossGHit;
        }

        public Vector3 GetCenter()
        {
            return Position + Scale / 2f;
        }

        public static bool IsOverlaping(Box box, Box other)
        {
            bool isCrossX = ((box.Position.x + (box.Scale.x)) > other.Position.x && box.Position.x < (other.Position.x + (other.Scale.x)));
            bool isCrossZ = ((box.Position.z + (box.Scale.z)) > other.Position.z && box.Position.z < (other.Position.z + (other.Scale.z)));
            bool isCrossY = ((box.Position.y + (box.Scale.y)) > other.Position.y && box.Position.y < (other.Position.y + (other.Scale.y)));

            return (isCrossX && isCrossY && isCrossZ);
        }

        public static Box Clone(Box other, float epsilon)
        {
            Box box = Box.Clone(other);

            Vector3 kEpsilon = new Vector3(epsilon, epsilon, epsilon);

            box.Position += kEpsilon;
            box.Scale -= kEpsilon * 2;

            return box;
        }
    }
}