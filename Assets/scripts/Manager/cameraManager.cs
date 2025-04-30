using System.Collections;
using System.Timers;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UIElements;

public class cameraManager : MonoBehaviour
{
    [SerializeField]VolumeProfile volumeProfile;
    public UnityEngine.Rendering.Universal.Vignette colorPostProces;
    Vector3 originalPosition;
    Vector3 movingPosition;

    public Transform player; 
    public float zoomedFOV = 40f; 
    public float normalFOV = 60f; 
    public float transitionSpeed = 2f;

    Camera cam;
    Coroutine followRoutine;


    void Awake()
    {
        cam = Camera.main; 
    }
   
    void Start()
    {
        
        originalPosition = transform.position;
        
        volumeProfile = GameObject.Find("Global Volume").GetComponent<Volume>()?.profile;
        if (!volumeProfile) throw new System.NullReferenceException(nameof(VolumeProfile));
        if (colorPostProces != null)
        {
            if (!volumeProfile.TryGet(out colorPostProces)) throw new System.NullReferenceException(nameof(colorPostProces));
        }
       
        
        
    }

    public void FollowPlayer()
    {
        if (followRoutine != null) StopCoroutine(followRoutine);
        followRoutine = StartCoroutine(FollowPlayerRoutine());
    }

    public void Original()
    {
        if (followRoutine != null) StopCoroutine(followRoutine);
        followRoutine = StartCoroutine(ReturnToOriginalRoutine());
    }


    IEnumerator FollowPlayerRoutine()
    {
        Vector3 targetOffset = new Vector3(0, 5f, -5f); // Ajusta según ángulo deseado
        float elapsed = 0f;

        Vector3 startPos = transform.position;
        Vector3 targetPos = player.position + targetOffset;

        float startFOV = cam.fieldOfView;

        while (elapsed < 1f)
        {
            transform.position = Vector3.Lerp(startPos, targetPos, elapsed);
            cam.fieldOfView = Mathf.Lerp(startFOV, zoomedFOV, elapsed);
            elapsed += Time.deltaTime * transitionSpeed;
            yield return null;
        }

        transform.position = targetPos;
        cam.fieldOfView = zoomedFOV;

        // Luego de centrarse, sigue al jugador
        while (true)
        {
            transform.position = player.position + targetOffset;
            yield return null;
        }
    }

    IEnumerator ReturnToOriginalRoutine()
    {
        float elapsed = 0f;
        Vector3 startPos = transform.position;
        float startFOV = cam.fieldOfView;

        while (elapsed < 1f)
        {
            transform.position = Vector3.Lerp(startPos, originalPosition, elapsed);
            cam.fieldOfView = Mathf.Lerp(startFOV, normalFOV, elapsed);
            elapsed += Time.deltaTime * transitionSpeed;
            yield return null;
        }

        transform.position = originalPosition;
        cam.fieldOfView = normalFOV;
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
