using System.Collections;
using UnityEngine;

public class PlayerStatsSlider : MonoBehaviour
{
    Vector3 startPos;
    public Vector3 endPos;
    public float speed = 1f;

    private void Start()
    {
        startPos = transform.position;
    }

    public void SlideOut()
    {
        StartCoroutine(LerpMovement(startPos, endPos));
    }

    public void SlideIn()
    {
        StartCoroutine(LerpMovement(endPos, startPos));
    }

    IEnumerator LerpMovement(Vector3 start, Vector3 end)
    {
        float elapsedTime = 0f;
        while (elapsedTime < speed)
        {
            transform.position = Vector3.Lerp(start, end, (elapsedTime / speed));
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        transform.position = end;
    }
}