using UnityEngine;
using System.Collections.Generic;
using System;

public class shootPlayer : MonoBehaviour
{
    public ControlLiniarRender controlLiniarRender;
    public ControlListMovment controlListMovment;
    public Animator fx;

    public GameObject bulletPrefab;

    private float bulletSpeed = 0.5f;

    public void preShoot()
    {
        if (!Input.GetMouseButton(0))
        {

           
            controlLiniarRender.ControlLiniarRenderer();
            controlLiniarRender.UpdateLineRendererr();
        }
        if (Input.GetMouseButtonUp(0))
        {

            controlListMovment.AddMovement(controlLiniarRender,0.75f, PlayerBase.ActionEnum.SHOOT);

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

        Rigidbody rb = bullet.GetComponent<Rigidbody>();

        rb.linearVelocity = shootDirection * bulletSpeed;

    }

}
