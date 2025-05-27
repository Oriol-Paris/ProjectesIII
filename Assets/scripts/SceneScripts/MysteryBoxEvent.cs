using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MysteryBoxEvent : MonoBehaviour
{
    [SerializeField] private Dialogue eventText;
    [SerializeField] private GameObject buttonCanvas;
    [SerializeField] private Canvas exitCanvas;
    [SerializeField] private PlayerData playerData;
    [SerializeField] private AudioSource audioSource;

    public enum eventState { INTRODUCTION, DECISION, OUTCOME }
    private eventState currentState;

    private string outcomeMessage = "";

    void Awake()
    {
        currentState = eventState.INTRODUCTION;
        buttonCanvas.SetActive(false);
        exitCanvas.enabled = false;
        UpdateCurrentState();
    }

    void Update()
    {
        if (Input.GetMouseButtonUp(0) && eventText.textFullyDisplayed)
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
                eventText.dialogueLines = "YOU FIND A MYSTERIOUS BOX FOR SALE...";
                break;

            case eventState.DECISION:
                eventText.dialogueLines = "WHAT DO YOU DO?";
                eventText.StartDialogue();
                buttonCanvas.SetActive(false);
                StartCoroutine(EnableButtonsWhenReady());
                break;

            case eventState.OUTCOME:
                buttonCanvas.SetActive(false);
                eventText.dialogueLines = outcomeMessage + "\nCurrent XP: " + playerData.exp;
                eventText.StartDialogue();
                exitCanvas.enabled = true;
                break;
        }
    }

    IEnumerator EnableButtonsWhenReady()
    {
        while (!eventText.textFullyDisplayed)
        {
            yield return null;
        }
        buttonCanvas.SetActive(true);
    }

    public void BuyBox()
    {
        if (playerData.exp >= 15)
        {
            playerData.exp -= 15;
            int reward = Random.Range(25, 46);
            playerData.exp += reward;
            outcomeMessage = $"YOU BUY THE BOX. INSIDE YOU FIND XP!\n+{reward} XP";
        }
        else
        {
            outcomeMessage = "YOU DON'T HAVE ENOUGH XP TO BUY THE BOX.";
        }
        currentState = eventState.OUTCOME;
        UpdateCurrentState();
    }

    public void StealBox()
    {
        bool takesDamage = Random.value < 0.5f;
        int reward = Random.Range(10, 31);
        playerData.exp += reward;
        if (takesDamage)
        {
            playerData.health -= 1;
            outcomeMessage = $"YOU STEAL THE BOX AND GET HURT.\n-1 HP, +{reward} XP";
        }
        else
        {
            outcomeMessage = $"YOU SUCCESSFULLY STEAL THE BOX!\n+{reward} XP";
        }
        currentState = eventState.OUTCOME;
        UpdateCurrentState();
    }

    public void LeaveBox()
    {
        outcomeMessage = "YOU DECIDE TO WALK AWAY FROM THE BOX.";
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
