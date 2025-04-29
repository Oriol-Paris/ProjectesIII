using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

public class UIBarManager : MonoBehaviour
{
    [SerializeField] private GameObject healthBarPrefab;
    private List<GameObject> healthBarSegments = new List<GameObject>();
    private PlayerData player;

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
            segment.transform.localPosition = new Vector3(-841 + i * 90, 475, 0);
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
}