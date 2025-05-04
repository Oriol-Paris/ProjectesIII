using UnityEngine;

public class spin : MonoBehaviour
{

    void Update()
    {
        transform.Rotate(0f, 0f, 120 * Time.deltaTime);
    }
}
