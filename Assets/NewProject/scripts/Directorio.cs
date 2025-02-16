using UnityEngine;

public class Directorio : MonoBehaviour
{
    public static void Apuntar(GameObject instantiatedObject, Vector3 lastPosition, Vector3 Target)
    {

        Vector3 direction = Target - lastPosition;


        Quaternion rotation = Quaternion.LookRotation(Vector3.forward, direction);


        instantiatedObject.transform.rotation = rotation;
    }

    public static Vector3 mousePosition()
    {
        Vector3 mousePosition;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            mousePosition = hit.point;
            return mousePosition;
        }
        else return Vector3.zero;

      

    }

    public static float ApproximateBezierLength(Vector3 p0, Vector3 p1, Vector3 p2, int segments)
    {
        float length = 0f;
        Vector3 previousPoint = p0;

        for (int i = 1; i <= segments; i++)
        {
            float t = i / (float)segments;
            Vector3 point = Directorio.BezierCurve(t, p0, p1, p2);
            length += Vector3.Distance(previousPoint, point);
            previousPoint = point;
        }

        return length;
    }

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
