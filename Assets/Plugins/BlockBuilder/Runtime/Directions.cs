using UnityEngine;

namespace BlockBuilder
{
    public enum Directions : byte 
    {
        // Keep in this order to easily rotate
        Right = 0,

        Back = 1,
        Left = 2,
        Forward = 3,

        Up = 4,
        Down = 5,
        LENGTH = 6,
    }
    
    public static class DirectionsExtensions
    {
        public static Directions Opposite(this Directions direction)
        {
            int oppositeDirection = (int)direction < 4 ? ((int) direction + 2) % 4 : (int)direction == 5 ? 4 : 5;
            return (Directions) oppositeDirection;
        }

        public static Vector3 ToVector3(this Directions direction)
        {
            switch (direction)
            {
                case Directions.Right: return Vector3.right;
                case Directions.Back: return Vector3.back;
                case Directions.Left: return Vector3.left;
                case Directions.Forward: return Vector3.forward;
                case Directions.Up: return Vector3.up;
                case Directions.Down: return Vector3.down;
            }

            return Vector3.zero;
        }

        public static Vector3Int ToVector3Int(this Directions direction)
        {
            switch (direction)
            {
                case Directions.Right: return Vector3Int.right;
                case Directions.Back: return Vector3Int.back;
                case Directions.Left: return Vector3Int.left;
                case Directions.Forward: return Vector3Int.forward;
                case Directions.Up: return Vector3Int.up;
                case Directions.Down: return Vector3Int.down;
            }

            return Vector3Int.zero;
        }

        public static Directions ToDirection(this Vector3Int vector)
        {
            if (vector == Vector3Int.right) return Directions.Right;
            if (vector == Vector3Int.back) return Directions.Back;
            if (vector == Vector3Int.left) return Directions.Left;
            if (vector == Vector3Int.forward) return Directions.Forward;
            if (vector == Vector3Int.up) return Directions.Up;
            if (vector == Vector3Int.down) return Directions.Down;
            return 0;
        }

        public static Directions Rotate(this Directions direction, int rotation)
        {
            if ((int) direction < 4)
                return (Directions)(((int)direction + rotation) % 4);
            return direction;
            // else
            // return (Directions)(((int)direction + rotation) % 4);
        }
    }
}