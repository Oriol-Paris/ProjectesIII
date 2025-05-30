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
    [SerializeField] private GameObject buttonCanvas;
    [SerializeField] private Canvas exitCanvas;
    [SerializeField] private PlayerData playerData;

    [SerializeField] private AudioSource audioSource;

    // Start is called before the first frame update
    void Awake()
    {
        currentState = eventState.INTRODUCTION;
        buttonCanvas.SetActive(false);
        exitCanvas.enabled = false;
        UpdateCurrentState();
    }

    void Update()
    {
        
        if (Input.GetMouseButtonUp(0)&& eventText.textFullyDisplayed)
        {
            if (currentState == eventState.INTRODUCTION)
            {
                audioSource.Play();
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
                buttonCanvas.SetActive(false);
                StartCoroutine(EnableButtonsWhenReady());
                break;

            case eventState.OUTCOME:
                buttonCanvas.SetActive(false);

                string resultText = $"YOU FLIP A COIN AND IT LANDS ON {prizeSide}.\n";
                if (playerSide == prizeSide)
                {
                    ReceivePrize();
                    resultText += "YOU WIN! YOUR XP DOUBLES.\n";
                }
                else
                {
                    playerData.exp /= 2;
                    resultText += "YOU LOSE! YOUR XP IS HALVED.\n";
                }
                resultText += "Current XP: " + playerData.exp;
                eventText.dialogueLines = resultText;
                eventText.StartDialogue();
                exitCanvas.enabled = true;
                break;
        }
    }
    void ReceivePrize()
    {
        int original = playerData.exp;
        playerData.exp *= 2;
    }

    

    IEnumerator EnableButtonsWhenReady()
    {
        while (!eventText.textFullyDisplayed)
        {
            yield return null;
        }
        buttonCanvas.SetActive(true);

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
        audioSource.Play();
        prizeSide = (Random.value > 0.5f) ? coinSide.HEADS : coinSide.TAILS;
        currentState = eventState.OUTCOME;
        UpdateCurrentState();
    }

    public void ExitScene()
    {
        audioSource.Play();
        DontDestroyOnLoad(audioSource);
        PlayerPrefs.SetString("LastLevelCleared", SceneManager.GetActiveScene().name);

        SceneManager.LoadScene(PlayerPrefs.GetString("EneteredMap"));
    }
}
