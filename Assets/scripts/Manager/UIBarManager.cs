using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIBarManager : MonoBehaviour
{
    [SerializeField] private GameObject healthBarPrefab;
    private List<GameObject> healthBarSegments = new List<GameObject>();
    private PlayerData player;

    public Vector3 startPos;
    public Vector3 scale;
    public float xOffset;

    void Start()
    {
        player = FindAnyObjectByType<PlayerBase>().playerData;

        InitializeHealthbar();
        UpdateHealthbar();
    }

    public void InitializeHealthbar()
    {
        // Elimina cualquier segmento anterior
        foreach (var segment in healthBarSegments)
        {
            Destroy(segment);
        }
        healthBarSegments.Clear();

        // Crea tantos segmentos como la vida máxima
        for (int i = 0; i < player.maxHealth; i++)
        {
            GameObject segment = Instantiate(healthBarPrefab, transform);
            segment.transform.localPosition = startPos + new Vector3(i * xOffset, 0, 0);
            segment.transform.localScale = scale;
            healthBarSegments.Add(segment);
        }
    }

    public void UpdateHealthbar()
    {
        for (int i = 0; i < healthBarSegments.Count; i++)
        {
            Image fill = healthBarSegments[i].transform.Find("Fill").GetComponent<Image>();
            fill.color = i < player.health ? Color.red : Color.gray;
        }
    }
}
