using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FlipCoinEvent : MonoBehaviour
{
    [SerializeField] private Dialogue eventText;
    private enum eventState { INTRODUCTION, DECISION, OUTCOME }
    private enum coinSide { HEADS, TAILS }

    private coinSide playerSide;
    private coinSide prizeSide;

    [SerializeField] private eventState currentState;
    [SerializeField] private Canvas buttonCanvas;
    [SerializeField] private Canvas exitCanvas;
    [SerializeField] private PlayerData playerData;

    // Start is called before the first frame update
    void Start()
    {
        currentState = eventState.INTRODUCTION;
        buttonCanvas.enabled = false;
        exitCanvas.enabled = false;
        UpdateCurrentState();
    }

    void Update()
    {
        
        if (Input.GetMouseButtonUp(0)&& eventText.textFullyDisplayed)
        {
            if (currentState == eventState.INTRODUCTION)
            {
                currentState = eventState.DECISION;
                eventText.SetIsTyping(true);
                eventText.textFullyDisplayed = false;
                UpdateCurrentState();
            }
        }
    }


    void UpdateCurrentState()
    {
        switch (currentState)
        {
            case eventState.INTRODUCTION:
                eventText.StartDialogue();
                eventText.dialogueLines = "YOU FIND A MAN WHO PROPOSES TO FLIP A COIN";
                
                break;

            case eventState.DECISION:
                
                eventText.dialogueLines = "HEADS OR TAILS?";
                eventText.StartDialogue();
                buttonCanvas.enabled = false;
                StartCoroutine(EnableButtonsWhenReady());
                break;

            case eventState.OUTCOME:
                buttonCanvas.enabled = false;
               
                string resultText = $"YOU FLIP A COIN AND IT LANDS ON {prizeSide}.\n";
               
                if(playerSide == prizeSide)
                {
                    ReceivePrize();
                }
                resultText += playerSide == prizeSide ? "YOU WIN " + 25 + " EXP\nCurrent XP:" + playerData.exp : "YOU LOSE!";
                eventText.dialogueLines = resultText;
                eventText.StartDialogue();
                exitCanvas.enabled = true;
                break;
        }
    }
    void ReceivePrize()
    {
        playerData.exp += 25;
    }
    IEnumerator EnableButtonsWhenReady()
    {
        while (!eventText.textFullyDisplayed)
        {
            yield return null;
        }
        buttonCanvas.enabled = true;
    }

    public void Heads()
    {
        playerSide = coinSide.HEADS;
        FlipCoin();
    }

    public void Tails()
    {
        playerSide = coinSide.TAILS;
        FlipCoin();
    }

    void FlipCoin()
    {
        prizeSide = (Random.value > 0.5f) ? coinSide.HEADS : coinSide.TAILS;
        currentState = eventState.OUTCOME;
        UpdateCurrentState();
    }

    public void ExitScene()
    {
        SceneManager.LoadScene("Node Map");
    }
}
