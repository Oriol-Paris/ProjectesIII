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
    [SerializeField] private float restTime = 1.5f; // Tiempo de descanso tras un ataque
    [SerializeField] private Collider hitBox;
    [SerializeField]private NavMeshAgent agent; // Referencia al NavMeshAgent
    private SpriteRenderer spriteRenderer; // Referencia al SpriteRenderer
    private bool isResting = false; // Indica si el enemigo está en descanso
    [SerializeField] private float stopDistance = 1.0f; // Distance to stop from the player

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
        hitBox.enabled = false;
        spriteRenderer = GetComponent<SpriteRenderer>(); // Obtener el componente SpriteRenderer
    }

    void Update()
    {
        if (enemyStats.isAlive) // No moverse si está descansando
        {
            PlayerPos = Player.transform.position;
            float distanceToPlayer = Vector3.Distance(transform.position, PlayerPos);

            // Voltear sprite dependiendo de la posición del jugador
            spriteRenderer.flipX = PlayerPos.x < transform.position.x;

            // Ajustar la posición de la hitBox según la dirección
            Vector3 hitBoxPosition = hitBox.transform.localPosition;
            hitBoxPosition.x = spriteRenderer.flipX ? -Mathf.Abs(hitBoxPosition.x) : Mathf.Abs(hitBoxPosition.x);
            hitBox.transform.localPosition = hitBoxPosition;

            if (Player.GetIsExecuting() || Player.GetComponent<PlayerBase>().GetInAction())
            {
                if (distanceToPlayer <= range)
                {
                    Attack();
                }
                agent.isStopped = false;
                GetComponent<Animator>().SetBool("isMoving", true);

                // Calculate the destination with an offset
                Vector3 directionToPlayer = (PlayerPos - transform.position).normalized;
                Vector3 destination = PlayerPos - directionToPlayer * stopDistance;
                agent.SetDestination(destination); // Usar NavMeshAgent para moverse hacia el jugador
            }
            else
            {
              
                GetComponent<Animator>().SetBool("isMoving", false);
                agent.isStopped = true;
                agent.velocity = Vector3.zero;
                hitBox.GetComponent<MeleeEnemyDamage>().hasDealtDamage = false;
            }
        }
    }

    IEnumerator AttackCoroutine()
    {
        isResting = true; // Comienza el descanso
        Debug.Log("Attacking");
        GetComponent<Animator>().SetTrigger("attack");

        yield return new WaitForSeconds(0.5f);
      // Tiempo de animación del ataque
        hitBox.enabled = true;

        yield return new WaitForSeconds(1f);
       // Duración del ataque
        hitBox.enabled = false;

        yield return new WaitForSeconds(restTime); // Tiempo de descanso después del ataque
      
        isResting = false; // El enemigo puede volver a moverse y atacar
    }

    public void Attack()
    {
        if (!isResting) // Evita que ataque si aún está descansando
        {
            StartCoroutine(AttackCoroutine());
        }
    }
}
