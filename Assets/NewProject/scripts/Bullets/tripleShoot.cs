using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public class tripleShoot : MonoBehaviour
{

    [SerializeField] private Vector3 shootDirection;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private GameObject enemyPrefab;

    [SerializeField] private float shotBetweenShots = 0.2f; 
    private float coneAngle = 15f;
    private int numbreBullet = 3;

    public void setShootDirection(Vector3 _shootDirection, bool itsFromPlayer)
    {
        shootDirection = _shootDirection;
        if (itsFromPlayer) { bulletPrefab = playerPrefab; }
        else { bulletPrefab = enemyPrefab; }

        StartCoroutine(Shoot());
    }

    IEnumerator Shoot()
    {
       
        for (int i = 0; i < numbreBullet ; i++)
        {
            float angleOffset = (i - (numbreBullet / 2)) * coneAngle;

            
            Vector3 rotatedDirection = Quaternion.Euler(0, angleOffset, 0) * shootDirection;

            GameObject bullet = Instantiate(bulletPrefab, this.transform.position, Quaternion.identity);

            bullet.GetComponent<DestroyBullet>().setShootDirection(rotatedDirection,true);
            yield return new WaitForSeconds(shotBetweenShots);
        }
       
    }


}
