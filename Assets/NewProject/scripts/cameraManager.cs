using System.Collections;
using System.Timers;
using UnityEngine;
using UnityEngine.Rendering;

public class cameraManager : MonoBehaviour
{
    VolumeProfile volumeProfile;
    public UnityEngine.Rendering.Universal.Vignette colorPostProces;
    Vector3 originalPosition;
    Vector3 movingPosition;
   
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
        originalPosition = transform.position;
        
        volumeProfile = GameObject.Find("Global Volume").GetComponent<Volume>()?.profile;
        if (!volumeProfile) throw new System.NullReferenceException(nameof(VolumeProfile));
        
        if (!volumeProfile.TryGet(out colorPostProces)) throw new System.NullReferenceException(nameof(colorPostProces));
        StartCoroutine(FadeInVignette(0.3f, 0.5f, Color.red));
        
    }

    public IEnumerator Shake(float timeLenght, float range)
    {
        float elapsed = 0.0f;
        while (elapsed < timeLenght)
        {
            movingPosition= Vector3.zero;

            float x = 0;
            float z = 0;

            x = Random.Range(originalPosition.x - 1.0f * range, originalPosition.x + 1.0f * range);
            z = Random.Range(originalPosition.z - 1.0f * (range / 2), originalPosition.z + 1.0f * (range / 2)) ;


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
        float elapsed = 0.0f;

        Debug.Log("Color set");
        colorPostProces.color.Override(color);
        colorPostProces.intensity.Override(intensity);
        float elapsed = 0.0f;
        while (elapsed < length)
        {
            colorPostProces.intensity.Override(intensity - (elapsed / length) * intensity);

            elapsed += Time.deltaTime;
        
            yield return null;
        }
        colorPostProces.intensity.Override(0);
        yield return null;
    }
    public IEnumerator FadeInVignette(float intensity, float length, Color color)
    {
        Debug.Log("Color set");
        colorPostProces.color.Override(color);
        
        float elapsed = 0.0f;
        while (elapsed < length)
        {
            colorPostProces.intensity.Override((elapsed/length)*intensity);

            elapsed += Time.deltaTime;

            yield return null;
        }

        yield return null;
    }
    public void RemoveVignette()
    {
        colorPostProces.intensity.Override(0);
    }
}
