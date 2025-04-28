using UnityEngine;

public interface IBulletBehavior
{
    void setShootDirection(Vector3 direction, bool isFromPlayer);
}