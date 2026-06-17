using UnityEngine;

public static class NavigationHelpers {
    public enum Direction {
        None,
        Up,
        Down,
        Left,
        Right,
    }
    
    // Makes grid operations easier.
    public static Direction GetOppositeDirection(Direction direction) {            
        if (direction == Direction.Up) {
            return Direction.Down;
        }

        if (direction == Direction.Down) {
            return Direction.Up;
        }

        if (direction == Direction.Left) {
            return Direction.Right;
        }

        if (direction == Direction.Right) {
            return Direction.Left;
        }

        return Direction.None;
    }

    public static Vector2Int GetDirectionAsVector(Direction direction) {
        if (direction == Direction.Up) {
            return Vector2Int.up;
        }

        if (direction == Direction.Down) {
            return Vector2Int.down;
        }

        if (direction == Direction.Left) {
            return Vector2Int.left;
        }

        if (direction == Direction.Right) {
            return Vector2Int.right;
        }

        return Vector2Int.zero;
    }

    public static int ManhattanDistance(Vector2Int a, Vector2Int b) {
        return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
    }

}