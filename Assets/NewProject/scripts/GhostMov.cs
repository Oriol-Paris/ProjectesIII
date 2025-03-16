using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostMov : MonoBehaviour
{
    GameObject player;
    ControlListMovment playerScript;
    TimeSecuence timeSecuence;
    public List<PlayerBase.ActionEnum> _actions = new List<PlayerBase.ActionEnum>();
    public List<Tuple<Vector3, Vector3, Vector3>> _MovList = new List<Tuple<Vector3, Vector3, Vector3>>();
    public List<float> _timeConsum = new List<float>();
    [SerializeField] private float maxDistance = 20f;
    [SerializeField] private float velocity = 1f;
    [SerializeField] private float tiemp = 0f;

    



    public float t;

    void Start()
    {
        player = GameObject.Find("Player");
        ControlListMovment playerScript = player.GetComponent<ControlListMovment>();
        _MovList = playerScript.MovList;
        _timeConsum = playerScript.timeConsum;
        TimeSecuence timeSecuence = player.GetComponent<TimeSecuence>();
        _actions = timeSecuence.actions;
        StartCoroutine(ExecuteActions());
    }


    IEnumerator ExecuteActions()
    {
        int movCount = 0;
        for (int i = 0; i < _actions.Count; i++)
        {
            PlayerBase.ActionEnum action = _actions[i];
            switch (action)
            {
                case PlayerBase.ActionEnum.SHOOT:

                    this.GetComponent<Animator>().SetTrigger("attack");
                    yield return new WaitForSeconds(0.75f);
                    movCount++;

                    break;
                case PlayerBase.ActionEnum.MOVE:


                    while (t < 1f) // Wait for the movement to finish
                    {

                      UpdateMovement(movCount);
                        yield return null; // Wait for a frame
                    }
                    StopMovment();
                    movCount++;
                    break;
                    // Add other cases for different actions if needed
            }
        }

        Destroy(this.gameObject);
    }

    public void StopMovment()
    {
        t = 0f;
     

    }
    public void UpdateMovement(int movCount)
    {


        List<Tuple<Vector3, Vector3, Vector3>> MovList = _MovList;
        var firstItem = MovList[movCount];
        Vector3 _playerPosition = firstItem.Item1;
        Vector3 _controlPoint = firstItem.Item2;
        Vector3 _positionDesired = firstItem.Item3;


       

        float duration = _timeConsum[movCount]; // Tiempo deseado para el movimiento
        float fixedDeltaTime = Time.deltaTime / duration;



        t += velocity * fixedDeltaTime;

        t = Mathf.Clamp(t, 0f, 1f);


        Vector3 newPosition = Directorio.BezierCurve(t, _playerPosition, _controlPoint, _positionDesired);
        newPosition.y = 0;
        transform.position = newPosition;


        tiemp += Time.deltaTime;
    }
}
