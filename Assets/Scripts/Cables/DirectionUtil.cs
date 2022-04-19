using UnityEngine;

namespace Cables
{
    public static class DirectionUtil
    {
        public enum Direction { Left, Right, Up, Down }

        private static Direction DirectionBetweenPoints(Vector2 a, Vector2 b, OrientationUtil.Orientation orientation)
        {
            if (orientation == OrientationUtil.Orientation.Horizontal)
            {
                return b.y - a.y > 0 ? Direction.Down : Direction.Up;
            }
            else
            {
                return b.x - a.x < 0 ? Direction.Left : Direction.Right;
            }
        }
    }
}