using UnityEngine;
using UnityEngine.UIElements;

public class DestroyBullet : MonoBehaviour
{
    [SerializeField] private TimeSecuence timeSecuence;
    [SerializeField] private Rigidbody rb;
    [SerializeField] public Vector3 shootDirection;
    [SerializeField] public float time;
    [SerializeField] private bool fromPlayer;
    [SerializeField] public int damage;

    private cameraManager _camera;
    [SerializeField] float shootCShakeTime;
    [SerializeField] float shootCShakeRange;

    public float bulletSpeed = 3.0f;

    void Start()
    {
        if (fromPlayer)
        {
            damage = FindAnyObjectByType<BulletCollection>().GetBullet(BulletType.GUN).damage;
        }
        else
        {
            damage = 1;
        }

        timeSecuence = FindFirstObjectByType<TimeSecuence>();
        _camera = FindAnyObjectByType<cameraManager>();
        StartCoroutine(_camera.Shake(shootCShakeTime, shootCShakeRange));
        transform.position = new Vector3(transform.position.x,0,transform.position.z);
    }

    public void setShootDirection(Vector3 _shootDirection,bool itsFromPlayer)
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
            
            Destroy(gameObject);
        }

    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision != null)
        {
            if (collision.gameObject.CompareTag("envairoment"))
            {
                Destroy(gameObject);
            }

            if (fromPlayer && collision.gameObject.GetComponent<EnemyBase>() != null)
            {
                EnemyBase hit = collision.gameObject.GetComponent<EnemyBase>();
                hit.Damage(damage, collision.gameObject);
                Destroy(gameObject);
            }

            if(!fromPlayer && collision.gameObject.GetComponent<PlayerBase>() != null)
            {
                PlayerBase hit = collision.gameObject.GetComponent<PlayerBase>();
                hit.Damage(damage, collision.gameObject);
                Destroy(gameObject);
            }
        }
    }
}
