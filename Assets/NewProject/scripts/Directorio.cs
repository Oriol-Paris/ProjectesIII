using UnityEngine;

public class Directorio : MonoBehaviour
{
    public static void Apuntar(GameObject instantiatedObject, Vector3 lastPosition, Vector3 Target)
    {

        Vector3 direction = Target - lastPosition;


        Quaternion rotation = Quaternion.LookRotation(Vector3.forward, direction);


        instantiatedObject.transform.rotation = rotation;
    }
}
