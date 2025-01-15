using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIBarManager : MonoBehaviour
{
    [SerializeField] private Image healthBar;
    [SerializeField] private Image actionPointBar;
    private PlayerData player;
    private float lerpSpeed;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = FindAnyObjectByType<PlayerBase>().playerData;
    }

    // Update is called once per frame
    void Update()
    {
        lerpSpeed = 3f * Time.deltaTime;

        healthBar.fillAmount = Mathf.Lerp(healthBar.fillAmount, player.health / (player.maxHealth * 2.0f), lerpSpeed);
        healthBar.color = Color.Lerp(Color.red, Color.green, (player.health / player.maxHealth));

        actionPointBar.fillAmount = Mathf.Lerp(actionPointBar.fillAmount, player.actionPoints / (player.maxActionPoints * 2.0f), lerpSpeed);
    }
}
