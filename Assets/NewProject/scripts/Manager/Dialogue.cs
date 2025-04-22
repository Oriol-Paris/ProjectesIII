using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class Dialogue : MonoBehaviour
{
    public TextMeshProUGUI textComponent;
    public string dialogueLines;
    public float textSpeed;
    public TutorialManager tutorialManager; // Add this line
    public bool textFullyDisplayed;
    private int index;

    void Start()
    {
        textComponent.text = string.Empty;
        StartDialogue();
    }

    void Update()
    {
        if (Input.GetMouseButtonUp(0))
        {
            if (textComponent.text == dialogueLines)
            {
                if(tutorialManager != null)
                {
                    tutorialManager.DisableCurrentPopup(); 
                }
                

            }
            else
            {
                StopAllCoroutines();
                textComponent.text = dialogueLines;
                textFullyDisplayed = true;
            }
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
        foreach (char c in dialogueLines.ToCharArray())
        {
            textComponent.text += c;
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
}
