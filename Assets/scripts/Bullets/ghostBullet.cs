using UnityEngine;
using UnityEngine.UIElements;

public class ghostBullet : MonoBehaviour
{
    private Vector3 direction;
    private float time;

    [SerializeField] private Rigidbody rb;

    void Start()
    {

    }

    public void takedirection(Vector3 _direction, float _time)
    {
        direction = _direction;
        time = _time;
    }


    void Update()
    {
        rb.linearVelocity = direction * 3.0f;
        time += Time.deltaTime;

        if (time >= 7f)
        {
            Debug.Log("destoy");
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.CompareTag("envairoment"))
        {
            Destroy(gameObject);
        }
    }
}
