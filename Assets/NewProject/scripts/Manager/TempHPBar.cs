using UnityEngine;
using UnityEngine.UI;

public class TempHPBar : MonoBehaviour
{
    private PlayerBase player;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = FindAnyObjectByType<PlayerBase>();
    }

    // Update is called once per frame
    void Update()
    {
        this.GetComponent<Image>().fillAmount = (float)(player.health / player.maxHealth);
    }
}
