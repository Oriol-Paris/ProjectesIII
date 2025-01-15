using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIBarManager : MonoBehaviour
{
    [SerializeField] private Image healthBar;
    [SerializeField] private Image actionPointBar;
    [SerializeField] private Image actionPointBarBG;
    [SerializeField] private Image timeBar;
    private PlayerData player;
    private TimeSecuence time;
    private float lerpSpeed;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = FindAnyObjectByType<PlayerBase>().playerData;
        time = FindAnyObjectByType<MovPlayer>().timeSceuence;
    }

    // Update is called once per frame
    void Update()
    {
        lerpSpeed = 3f * Time.deltaTime;

        healthBar.fillAmount = Mathf.Lerp(healthBar.fillAmount, player.health / (player.maxHealth * 4.0f), lerpSpeed);
        healthBar.color = Color.Lerp(Color.red, Color.green, (player.health / player.maxHealth));

        actionPointBar.fillAmount = Mathf.Lerp(actionPointBar.fillAmount, time.actualTime / (time.totalTime * 4.0f), lerpSpeed);

        timeBar.fillAmount = Mathf.Clamp01(1.0f - FindAnyObjectByType<MovPlayer>().t);
    }
}