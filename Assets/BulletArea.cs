using UnityEngine;
using UnityEngine.UIElements;

public class BulletArea : MonoBehaviour, IBulletBehavior
{
    [SerializeField] private bool fromPlayer;
    [SerializeField] public int damage;
    [SerializeField] private TimeSecuence timeSecuence;
    private cameraManager _camera;
    [SerializeField] public float time;
    [SerializeField] public Vector3 shootDirection;

    [SerializeField] float shootCShakeTime;
    [SerializeField] float shootCShakeRange;
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
        transform.position = new Vector3(transform.position.x, 0, transform.position.z);
    }
    public void setShootDirection(Vector3 _shootDirection, bool itsFromPlayer)
    {
        shootDirection = _shootDirection;
        Vector3 lookDirection = new Vector3(_shootDirection.x, 0f, _shootDirection.z);
        Quaternion targetRotation = Quaternion.LookRotation(lookDirection);
        transform.rotation = targetRotation * Quaternion.Euler(0f, -90f, 0f);
    }
    // Update is called once per frame
    void Update()
    {
        if (timeSecuence.play)
        {
         
            time += Time.deltaTime;
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
          

            if (fromPlayer && collision.gameObject.GetComponent<EnemyBase>() != null)
            {
                EnemyBase hit = collision.gameObject.GetComponent<EnemyBase>();
                hit.Damage(damage, collision.gameObject);
                Destroy(gameObject);
            }

            if (!fromPlayer && collision.gameObject.GetComponent<PlayerBase>() != null)
            {
                PlayerBase hit = collision.gameObject.GetComponent<PlayerBase>();
                hit.Damage(damage, collision.gameObject);
                Destroy(gameObject);
            }
        }
    }
}
