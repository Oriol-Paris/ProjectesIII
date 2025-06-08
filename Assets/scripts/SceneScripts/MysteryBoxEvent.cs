using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Localization.Settings;

public class MysteryBoxEvent : MonoBehaviour
{
    [SerializeField] private Dialogue eventText;
    [SerializeField] private GameObject buttonCanvas;
    [SerializeField] private Canvas exitCanvas;
    [SerializeField] private PlayerData playerData;
    [SerializeField] private AudioSource audioSource;

    public enum eventState { INTRODUCTION, DECISION, OUTCOME }
    private eventState currentState;

    private string localizedOutcome = "";

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
                eventText.dialogueLines = "MysteryBox_Intro";
                eventText.StartDialogue();
                break;

            case eventState.DECISION:
                eventText.dialogueLines = "MysteryBox_Choose";
                eventText.StartDialogue();
                buttonCanvas.SetActive(false);
                StartCoroutine(EnableButtonsWhenReady());
                break;

            case eventState.OUTCOME:
                buttonCanvas.SetActive(false);
                StartCoroutine(TypeOutcomeText());
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
            localizedOutcome = LocalizationSettings.StringDatabase
                .GetLocalizedString("StringLocation", "MysteryBox_Buy_Success", new[] { reward.ToString() });
        }
        else
        {
            localizedOutcome = LocalizationSettings.StringDatabase
                .GetLocalizedString("StringLocation", "MysteryBox_Buy_Fail");
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
            localizedOutcome = LocalizationSettings.StringDatabase
                .GetLocalizedString("StringLocation", "MysteryBox_Steal_Hurt", new[] { reward.ToString() });
        }
        else
        {
            localizedOutcome = LocalizationSettings.StringDatabase
                .GetLocalizedString("StringLocation", "MysteryBox_Steal_Success", new[] { reward.ToString() });
        }

        currentState = eventState.OUTCOME;
        UpdateCurrentState();
    }

    public void LeaveBox()
    {
        localizedOutcome = LocalizationSettings.StringDatabase
            .GetLocalizedString("StringLocation", "MysteryBox_Leave");

        currentState = eventState.OUTCOME;
        UpdateCurrentState();
    }

    IEnumerator TypeOutcomeText()
    {
        string finalText = localizedOutcome + $"\nCurrent XP: {playerData.exp}";
        eventText.textComponent.text = "";
        eventText.textFullyDisplayed = false;
        eventText.SetIsTyping(true);

        foreach (char c in finalText)
        {
            eventText.textComponent.text += c;
            yield return new WaitForSeconds(eventText.textSpeed);
        }

        eventText.textFullyDisplayed = true;
        eventText.SetIsTyping(false);
    }

    public void ExitScene()
    {
        audioSource.Play();
        DontDestroyOnLoad(audioSource);
        PlayerPrefs.SetString("LastLevelCleared", SceneManager.GetActiveScene().name);
        SceneManager.LoadScene(PlayerPrefs.GetString("EneteredMap"));
    }
}
