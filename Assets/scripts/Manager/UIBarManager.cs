using System;
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

        ResetHealthbar();
    }

    public void ResetHealthbar()
    {
        
        for (int i = 0; i < player.health; i++)
        {
            GameObject segment = Instantiate(healthBarPrefab, transform);
            segment.transform.Find("Fill").GetComponent<Image>().color = Color.red;
            segment.transform.localPosition = startPos + new Vector3(i * xOffset, 0, 0);
            segment.transform.localScale = scale;
            healthBarSegments.Add(segment);
        }
    }

    public void UpdateHealthBar(int damageTaken)
    {
        for(int i = 0; i < damageTaken; i++)
        {
            Destroy(this.transform.GetChild(healthBarSegments.Count - 1).gameObject);
            healthBarSegments.RemoveAt(healthBarSegments.Count - 1);
            
        }
    }
    public void AddSlot(int healingAmount)
    {
        for (int i = 0; i < healingAmount; i++)
        {
            Debug.Log("HEALED");
            GameObject segment = Instantiate(healthBarPrefab, transform);
            segment.transform.Find("Fill").GetComponent<Image>().color = Color.red;
            segment.transform.localPosition = startPos + new Vector3(healthBarSegments.Count * xOffset, 0, 0);
            segment.transform.localScale = scale;
            healthBarSegments.Add(segment);

        }
    }
}