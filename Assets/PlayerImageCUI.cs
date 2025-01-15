using UnityEngine;
using UnityEngine.UI;

public class PlayerImageCUI : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        this.GetComponent<Image>().color = Color.white;
        this.GetComponent<Image>().overrideSprite = FindAnyObjectByType<PlayerBase>().GetComponent<SpriteRenderer>().sprite;
    }
}
