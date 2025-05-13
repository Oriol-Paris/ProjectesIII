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
   
    [SerializeField]private NavMeshAgent agent; // Referencia al NavMeshAgent
    [SerializeField]private GameObject attack; 
    private SpriteRenderer spriteRenderer; // Referencia al SpriteRenderer
    private bool isResting = false; // Indica si el enemigo está en descanso
    [SerializeField] private float stopDistance = 1.0f; // Distance to stop from the player

    [SerializeField] AudioClip[] attackClips;

    #endregion

    public enum ActionEnum { MOVE, SHOOT, HEAL, MELEE, NOTHING };

    void Start()
    {
        Player = FindAnyObjectByType<TimeSecuence>();
        enemyStats = GetComponent<EnemyBase>();
        agent = GetComponent<NavMeshAgent>(); // Obtener el NavMeshAgent
        agent.speed = velocity * PlayerPrefs.GetFloat("DifficultyMultiplier");
        range *= PlayerPrefs.GetFloat("DifficultyMultiplier");
        agent.updateRotation = false;
       
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

           

            if (Player.GetIsExecuting() )
            {
                if (distanceToPlayer <= range)
                {
                    Attack();
                }
                agent.isStopped = false;
                GetComponent<Animator>().SetBool("isMoving", true);
                agent.speed = 2;
                // Calculate the destination with an offset
                Vector3 directionToPlayer = (PlayerPos - transform.position).normalized;
                Vector3 destination = PlayerPos - directionToPlayer * stopDistance;
                agent.SetDestination(PlayerPos); // Usar NavMeshAgent para moverse hacia el jugador
            }
            else
            {
              
                GetComponent<Animator>().SetBool("isMoving", false);
                agent.isStopped = true;
                agent.speed = 0;
                agent.velocity = Vector3.zero;
               
            }
        }
        else
        {
            agent.speed = 0;    
        }
    }

    IEnumerator AttackCoroutine()
    {
        isResting = true; // Comienza el descanso
        Debug.Log("Attacking");
        GetComponent<Animator>().SetTrigger("attack");

        GameObject _attack = Instantiate(attack, transform.position, Quaternion.identity, transform);
        yield return new WaitForSeconds(0.5f);

        Destroy(_attack);

        yield return new WaitForSeconds(restTime);

        isResting = false;
    }

    public void Attack()
    {
        if (!isResting) // Evita que ataque si aún está descansando
        {
            StartCoroutine(AttackCoroutine());
            SoundEffectsManager.instance.PlaySoundFXClip(attackClips, transform, 1f);
        }
    }


   
}
