using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Components;
using UnityEngine.Localization.Settings;

public class Dialogue : MonoBehaviour
{
    public TextMeshProUGUI textComponent;
    public string dialogueLines;
    public float textSpeed;
    public TutorialManager tutorialManager;
    public bool textFullyDisplayed;
    private int index;
    public bool IsTyping { get; private set; }

    public LocalizeStringEvent localizeEvent;
    
    void Awake()
    {
        localizeEvent = GetComponent<LocalizeStringEvent>();
    }

    void Start()
    {
        textComponent.text = string.Empty;
        StartDialogue();
    }

    public void StartDialogue()
    {
        index = 0;
        if (localizeEvent != null)
        {
            localizeEvent.StringReference.TableEntryReference = dialogueLines;
            localizeEvent.RefreshString(); // Fuerza la actualización inmediata
        }

        StartCoroutine(TypeLine());
    }

    IEnumerator TypeLine()
    {
        textFullyDisplayed = false;
        IsTyping = true;

        string fullText = textComponent.text;
        textComponent.text = string.Empty;

        foreach (char c in fullText)
        {
            textComponent.text += c;
            yield return new WaitForSeconds(textSpeed);
        }

        IsTyping = false;
        textFullyDisplayed = true;
    }

    public void SetIsTyping(bool condition)
    {
        IsTyping = condition;
    }

    

}
