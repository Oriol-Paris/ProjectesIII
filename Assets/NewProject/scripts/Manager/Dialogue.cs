using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using System;

public class Dialogue : MonoBehaviour
{
    [SerializeField] OG_MovementByMouse player;
    public TextMeshProUGUI textComponent;
    public string[] intro;
    public string[] postWalk;
    public string[] attack;
    public string[] rest;
    public List<string[]> tutorialBoxes;
    public float textSpeed;

    private int index;
    private int globalIndex;

    public enum TutorialState { Intro, PostWalk, Attack, Rest, Completed }
    private TutorialState currentState;

    private bool hasMoved = false;
    private bool hasShot = false;
    private bool hasHealed = false;

    void Start()
    {
        tutorialBoxes = new List<string[]>();
        tutorialBoxes.Add(intro);
        tutorialBoxes.Add(postWalk);
        tutorialBoxes.Add(attack);
        tutorialBoxes.Add(rest);

        textComponent.text = string.Empty;
        currentState = TutorialState.Intro;
        StartDialogue();
    }

    void Update()
    {
        if (Input.GetMouseButtonUp(0))
        {
            if (textComponent.text == tutorialBoxes[globalIndex][index])
            {
                NextLine();
            }
            else
            {
                StopAllCoroutines();
                textComponent.text = tutorialBoxes[globalIndex][index];
            }
        }
    }

    void StartDialogue()
    {
        index = 0;
        StartCoroutine(TypeLine());
        player.enabled = false;
    }

    IEnumerator TypeLine()
    {
        foreach (char c in tutorialBoxes[globalIndex][index].ToCharArray())
        {
            textComponent.text += c;
            yield return new WaitForSeconds(textSpeed);
        }
    }

    void NextLine()
    {
        if (index < tutorialBoxes[globalIndex].Length - 1)
        {
            index++;
            textComponent.text = string.Empty;
            StartCoroutine(TypeLine());
        }
        else
        {
            gameObject.SetActive(false);
            player.enabled = true;
        }
    }

    public void ActionCompleted(PlayerBase.ActionEnum action)
    {
        switch (action)
        {
            case PlayerBase.ActionEnum.MOVE:
                if (currentState == TutorialState.Intro && !hasMoved)
                {
                    hasMoved = true;
                    currentState = TutorialState.PostWalk;
                }
                break;
            case PlayerBase.ActionEnum.SHOOT:
                if (currentState == TutorialState.PostWalk && !hasShot)
                {
                    hasShot = true;
                    currentState = TutorialState.Attack;
                }
                break;
            case PlayerBase.ActionEnum.HEAL:
                if (currentState == TutorialState.Attack && !hasHealed)
                {
                    hasHealed = true;
                    currentState = TutorialState.Rest;
                }
                break;
        }

        if (currentState != TutorialState.Completed)
        {
            globalIndex = (int)currentState;
            gameObject.SetActive(true);
            textComponent.text = string.Empty;
            StartDialogue();
        }
        else
        {
            Debug.Log("Tutorial completed!");
        }
    }
}
