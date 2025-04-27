using UnityEngine;
using System.Collections;

public class AnimateMapTile : MonoBehaviour
{
    float speed = 0.25f;
    Vector3 basePos;
    Vector3 liftedPos;

    bool isAnimating = false;
    Coroutine hoverLoop;

    float hoverY = 0.3f;
    float hoverSpeed = 4f;

    public bool wasVisited = false;

    void Start()
    {
        basePos = transform.position;
        liftedPos = basePos + new Vector3(0, 0.5f, 0);
    }

    public void AnimateTile()
    {
        if (!isAnimating)
            StartCoroutine(Animate());
    }

    IEnumerator Animate()
    {
        isAnimating = true;
        float elapsed = 0f;
        float duration = 1f / speed;

        while (elapsed < duration)
        {
            float t = elapsed / duration;
            float easedT = EaseOutElastic(t);
            transform.position = Vector3.LerpUnclamped(basePos, liftedPos, easedT);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = liftedPos;
        isAnimating = false;

        if (hoverLoop == null)
            hoverLoop = StartCoroutine(Hover());
    }

    IEnumerator Hover()
    {
        float t = 0f;
        Vector3 origin = transform.position;

        while (true)
        {
            float yOffset = Mathf.Sin(t * 2f) * 0.05f;
            transform.position = origin + new Vector3(0, yOffset, 0);
            t += Time.deltaTime;
            yield return null;
        }
    }

    Coroutine mouseHoverAnim;

    void OnMouseEnter()
    {
        if(!wasVisited)
        {
            if (mouseHoverAnim != null)
                StopCoroutine(mouseHoverAnim);
            mouseHoverAnim = StartCoroutine(HoverMouse(true));
        }
    }

    void OnMouseExit()
    {
        if(!wasVisited)
        {
            if (mouseHoverAnim != null)
                StopCoroutine(mouseHoverAnim);
            mouseHoverAnim = StartCoroutine(HoverMouse(false));
        }
    }

    IEnumerator HoverMouse(bool enter)
    {
        Vector3 startPos = transform.position;
        Vector3 endPos = basePos + new Vector3(0, enter ? hoverY : 0f, 0);

        float t = 0f;
        float duration = 1f / hoverSpeed;

        while (t < duration)
        {
            transform.position = Vector3.Lerp(startPos, endPos, t / duration);
            t += Time.deltaTime;
            yield return null;
        }

        transform.position = endPos;
    }

    float EaseOutElastic(float t)
    {
        float c4 = (2 * Mathf.PI) / 3;

        return t == 0 ? 0 : t == 1 ? 1 : Mathf.Pow(2, -10 * t) * Mathf.Sin((t * 10 - 0.75f) * c4) + 1;
    }
}
