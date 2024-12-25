using System.Collections.Generic;
using UnityEngine;

public class TimeSecuence : MonoBehaviour
{
    [SerializeField]
    private float actualTime;
    float totalTime = 3;

    public GameObject player;

    private List<string> actions = new List<string>();

    private Dictionary<string, float> actionCosts = new Dictionary<string, float>
    {
       
        { "shoot", 0.75f },
        { "pick_up", 1.0f }
    };

    void Start()
    {
        actualTime = totalTime;
    }

   
    void Update()
    {

        if (actualTime > 0)
        {
            if (Input.GetKeyDown(KeyCode.Space)) AddAction("shoot");
            if (Input.GetKeyDown(KeyCode.E)) AddAction("pick_up");
        }


        if (Input.GetKeyDown(KeyCode.Return))
        {
            PassTurm();
        }
    }

    void AddAction(string action)
    {
        if (actualTime >= actionCosts[action])
        {
            actions.Add(action);
            actualTime -= actionCosts[action];
          
          
        }
        else
        {
            Debug.Log("No hay tiempo suficiente para esta acción.");
        }
    }

    void ExecuteActions()
    {
        foreach (string action in actions)
        {
            switch (action)
            {
                
                case "shoot":
                    Debug.Log("¡Disparo!");
                    break;
                case "pick_up":
                    Debug.Log("Objeto recogido");
                    break;
            }
        }
        actions.Clear();
    }


    void PassTurm()
    {
        ExecuteActions();
        actualTime = totalTime;
    }
}
