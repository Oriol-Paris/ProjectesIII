using UnityEngine;
using System.Collections.Generic;
using System;

public class shootPlayer : MonoBehaviour
{
    public ControlLiniarRender controlLiniarRender;
    public ControlListMovment controlListMovment;
    public TimeSecuence timeSecuence;
    public Animator fx;
    private int internalIterator = 0;
    public List<GameObject> bulletPrefab;

    GameObject bullet;



    public void preShoot()
    {
        if (!timeSecuence.notacction)
        {
            controlLiniarRender.Disable(false);
            if (!Input.GetMouseButton(0))
            {


                controlLiniarRender.ControlLiniarRenderer();
                controlLiniarRender.UpdateLineRendererr();
            }
            if (Input.GetMouseButtonUp(0))
            {

                controlListMovment.AddMovement(controlLiniarRender, 0.75f, PlayerBase.ActionEnum.SHOOT);
                bulletPrefab.Add(GetComponent<PlayerBase>().GetAction().m_style.prefab);
                
            }
        }
        else
        {
            controlLiniarRender.Disable(true);
        }
    }

    public void UpdateShoot(int Count)
    {
        this.GetComponent<Animator>().SetTrigger("attack");
        fx.SetTrigger("playFX");
        SoundEffectsManager.instance.PlaySoundFXClip(GetComponent<PlayerActionManager>().shootClip, transform, 1f);
        List<Tuple<Vector3, Vector3, Vector3>> MovList = controlListMovment.MovList;
        var firstItem = MovList[Count];
        Vector3 _playerPosition = firstItem.Item1;
        Vector3 _controlPoint = firstItem.Item2;

        Vector3 shootDirection = (_controlPoint - _playerPosition).normalized;
        
        if (bulletPrefab[internalIterator] != null) { 

            bullet = Instantiate(bulletPrefab[internalIterator], _playerPosition, Quaternion.identity);
            internalIterator++;
        }
        if (bullet.GetComponent<DestroyBullet>() != null)
        {
            bullet.GetComponent<DestroyBullet>().setShootDirection(shootDirection, true);
            Debug.Log("bullet has gun");
        } else if(bullet.GetComponent<multiShoot>()!=null)
        {
            Debug.Log("Bullet has other gun");
            bullet.GetComponent<multiShoot>().setShootDirection(shootDirection, true);
        }else if(bullet.GetComponent<LaserBullet>()!=null)
        {
            bullet.GetComponent<LaserBullet>().setShootDirection(shootDirection);
        }

        

    }

    public void SetInternalIterator(int amount) { internalIterator = amount; }

}
