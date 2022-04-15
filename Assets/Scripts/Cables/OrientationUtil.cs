using UnityEngine;

namespace Cables
{
    public static class OrientationUtil
    {
        public enum Orientation { Horizontal, Vertical }
        
        public struct OrientedVector2
        {
            private Vector2 value;

            public OrientedVector2 (float x, float y)
            {
                value = new Vector2(x, y);
            }
        
            public OrientedVector2 (Vector2 vector)
            {
                value = vector;
            }

            public float X (Orientation orientation) =>
                orientation == Orientation.Horizontal ? value.x : value.y;

            public float Y(Orientation orientation) =>
                orientation == Orientation.Horizontal ? value.y : value.x;
        }
    }
}