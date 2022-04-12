using UnityEngine;

namespace Bump
{
    public static class AABB
    {
        public static Vector3 Move(Box player, Box[] others, ref int normalY)
        {
            bool isOverlapingX = false;
            float crossX = player.GetCrossX(others, float.Epsilon, ref isOverlapingX);

            float minimumX = isOverlapingX && player.Velocity.x < 0 ? crossX : float.NegativeInfinity;
            float maximumX = isOverlapingX && player.Velocity.x > 0 ? crossX : float.PositiveInfinity;

            player.Position.x += player.Velocity.x;

            if (player.Position.x > maximumX)
            {
                player.Position.x = maximumX;
            }

            if (player.Position.x < minimumX)
            {
                player.Position.x = minimumX;
            }

            bool isOverlapingZ = false;
            float crossZ = player.GetCrossZ(others, float.Epsilon, ref isOverlapingZ);

            float minimumZ = isOverlapingZ && player.Velocity.z < 0 ? crossZ : float.NegativeInfinity;
            float maximumZ = isOverlapingZ && player.Velocity.z > 0 ? crossZ : float.PositiveInfinity;

            player.Position.z += player.Velocity.z;

            if (player.Position.z > maximumZ)
            {
                player.Position.z = maximumZ;
            }

            if (player.Position.z < minimumZ)
            {
                player.Position.z = minimumZ;
            }

            bool isOverlapingY = false;
            float crossY = player.GetCrossY(others, float.Epsilon, ref isOverlapingY);

            float minimumY = isOverlapingY && player.Velocity.y < 0 ? crossY : float.NegativeInfinity;
            float maximumY = isOverlapingY && player.Velocity.y > 0 ? crossY : float.PositiveInfinity;

            player.Position.y += player.Velocity.y;

            normalY = 0;

            if (player.Position.y > maximumY)
            {
                normalY = -1;
                player.Position.y = maximumY;
            }

            if (player.Position.y < minimumY)
            {
                normalY = 1;
                player.Position.y = minimumY;
            }

            return player.Position;
        }

    }
}