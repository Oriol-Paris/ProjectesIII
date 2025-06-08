using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Components;

public class Dialogue : MonoBehaviour
{
    public TextMeshProUGUI textComponent;
    public string dialogueLines;
    public float textSpeed;
    public TutorialManager tutorialManager;
    public bool textFullyDisplayed;
    private int index;
    public bool IsTyping { get; private set; }

    private LocalizeStringEvent localizeEvent;

    void Awake()
    {
        localizeEvent = GetComponent<LocalizeStringEvent>();
    }

    public void StartDialogue()
    {
        index = 0;

        if (localizeEvent != null)
        {
            localizeEvent.StringReference.TableEntryReference = dialogueLines;
            localizeEvent.RefreshString();
        }

        StartCoroutine(TypeLine());
    }

    IEnumerator TypeLine()
    {
        textFullyDisplayed = false;
        IsTyping = true;

        // Esperamos a que se resuelva la localización
        var localized = localizeEvent.StringReference.GetLocalizedStringAsync();
        yield return localized;

        string fullText = localized.Result;
        textComponent.text = "";

        foreach (char c in fullText)
        {
            textComponent.text += c;
            yield return new WaitForSeconds(textSpeed);
        }

        textFullyDisplayed = true;
        IsTyping = false;
    }

    public void SetIsTyping(bool condition) => IsTyping = condition;
}
