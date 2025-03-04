using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI; // Importante para usar NavMesh
using UnityEngine.Rendering;
using UnityEngine.UIElements;

public class EnemyMovement : MonoBehaviour
{
    #region VARIABLES

    [SerializeField] private TimeSecuence Player;
    private Vector3 PlayerPos;
    private EnemyBase enemyStats;
    [SerializeField] private float velocity;
    [SerializeField] private float range;
    private NavMeshAgent agent; // Referencia al NavMeshAgent

    #endregion

    public enum ActionEnum { MOVE, SHOOT, HEAL, MELEE, NOTHING };

    void Start()
    {
        Player = FindAnyObjectByType<TimeSecuence>();
        enemyStats = GetComponent<EnemyBase>();
        agent = GetComponent<NavMeshAgent>(); // Obtener el NavMeshAgent
        agent.speed = velocity * FindAnyObjectByType<CombatManager>().enemyStatMultiplier;
        range *= FindAnyObjectByType<CombatManager>().enemyStatMultiplier;
        agent.updateRotation = false;
    }

    void Update()
    {
        
        if (enemyStats.isAlive)
        {
            PlayerPos = Player.transform.position;
            float distanceToPlayer = Vector3.Distance(transform.position, PlayerPos);

            // Voltear sprite dependiendo de la posición del jugador
            GetComponent<SpriteRenderer>().flipX = PlayerPos.x < transform.position.x;

            if (Player.GetIsExecuting() || Player.GetComponent<PlayerBase>().GetInAction())
            {
                agent.isStopped = false;
                Debug.Log("MEEEP");
                GetComponent<Animator>().SetBool("isMoving", true);
                agent.SetDestination(PlayerPos); // Usar NavMeshAgent para moverse hacia el jugador
            }
            else
            {
                GetComponent<Animator>().SetBool("isMoving", false);
                agent.isStopped = true;
            }
        }
    }

    IEnumerator AttackCoroutine()
    {
        GetComponent<Animator>().SetTrigger("attack");
        yield return new WaitForSeconds(1);
    }

    public void Attack()
    {
        StartCoroutine(AttackCoroutine());
    }
}
