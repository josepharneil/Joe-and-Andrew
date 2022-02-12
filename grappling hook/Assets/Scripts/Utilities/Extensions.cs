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
    public static float DistanceToSquared(this Vector3 v0, Vector3 v1)
    {
        float deltaX = (v0.x - v1.x);
        float deltaY = (v0.y - v1.y);
        float deltaZ = (v0.z - v1.z);
        return (deltaX * deltaX) + (deltaY * deltaY) + (deltaZ * deltaZ);
    }
    public static float DistanceTo(this Vector3 v0, Vector3 v1)
    {
        return Mathf.Sqrt(DistanceToSquared(v0, v1));
    }
    public static Vector3 DirectionTo(this Vector3 v0, Vector3 v1)
    {
        return v1 - v0;
    }
    public static Vector3 DirectionFrom(this Vector3 v0, Vector3 v1)
    {
        return -DirectionTo(v0, v1);
    }
    public static Vector3 DirectionToNormalized(this Vector3 v0, Vector3 v1)
    {
        return (v1 - v0).normalized;
    }
    public static Vector3 DirectionFromNormalized(this Vector3 v0, Vector3 v1)
    {
        return -DirectionToNormalized(v0, v1);
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
    public static Vector2 DirectionFrom(this Vector2 v0, Vector2 v1)
    {
        return -DirectionTo(v0, v1);
    }
    public static Vector2 DirectionToNormalized(this Vector2 v0, Vector2 v1)
    {
        return (v1 - v0).normalized;
    }
    public static Vector2 DirectionFromNormalized(this Vector2 v0, Vector2 v1)
    {
        return -DirectionToNormalized(v0, v1);
    }
    #endregion
}
