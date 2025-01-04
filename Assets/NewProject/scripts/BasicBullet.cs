using UnityEngine;

public class BasicBullet : MonoBehaviour
{

    [SerializeField]
    private float bulletVelocity = 5f;
    

    // Update is called once per frame
    void Update()
    {
        transform.position += transform.up * bulletVelocity * Time.deltaTime;
    }
}
