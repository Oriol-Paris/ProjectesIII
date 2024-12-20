using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UIElements;

public class EnemyMovement : MonoBehaviour
{
    #region VARIABLES

    //[SerializeField]private MovementByMouse Player;
    [SerializeField]private OG_MovementByMouse Player;
    private Vector3 PlayerPos;
    float moveTime;
    EnemyBase enemyStats;
    [SerializeField] private float velocity;
    [SerializeField] private float range;

    #endregion

    public enum ActionEnum { MOVE, SHOOT, HEAL, MELEE, NOTHING };

    void Start()
    {
        Player = FindAnyObjectByType<OG_MovementByMouse>();
        enemyStats = GetComponent<EnemyBase>();

        velocity = velocity * FindAnyObjectByType<CombatManager>().enemyStatMultiplier;
        range = range * FindAnyObjectByType<CombatManager>().enemyStatMultiplier;
    }

    void Update()
    {
        moveTime = Time.deltaTime * velocity;

        if (enemyStats.isAlive)
        {
            if(PlayerPos.x < this.GetComponent<Rigidbody2D>().position.x)
                this.GetComponent<SpriteRenderer>().flipX = true;
            else
                this.GetComponent<SpriteRenderer>().flipX = false;


            if ((Vector3.Distance(PlayerPos, transform.position) < range && Player.isMoving) || Player.GetComponent<PlayerBase>().GetInAction())
            {
                this.GetComponent<Animator>().SetBool("isMoving", true);
                PlayerPos = Player.GetPosition();
                transform.position = Vector3.MoveTowards(transform.position, PlayerPos, moveTime);
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
