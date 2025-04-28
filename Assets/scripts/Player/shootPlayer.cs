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

                controlListMovment.AddMovement(controlLiniarRender,1.0f, 0.75f, PlayerBase.ActionEnum.SHOOT);
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

        GameObject bullet = Instantiate(bulletPrefab[internalIterator], _playerPosition, Quaternion.identity);

        internalIterator++;
        bullet.GetComponent<multiShoot>().setShootDirection(shootDirection, true);



    }

    public void ResetShotPlayer()
    {
       bulletPrefab.Clear();
        internalIterator = 0;
    }

}
