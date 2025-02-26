using System.Collections;
using System.Timers;
using UnityEngine;
using UnityEngine.Rendering;

public class cameraManager : MonoBehaviour
{
    VolumeProfile volumeProfile;
    UnityEngine.Rendering.Universal.Vignette colorPostProces;
    Vector3 originalPosition;
    Vector3 movingPosition;
    float elapsed;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        originalPosition = transform.position;
        Debug.LogWarning(originalPosition);
        
        volumeProfile = GameObject.Find("Global Volume").GetComponent<Volume>()?.profile;
        if (!volumeProfile) throw new System.NullReferenceException(nameof(VolumeProfile));
        
        if (!volumeProfile.TryGet(out colorPostProces)) throw new System.NullReferenceException(nameof(colorPostProces));

        //StartCoroutine(Shake(1, 2));
        
    }



    public IEnumerator Shake(float timeLenght, float range)
    {
        elapsed = 0.0f;
        while (elapsed < timeLenght)
        {
            movingPosition= Vector3.zero;

            float x = 0;
            float z = 0;

            x = Random.Range(originalPosition.x - 1.0f * range, originalPosition.x + 1.0f * range);
            z = Random.Range(originalPosition.z - 1.0f * (range / 2), originalPosition.z + 1.0f * (range / 2)) ;

            Debug.Log(originalPosition);
            movingPosition.x = x;
            movingPosition.y = transform.position.y;
            movingPosition.z = z;

            transform.position = movingPosition;
           
            elapsed += Time.deltaTime;

            
           
            yield return null;
            
        }

        transform.position = originalPosition;

    }

    public IEnumerator Flash(float intensity, float length, Color color)
    {
        Debug.Log("Color set");
        colorPostProces.color.Override(color);
        colorPostProces.intensity.Override(intensity);
        elapsed = 0.0f;
        while (elapsed < length)
        {
            colorPostProces.intensity.Override(intensity-=0.2f);

            elapsed += Time.deltaTime;
            Debug.Log(colorPostProces.intensity);
            yield return null;
        }
        colorPostProces.intensity.Override(0);
        yield return null;
    }
}
