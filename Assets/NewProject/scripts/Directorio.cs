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
