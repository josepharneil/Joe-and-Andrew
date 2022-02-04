using UnityEngine;

public static class Extensions
{
    #region Vector3
    public static bool IsRightOf(this Vector3 v0, Vector3 v1)
    {
        return v0.x > v1.x;
    }
    public static bool IsLeftOf(this Vector3 v0, Vector3 v1)
    {
        return v0.x < v1.x;
    }
    public static bool IsAbove(this Vector3 v0, Vector3 v1)
    {
        return v0.y > v1.y;
    }
    public static bool IsBelow(this Vector3 v0, Vector3 v1)
    {
        return v0.y < v1.y;
    }
    #endregion

    #region Vector2
    public static bool IsRightOf(this Vector2 v0, Vector2 v1)
    {
        return v0.x > v1.x;
    }
    public static bool IsLeftOf(this Vector2 v0, Vector2 v1)
    {
        return v0.x < v1.x;
    }
    public static bool IsAbove(this Vector2 v0, Vector2 v1)
    {
        return v0.y > v1.y;
    }
    public static bool IsBelow(this Vector2 v0, Vector2 v1)
    {
        return v0.y < v1.y;
    }
    public static float DistanceToSquared(this Vector2 v0, Vector2 v1)
    {
        float deltaX = (v0.x - v1.x);
        float deltaY = (v0.y - v1.y);
        return (deltaX * deltaX) + (deltaY * deltaY);
    }
    public static float DistanceTo(this Vector2 v0, Vector2 v1)
    {
        return Mathf.Sqrt(DistanceToSquared(v0, v1));
    }
    public static Vector2 DirectionTo(this Vector2 v0, Vector2 v1)
    {
        return v1 - v0;
    }
    #endregion
}
