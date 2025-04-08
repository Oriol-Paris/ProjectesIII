using System.Collections;
using UnityEngine;

public class AnimateMapTile : MonoBehaviour
{
    float speed = 0.1f;
    Vector3 originalPos;
    Vector3 targetPos;

    bool isAnimating = false;
    bool isHovering = false;

    float hoverAmplitude = 0.05f;
    float hoverFrequency = 0.05f;

    private void Start()
    {
        originalPos = transform.position;
        targetPos = originalPos + new Vector3(0, 0.5f, 0);
    }

    public void AnimateTile()
    {
        if (!isAnimating && !isHovering)
            StartCoroutine(Animate());
    }

    private IEnumerator Animate()
    {
        yield return new WaitForSeconds(1f);

        isAnimating = true;
        float elapsed = 0f;
        float duration = 1f / speed;

        while (elapsed < duration)
        {
            float t = elapsed / duration;
            float easedT = EaseOutElastic(t);
            transform.position = Vector3.LerpUnclamped(originalPos, targetPos, easedT);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = targetPos;
        isAnimating = false;

        StartCoroutine(Hover());
    }

    private IEnumerator Hover()
    {
        isHovering = true;
        float t = 0f;
        Vector3 basePos = transform.position;

        while (true) // loop infinito, puedes parar esto si quieres más adelante
        {
            float yOffset = Mathf.Sin(t * hoverFrequency) * hoverAmplitude;
            transform.position = basePos + new Vector3(0, yOffset, 0);
            t += Time.deltaTime;
            yield return null;
        }
    }

    float EaseOutElastic(float t)
    {
        float c4 = (2 * Mathf.PI) / 3;

        return t == 0 ? 0 : t == 1 ? 1 : Mathf.Pow(2, -10 * t) * Mathf.Sin((t * 10 - 0.75f) * c4) + 1;
    }
}