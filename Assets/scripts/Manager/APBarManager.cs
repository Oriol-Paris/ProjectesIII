using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Rendering.HableCurve;

public class APBarManager : MonoBehaviour
{
    public GameObject APBarPrefab;
    private PlayerBase player;
    [SerializeField]private List<GameObject> apBars = new List<GameObject>();
    private int currentAP = 0;

    [Header("Animation Settings")]
    [SerializeField] private float fadeDuration = 0.5f;
    [SerializeField] private float minAlpha = 0.3f;
    [SerializeField] private float maxAlpha = 1f;

    void Start()
    {
        InitBar();
    }

    public void InitBar()
    {
        player = GetComponentInParent<PlayerBase>();
        for (int i = 0; i < player.playerData.maxActionPoints; i++)
        {
            GameObject apBar = Instantiate(APBarPrefab, transform);
            apBar.transform.Find("Fill").GetComponent<Image>().color = Color.blue;
            apBar.transform.localScale = new Vector3(0.006f, 0.006f, 1f);
            apBars.Add(apBar);
        }

        currentAP = (int)player.playerData.maxActionPoints - 1;
    }

    public void ResetBar()
    {
        foreach (var item in apBars)
        {
            Destroy(item);
        }
        apBars.Clear();

        InitBar();
    }


    public void AnimateLastAP()
    {
        StartCoroutine(AnimateBar(apBars[(int)currentAP]));
        currentAP--;
    }
    public void AnimateAPCost()
    {
        int cost = player.GetAction().m_cost;

        for (int i = 0; i < cost; i++)
        {
            int index = (int)currentAP - i;
            if (index >= 0 && index < apBars.Count)
            {
                StartCoroutine(AnimateBar(apBars[index]));
            }
        }

        currentAP -= cost;
        if (currentAP < -1) currentAP = -1;
    }

    public void DestroyUsedAP()
    {
        for (int i = (int)player.playerData.maxActionPoints - 1; i > currentAP; i--)
        {
            Destroy(apBars[i]);
            apBars.Remove(apBars[i]);
        }
    }

    IEnumerator AnimateBar(GameObject obj)
    {
        Image fillImg = obj.transform.Find("Fill").GetComponent<Image>();
        Image borderImg = obj.transform.Find("Border").GetComponent<Image>();
        if (fillImg == null || borderImg == null) yield break;

        List<Image> images = new List<Image> { fillImg, borderImg };

        while (true)
        {
            float phase = Mathf.PingPong(Time.time / fadeDuration, 1f);

            float alpha = Mathf.Lerp(minAlpha, maxAlpha, phase);

            foreach (var img in images)
            {
                if (img != null)
                {
                    Color c = img.color;
                    c.a = alpha;
                    img.color = c;
                }
            }

            yield return null; // Continuar en el siguiente frame
        }
    }
}