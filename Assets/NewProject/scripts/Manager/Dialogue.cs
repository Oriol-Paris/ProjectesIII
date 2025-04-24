using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using System;

public class Dialogue : MonoBehaviour
{
    public TextMeshProUGUI textComponent;
    public string dialogueLines;
    public float textSpeed;
    public TutorialManager tutorialManager; // Add this line
    public bool textFullyDisplayed;
    private int index;
    public bool IsTyping { get; private set; }

    void Start()
    {
        textComponent.text = string.Empty;
        StartDialogue();
    }

    void Update()
    {

        if (textComponent.text == dialogueLines)
        {
            
            textFullyDisplayed = true;
        }

    }


    public void StartDialogue()
    {
        index = 0;
        StartCoroutine(TypeLine());
    }

    IEnumerator TypeLine()
    {
        textFullyDisplayed = false;
        textComponent.text = string.Empty;
        foreach (char c in dialogueLines.ToCharArray())
        {
            IsTyping = true;
            textComponent.text += c;
            textFullyDisplayed = false;
            yield return new WaitForSeconds(textSpeed);
        }
    }


    void NextLine()
    {
        if (index < dialogueLines.Length - 1)
        {
            index++;
            textComponent.text = string.Empty;
            StartCoroutine(TypeLine());
        }
        else
        {
            gameObject.SetActive(false);
            tutorialManager.DisableCurrentPopup(); // Add this line
        }
    }
    public void SetIsTyping(bool condition) { IsTyping = condition; }
}
