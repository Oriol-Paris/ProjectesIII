using UnityEngine;

public class Enemy_Laser : MonoBehaviour
{
    [SerializeField] private bool fromPlayer;
    [SerializeField] public int damage;
    [SerializeField] private TimeSecuence timeSecuence;
    private cameraManager _camera;
    [SerializeField] public float time;
    [SerializeField] public Vector3 shootDirection;

    [SerializeField] float shootCShakeTime;
    [SerializeField] float shootCShakeRange;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (fromPlayer)
        {
            damage = FindAnyObjectByType<BulletCollection>().GetBullet(BulletType.GUN).damage;
        }
        else
        {
            damage = 1 * (int)PlayerPrefs.GetFloat("DifficultyMultiplier");
        }

        timeSecuence = FindFirstObjectByType<TimeSecuence>();
        _camera = FindAnyObjectByType<cameraManager>();
        StartCoroutine(_camera.Shake(shootCShakeTime, shootCShakeRange));
        transform.position = new Vector3(transform.position.x, 0, transform.position.z);
    }

    void Update()
    {
        if (timeSecuence.play)
        {

            time += Time.deltaTime;
        }



        if (time >= 5.0f)
        {

            Destroy(gameObject);
        }

    }

    // Update is called once per frame
    private void OnTriggerEnter(Collider collision)
    {
        if (collision != null)
        {


            if (fromPlayer && collision.gameObject.GetComponent<EnemyBase>() != null)
            {
                EnemyBase hit = collision.gameObject.GetComponent<EnemyBase>();
                hit.Damage(damage, collision.gameObject);
                Destroy(gameObject, 0.2f);
            }

            if (!fromPlayer && collision.gameObject.GetComponent<PlayerBase>() != null)
            {
                PlayerBase hit = collision.gameObject.GetComponent<PlayerBase>();
                hit.Damage(damage, collision.gameObject);
                Destroy(gameObject, 0.2f);
            }
        }
    }
}
