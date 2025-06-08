using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Localization.Settings;

public class InfectedTownEvent : MonoBehaviour
{
    [SerializeField] private Dialogue eventText;
    [SerializeField] private GameObject buttonCanvas;
    [SerializeField] private Canvas exitCanvas;
    [SerializeField] private PlayerData playerData;
    [SerializeField] private AudioSource audioSource;

    public enum eventState { INTRODUCTION, DECISION, OUTCOME }
    private eventState currentState;
    private bool hasHealingSkill;
    private string localizedOutcome;

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
                eventText.dialogueLines = "InfectedTown_Intro";
                eventText.StartDialogue();
                break;

            case eventState.DECISION:
                eventText.dialogueLines = "InfectedTown_Choose";
                eventText.StartDialogue();
                buttonCanvas.SetActive(false);
                StartCoroutine(EnableButtonsWhenReady());
                break;

            case eventState.OUTCOME:
                buttonCanvas.SetActive(false);
                exitCanvas.enabled = true;

                // Verificar si el jugador tiene la habilidad HEAL
                hasHealingSkill = false;
                foreach (var action in playerData.availableActions)
                {
                    if (action.action == PlayerBase.ActionEnum.HEAL)
                    {
                        hasHealingSkill = true;
                        break;
                    }
                }

                if (hasHealingSkill)
                {
                    playerData.exp += 100;
                    localizedOutcome = LocalizationSettings.StringDatabase
                        .GetLocalizedString("StringLocation", "InfectedTown_Outcome_Heal");
                }
                else
                {
                    localizedOutcome = LocalizationSettings.StringDatabase
                        .GetLocalizedString("StringLocation", "InfectedTown_Outcome_NoHeal");
                }

                StartCoroutine(TypeOutcomeText());
                break;
        }
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

    IEnumerator EnableButtonsWhenReady()
    {
        while (!eventText.textFullyDisplayed)
        {
            yield return null;
        }
        buttonCanvas.SetActive(true);
    }

    public void HelpVillage()
    {
        currentState = eventState.OUTCOME;
        UpdateCurrentState();
    }

    public void LeaveVillage()
    {
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
