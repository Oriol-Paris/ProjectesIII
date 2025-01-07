using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TimeSecuence : MonoBehaviour
{
    [SerializeField]
    public float actualTime;
    public float totalTime = 3;
    float rang = 10f;

    public Vector3 lastPosition;

    public MovPlayer movPlayer;

    public GameObject player;

    public shootPlayer shootPl;

    private List<string> actions = new List<string>();
    private List<Vector3> actionTargets = new List<Vector3>();

    private Dictionary<string, float> actionCosts = new Dictionary<string, float>
    {
        { "shoot", 0.75f },
        { "pick_up", 1.0f },
        { "move", 0.0f }
    };

    void Start()
    {
        actualTime = totalTime;
        lastPosition = transform.position;
    }

    void Update()
    {
        if (actualTime > 0)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Vector3 targetPosition = GetMouseTargetPosition();
                AddAction("shoot", targetPosition);
                shootPl.PreShoot(lastPosition);
            }

            movPlayer.PreStartMov();
        }

        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            PassTurn();
        }
    }

    public void AddAction(string action, Vector3 targetPosition)
    {
        actions.Add(action);
        actionTargets.Add(targetPosition);
    }

    IEnumerator ExecuteActions()
    {
        int movCount = 0;
        int shootCount = 0;

        for (int i = 0; i < actions.Count; i++)
        {
            string action = actions[i];
            Vector3 targetPosition = actionTargets[i];

            switch (action)
            {
                case "shoot":
                    Debug.Log("¡Disparo!");
                    shootPl.Shoot(targetPosition);
                    yield return new WaitForSeconds(0.75f);
                    shootCount++;
                    break;
                case "pick_up":
                    Debug.Log("Objeto recogido");
                    break;
                case "move":
                    movPlayer.StartMov();

                    while (movPlayer.t < 1f) // Espera a que termine el movimiento
                    {
                        movPlayer.UpdateMovement(movCount);
                        yield return null; // Espera un frame
                    }
                    movPlayer.StopMovment();
                    movCount++;
                    break;
            }
        }
        actions.Clear();
        actionTargets.Clear();
        movPlayer.finish();
    }

    void PassTurn()
    {
        Debug.Log("aaaaaaa");
        StartCoroutine(ExecuteActions());
        actualTime = totalTime;
    }

    private Vector3 GetMouseTargetPosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            return hit.point; // Posición 3D del objeto impactado
        }
        return Vector3.zero; // Si no impacta, retorna Vector3.zero
    }
}
