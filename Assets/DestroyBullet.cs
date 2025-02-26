using UnityEngine;
using UnityEngine.UIElements;

public class DestroyBullet : MonoBehaviour
{
    [SerializeField] private TimeSecuence timeSecuence;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private Vector3 shootDirection;
    [SerializeField] private float time;
    [SerializeField] private bool fromPlayer;

    private GameObject _camera;
    [SerializeField] float shootCShakeTime;
    [SerializeField] float shootCShakeRange;
    private float bulletSpeed = 3.0f;

    void Start()
    {
        timeSecuence = FindFirstObjectByType<TimeSecuence>();
        _camera = GameObject.FindGameObjectWithTag("MainCamera");
        StartCoroutine(_camera.GetComponent<cameraManager>().Shake(shootCShakeTime, shootCShakeRange));
    }

    public void setShootDirection(Vector3 _shootDirection)
    {
        shootDirection = _shootDirection;
    }

    // Update is called once per frame
    void Update()
    {

        if (timeSecuence.play)
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

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.CompareTag("Walls"))
        {
            Destroy(gameObject);
        }

        if (fromPlayer)
        {
            EnemyBase hit = collision.gameObject.GetComponent<EnemyBase>();
            hit.Damage(1);
            Destroy(gameObject);
        }

        else
        {
            PlayerBase hit = collision.gameObject.GetComponent<PlayerBase>();
            hit.Damage(1);
            Destroy(gameObject);
        }

    }
}
