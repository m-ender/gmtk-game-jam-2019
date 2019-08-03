
using System;
using UnityEngine;

namespace GMTKGJ2019
{
    public enum Direction
    {
        None,
        North,
        West,
        South,
        East,
    }

    public static class DirectionExtensionMethods
    {
        public static bool IsHorizontal(this Direction dir)
            => dir == Direction.East || dir == Direction.West;

        public static Direction Reverse(this Direction dir)
        {
            switch (dir)
            {
            case Direction.North: return Direction.South;
            case Direction.West: return Direction.East;
            case Direction.South: return Direction.North;
            case Direction.East: return Direction.West;
            default: return Direction.None;
            }
        }

        public static float ToAngle(this Direction dir)
        {
            switch (dir)
            {
            case Direction.North: return 0;
            case Direction.West: return 90;
            case Direction.South: return 180;
            case Direction.East: return 270;
            default: throw new ArgumentException("Invalid direction.");
            }
        }

        public static Vector2 ToVector2(this Direction dir)
        {
            switch (dir)
            {
            case Direction.North: return new Vector2(0, 1);
            case Direction.West: return new Vector2(-1, 0);
            case Direction.South: return new Vector2(0, -1);
            case Direction.East: return new Vector2(1, 0);
            default: throw new ArgumentException("Invalid direction.");
            }
        }
    }
}