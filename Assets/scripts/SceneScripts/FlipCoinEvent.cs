using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Settings;
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
                eventText.dialogueLines = "FlipCoin_Intro";
                eventText.StartDialogue();
                break;

            case eventState.DECISION:
                eventText.dialogueLines = "FlipCoin_Choose";
                eventText.StartDialogue();
                buttonCanvas.SetActive(false);
                StartCoroutine(EnableButtonsWhenReady());
                break;

            case eventState.OUTCOME:
                buttonCanvas.SetActive(false);

                prizeSide = (Random.value > 0.5f) ? coinSide.HEADS : coinSide.TAILS;

                string resultText = LocalizationSettings.StringDatabase
                    .GetLocalizedString("StringLocation", "FlipCoin_Result", new[] { prizeSide.ToString() });

                if (playerSide == prizeSide)
                {
                    ReceivePrize();
                    resultText += "\n" + LocalizationSettings.StringDatabase.GetLocalizedString("StringLocation", "FlipCoin_Win");
                }
                else
                {
                    playerData.exp /= 2;
                    resultText += "\n" + LocalizationSettings.StringDatabase.GetLocalizedString("StringLocation", "FlipCoin_Lose");
                }

                resultText += $"\nCurrent XP: {playerData.exp}";

                eventText.textComponent.text = ""; // Limpieza por seguridad
                StartCoroutine(TypeLineExternalText(resultText));
                exitCanvas.enabled = true;
                break;

        }
    }

    IEnumerator TypeLineExternalText(string fullText)
    {
        eventText.textFullyDisplayed = false;
        eventText.SetIsTyping(true);
        eventText.textComponent.text = "";

        foreach (char c in fullText)
        {
            eventText.textComponent.text += c;
            yield return new WaitForSeconds(eventText.textSpeed);
        }

        eventText.SetIsTyping(false);
        eventText.textFullyDisplayed = true;
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
