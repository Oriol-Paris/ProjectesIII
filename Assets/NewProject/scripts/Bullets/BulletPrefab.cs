using System.Collections.Generic;
using UnityEngine;

public abstract class BulletPrefab : MonoBehaviour
{
    public PlayerData playerData;
    public bool isHit;
    [SerializeField] public int range;
    [SerializeField] public float speed;
    [SerializeField] public int damage;
    [SerializeField] public bool isFromPlayer;
    public Vector3 velocity;
    private bool isPaused = false;
    private Vector3 pausedPosition;
    protected Vector3 originalDirection;
    protected Vector3 originalTargetPosition;

    private OG_MovementByMouse movementScript;

    private void Start()
    {
        playerData = FindAnyObjectByType<PlayerData>();
        movementScript = FindAnyObjectByType<OG_MovementByMouse>();
        if (movementScript != null)
        {
            movementScript.RegisterBullet(this);
        }
        
        
    }

   

    public void SetFromPlayer(bool val) { isFromPlayer = val; }
    public abstract void Shoot(Vector3 direction);
    public void SetVelocity(float newSpeed)
    {
        speed = newSpeed;
    }
    public int GetRange() { return range; }

    public void Pause()
    {
        isPaused = true;
        pausedPosition = transform.position;
    }

    public void Resume()
    {
        isPaused = false;
        transform.position = pausedPosition;
        Shoot(originalDirection); // Resume shooting towards the original direction
    }

    protected bool IsPaused()
    {
        return isPaused;
    }

    protected void DestroyBullet()
    {
        if (movementScript != null)
        {
            movementScript.UnregisterBullet(this);
        }
        Destroy(gameObject);
    }

    public Vector3 GetOriginalDirection() { return originalDirection; }
}
