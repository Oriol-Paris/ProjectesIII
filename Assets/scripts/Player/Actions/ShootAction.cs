using System.Collections.Generic;
using UnityEngine;

public class ShootAction : ActiveAction
{
    public Bullet bullet;
    public GameObject spawnedBullet;
    public GameObject bulletPrefab;
    public List<Vector3> offsets = new List<Vector3>
    {
        new Vector3(-0.5f, 0, 0),
        new Vector3(0, 0, 0),
        new Vector3(0.5f, 0, 0)
    };

    public override void Execute(PlayerBase player, Vector3 targetPosition)
    {
        if (bulletPrefab == null)
        {
            Debug.LogError("Bullet prefab is not set.");
            return;
        }

       

        Vector3 direction = (targetPosition - player.transform.position).normalized;

        if (bulletPrefab.GetComponent<Shotgun>() != null)
        {
            // Ensure offsets contains three different offsets for triple shot
            if (offsets.Count != 3)
            {
                Debug.LogError("Offsets list does not contain exactly three offsets for shotgun triple shot.");
                return;
            }

            // Loop through each offset and instantiate a bullet for Shotgun
            int bulletCount = 0;
            foreach (var offset in offsets)
            {
                GameObject shotgunBulletInstance = Instantiate(bulletPrefab, player.transform.position, Quaternion.identity);
                //Debug.Log("Shotgun bullet instantiated.");
                Shotgun shotgunBullet = shotgunBulletInstance.GetComponent<Shotgun>();
                if (shotgunBullet != null)
                {
                    shotgunBullet.Shoot(direction, offset);
                    //movementScript.RegisterBullet(shotgunBullet);
                    bulletCount++;
                }
                else
                {
                    Debug.LogError("Shotgun component not found on the instantiated bullet.");
                }
            }
            //Debug.Log($"Shotgun fired {bulletCount} bullets.");
        }
        else
        {
            // Instantiate the bullet at the player's position
            GameObject bulletInstance = Instantiate(bulletPrefab, player.transform.position, Quaternion.identity);
            // Determine the type of bullet and call the appropriate Shoot method
            bullet = bulletInstance.GetComponent<Bullet>();
            Debug.Log("Bullet instantiated at player's position.");

            
            if (bullet != null)
            {
                if (bullet is GunBullet)
                {
                    
                    bullet.Shoot(direction);
                    
                }
                else if (bullet is LaserBullet)
                {
                    bullet.Shoot(direction);
                    
                }
                else
                {
                    Debug.LogError("Unknown bullet type.");
                }
                if (bullet.isHit)
                {
                    
                }
            }
            else
            {
                Debug.LogError("BulletPrefab component not found on the instantiated bullet.");
            }
        }
    }
}