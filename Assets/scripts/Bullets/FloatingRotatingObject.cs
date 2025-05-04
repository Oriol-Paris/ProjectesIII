using UnityEngine;

public class FloatingRotatingObject : MonoBehaviour
{
    [Header("Rotación")]
    public float rotationSpeed = 45f; 

    [Header("Flotación")]
    public float floatAmplitude = 0.2f; 
    public float floatFrequency = 1f;  

    private Vector3 startPos;

    [SerializeField] private GameObject torch;

    void Start()
    {
        startPos = transform.position;
    }

    void Update()
    {
        
        transform.Rotate(0f, rotationSpeed * Time.deltaTime, 0f);

       
        float newY = startPos.y + Mathf.Sin(Time.time * floatFrequency) * floatAmplitude;
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);

        if (GetComponent<SpriteRenderer>().enabled == false )
        {
            torch.SetActive(false);
        }
    }
}