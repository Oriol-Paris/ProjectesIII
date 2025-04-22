using TMPro;
using UnityEngine;

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
        if (Input.GetMouseButtonUp(0))
        {
            if (currentState == eventState.INTRODUCTION && eventText.textFullyDisplayed)
            {
                currentState = eventState.DECISION;
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
                eventText.dialogueLines = "YOU FIND A MAN WHO PROPOSES TO FLIP A COIN";
                eventText.StartDialogue();
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
                resultText += playerSide == prizeSide ? "YOU WIN THE PRIZE!" : "YOU LOSE!";
                eventText.dialogueLines = resultText;
                eventText.StartDialogue();
                exitCanvas.enabled = true;
                break;
        }
    }

    System.Collections.IEnumerator EnableButtonsWhenReady()
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
}
