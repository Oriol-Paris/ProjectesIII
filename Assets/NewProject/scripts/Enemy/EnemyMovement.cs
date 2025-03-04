using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UIElements;

public class EnemyMovement : MonoBehaviour
{
    #region VARIABLES

    //[SerializeField]private MovementByMouse Player;
    [SerializeField]private TimeSecuence Player;
    private Vector3 PlayerPos;
    float moveTime;
    EnemyBase enemyStats;
    [SerializeField] private float velocity;
    [SerializeField] private float range;

    #endregion

    public enum ActionEnum { MOVE, SHOOT, HEAL, MELEE, NOTHING };

    void Start()
    {
        Player = FindAnyObjectByType<TimeSecuence>();
        enemyStats = GetComponent<EnemyBase>();

        velocity = velocity * FindAnyObjectByType<CombatManager>().enemyStatMultiplier;
        range = range * FindAnyObjectByType<CombatManager>().enemyStatMultiplier;
    }

    void Update()
    {
        moveTime = Time.deltaTime * velocity;
       

        if (enemyStats.isAlive)
        {
            if(PlayerPos.x < this.GetComponent<Rigidbody>().position.x)
                this.GetComponent<SpriteRenderer>().flipX = true;
            else
                this.GetComponent<SpriteRenderer>().flipX = false;


            if (Player.GetIsExecuting() || Player.GetComponent<PlayerBase>().GetInAction())
            {
                Debug.Log("MEEEP");
                this.GetComponent<Animator>().SetBool("isMoving", true);
                PlayerPos = Player.transform.position;
                transform.position = Vector3.MoveTowards(transform.position, PlayerPos, moveTime); //Bad usage of moveTime, using moveTime as distance when it's actually a velocity (check line 34)
            }
            else
            {
                this.GetComponent<Animator>().SetBool("isMoving", false);
            }
        }
    }

    IEnumerator AttackCoroutine()
    {
        this.GetComponent<Animator>().SetTrigger("attack");

        yield return new WaitForSeconds(1);
    }

    public void Attack()
    {
        StartCoroutine(AttackCoroutine());
    }
}
