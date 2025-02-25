using UnityEngine;
using UnityEngine.UIElements;

public class DestroyBullet : MonoBehaviour
{
    [SerializeField] private TimeSecuence timeSecuence;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private Vector3 shootDirection;
    [SerializeField] private float time;

    private float bulletSpeed = 6.0f;

    void Start()
    {
        timeSecuence = FindFirstObjectByType<TimeSecuence>();
    }

    public void setShootDirection(Vector3 _shootDirection)
    {
        shootDirection = _shootDirection;
    }

    // Update is called once per frame
    void Update()
    {
        
        if ( timeSecuence.play)
        {
            rb.linearVelocity = shootDirection * bulletSpeed;
            time += Time.deltaTime;
        }
        else
        {
            rb.linearVelocity = Vector3.zero;
        }


        if (time >= 7f)
        {
            Debug.Log("aaaa0");
            Destroy(gameObject);
        }
          
    }
}
