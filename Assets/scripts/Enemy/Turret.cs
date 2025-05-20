using System.Collections;
using UnityEngine;

public class EnemyTurret : MonoBehaviour
{
    public enum ShootDirection { Up, Down, Left, Right }

    [SerializeField] private GameObject bulletShot;
    [SerializeField] private AudioClip[] shootingClips;
    [SerializeField] private Animator fx;

    [SerializeField] private ShootDirection shootDirection = ShootDirection.Up;
    [SerializeField] private TimeSecuence targetPlayer; // Jugador al que sincronizar

    private bool hasShot = false;

    void Update()
    {
        if (!hasShot && targetPlayer != null && targetPlayer.isExecuting)
        {
            StartCoroutine(AttackCoroutine());
        }
    }

    private IEnumerator AttackCoroutine()
    {
        hasShot = true;

        // FX y animación
        Debug.Log("PIUM PIUM");
        if (fx != null) fx.SetTrigger("playFX");

        yield return new WaitForSeconds(0.3f);

        Shoot();

        if (fx != null) fx.ResetTrigger("playFX");

        // Espera a que termine el ciclo de ejecución y vuelva a empezar
        yield return new WaitUntil(() => !targetPlayer.isExecuting);
        yield return new WaitUntil(() => targetPlayer.isExecuting);

        hasShot = false;
    }

    private void Shoot()
    {
        Debug.Log("BANG");

        GameObject bullet = Instantiate(bulletShot, transform.position, Quaternion.identity);
        Vector3 shootDir = Vector3.zero;

        switch (shootDirection)
        {
            case ShootDirection.Up:
                shootDir = Vector3.forward;
                break;
            case ShootDirection.Down:
                shootDir = Vector3.back;
                break;
            case ShootDirection.Left:
                shootDir = Vector3.left;
                break;
            case ShootDirection.Right:
                shootDir = Vector3.right;
                break;
        }

        bullet.GetComponent<multiShoot>().setShootDirection(shootDir.normalized, false);

        if (shootingClips.Length > 0)
            SoundEffectsManager.instance.PlaySoundFXClip(shootingClips, transform, 1f);
    }
}
