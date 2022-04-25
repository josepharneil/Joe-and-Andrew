using Player;
using Unity.VisualScripting.FullSerializer;
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

    #region MeleeWeaponsAnimation

    public static int ToAnimatorStateHash(this AnimationWeaponStateName animationWeaponStateName)
    {
        Debug.Assert(animationWeaponStateName != AnimationWeaponStateName.None, "Invalid state");
        return Animator.StringToHash(animationWeaponStateName.ToString());
    }

    #endregion

    #region LineRenderer
    
    /// <summary>
    /// Draws a circle from the centre.
    /// </summary>
    /// <param name="lineRenderer"></param>
    /// <param name="radius"></param>
    /// <param name="centre"></param>
    /// <param name="steps"></param>
    public static void DrawCircle(this LineRenderer lineRenderer, float radius, Vector3 centre = default, int steps = 100)
    {
        lineRenderer.loop = true;
        lineRenderer.positionCount = steps;

        for (int currentStep = 0; currentStep < steps; currentStep++)
        {
            float circumferenceProgress = (float)currentStep / steps;
            float currentRadian = circumferenceProgress * 2 * Mathf.PI;
                
            float xScaled = Mathf.Cos(currentRadian);
            float yScaled = Mathf.Sin(currentRadian);

            float x = xScaled * radius;
            float y = yScaled * radius;

            Vector3 currentPosition = new Vector3(x, y, 0) + centre;
            lineRenderer.SetPosition(currentStep, currentPosition);
        }
    }

    /// <summary>
    /// Draws a rectangle from the centre.
    /// </summary>
    /// <param name="lineRenderer"></param>
    /// <param name="width"></param>
    /// <param name="height"></param>
    /// <param name="centre"></param>
    public static void DrawRectangle(this LineRenderer lineRenderer, float width, float height, Vector3 centre = default)
    {
        // Line renderer actually draws starting from each corner, so offset the centre to be the top left corner.  
        centre.x -= width / 2f;
        centre.y -= height / 2f;

        lineRenderer.loop = true;
        lineRenderer.positionCount = 4;
        lineRenderer.SetPosition(0, new Vector3(0f, 0f, 0f) + centre);
        lineRenderer.SetPosition(1, new Vector3(0f, height, 0f) + centre);
        lineRenderer.SetPosition(2, new Vector3(width, height, 0f) + centre);
        lineRenderer.SetPosition(3, new Vector3(width, 0f, 0f) + centre);
    }

    #endregion
}
