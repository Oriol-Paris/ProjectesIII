using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

public class RandomEventSelector : MonoBehaviour
{
    public List<GameObject> events;

    private void Start()
    {
        if (events == null || events.Count == 0)
            return;

        int selectedIndex = Random.Range(0, events.Count);

        for (int i = 0; i < events.Count; i++)
        {
            if (events[i] != null)
                events[i].SetActive(i == selectedIndex);
        }
    }
}
