using UnityEngine;
using System.Collections.Generic;
using System;

public class shootPlayer : MonoBehaviour
{
    public ControlLiniarRender controlLiniarRender;
    public ControlListMovment controlListMovment;
    public TimeSecuence timeSecuence;
    public Animator fx;

    public GameObject bulletPrefab;

    
    

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
        List<Tuple<Vector3, Vector3, Vector3>> MovList = controlListMovment.MovList;
        var firstItem = MovList[Count];
        Vector3 _playerPosition = firstItem.Item1;
        Vector3 _controlPoint = firstItem.Item2;

        Vector3 shootDirection = (_controlPoint - _playerPosition).normalized;

        GameObject bullet = Instantiate(bulletPrefab, _playerPosition, Quaternion.identity);

        
        bullet.GetComponent<DestroyBullet>().setShootDirection(shootDirection,true);

        

    }

}
