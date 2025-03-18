using UnityEngine;

public static class Directorio
{
    // Method to get mouse position in world space with better debugging
    public static Vector3 mousePosition()
    {
        {
            int layerMask = LayerMask.GetMask("floor");
            Vector3 mousePosition;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, layerMask))
            {
                mousePosition = hit.point;
                return mousePosition;
            }
            else return Vector3.zero;

        }
    }

    // Alternative implementation using Physics.Raycast
    public static Vector3 mousePositionRaycast()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            return hit.point;
        }

        // Fallback to a plane at y=0
        Plane groundPlane = new Plane(Vector3.up, Vector3.zero);
        if (groundPlane.Raycast(ray, out float distance))
        {
            return ray.GetPoint(distance);
        }

        return Vector3.zero;
    }

    // Bezier curve calculation
    public static Vector3 BezierCurve(float t, Vector3 p0, Vector3 p1, Vector3 p2)
    {
        float u = 1 - t;
        float tt = t * t;
        float uu = u * u;

        Vector3 p = uu * p0;
        p += 2 * u * t * p1;
        p += tt * p2;

        return p;
    }
}