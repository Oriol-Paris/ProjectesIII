using System.Collections;
using System.Timers;
using UnityEngine;

public class cameraFeedbackManager : MonoBehaviour
{
    Vector3 originalPosition;
    float elapsed;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        originalPosition = transform.position;

    }

    public IEnumerator CameraShake(float timeLenght, float range)
    {
        elapsed = 0.0f;
        while (elapsed < timeLenght)
        {
            float x = Random.Range(originalPosition.x - 1.0f, originalPosition.x + 1.0f) * range;
            float z = Random.Range(originalPosition.z - 1.0f, originalPosition.z + 1.0f) * range;
            transform.position = new Vector3(x, originalPosition.y, z);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = originalPosition;

    }
}
