using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

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
                eventText.dialogueLines = "YOU ENCOUNTER A VILLAGE PLAGUED BY DISEASE...";
                
                break;

            case eventState.DECISION:
                eventText.dialogueLines = "DO YOU HELP THEM?";
                eventText.StartDialogue();
                buttonCanvas.SetActive(false);

                StartCoroutine(EnableButtonsWhenReady());
                break;

            case eventState.OUTCOME:
                buttonCanvas.SetActive(false);
                exitCanvas.enabled = true;
                for (int i = 0; i<playerData.availableActions.Count; i++)
                {
                    if (playerData.availableActions[i].action == PlayerBase.ActionEnum.HEAL)
                    {
                        hasHealingSkill = true;
                        break;
                    }
                    else
                    {
                        hasHealingSkill = false;
                    }
                }
                string outcome = hasHealingSkill
                    ? "YOU CURE THE VILLAGERS. THEY THANK YOU.\n+100 XP"
                    : "YOU LACK THE SKILL TO HELP.\nTHE VILLAGERS SUFFER.";
                if (hasHealingSkill)
                {
                    playerData.exp += 100;
                }
                eventText.dialogueLines = outcome + "\nCurrent XP: " + playerData.exp;
                
                eventText.StartDialogue();
                
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
