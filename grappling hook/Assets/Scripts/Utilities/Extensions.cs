using UnityEngine;

public static class Extensions
{
    // public static Vector3 AsVector3(this Vector2 v2)
    // {
    //     return v2;
    // }

    // public static float DistanceSquared(this Vector2 v2)
    // {
    //     
    //     return (v2.x * v2.x) + (v2.y * v2.y);
    // }
    //
    // public static float DistanceSquared(this Vector3 v3)
    // {
    //     return (v3.x * v3.x) + (v3.y * v3.y) + (v3.z * v3.z);
    // }
    
    // public static float DistanceSquared(this Vector2 a, Vector2 b)
    // {
    //     float num1 = a.x - b.x;
    //     float num2 = a.y - b.y;
    //     return (float) ((double) num1 * (double) num1 + (double) num2 * (double) num2);
    // }

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

    
    
}
