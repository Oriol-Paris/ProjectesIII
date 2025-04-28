using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarManager : MonoBehaviour
{
    [SerializeField] private PlayerBase m_Player;
    [SerializeField] private Slider health;
    [SerializeField] private Slider actionPoints;
    [SerializeField] private TextMeshProUGUI healthNumber;
    [SerializeField] private TextMeshProUGUI actPtNumber;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //health.value = (float)(m_Player.health / m_Player.maxHealth);
        //actionPoints.value = (float)(m_Player.actionPoints / m_Player.maxActionPoints);
        //healthNumber.text = m_Player.health.ToString();
        //actPtNumber.text = m_Player.actionPoints.ToString();

    }

    // Update is called once per frame
    void Update()
    {
        health.value =(float)(m_Player.health/m_Player.maxHealth);
        actionPoints.value = (float)(m_Player.actionPoints / m_Player.maxActionPoints);
        healthNumber.text = "Health: "+ m_Player.health.ToString();
        actPtNumber.text = "ActPt: "+m_Player.actionPoints.ToString();
    }
}
