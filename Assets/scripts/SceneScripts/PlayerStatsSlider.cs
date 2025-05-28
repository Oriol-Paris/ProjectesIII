using System.Collections;
using UnityEngine;

public class PlayerStatsSlider : MonoBehaviour
{
    Vector3 startPos;
    public Vector3 endPos;
    public float speed = 1f;
    public GameObject blurBG;
    public GameObject exitText;

    public AudioSource enterSfxSource;
    public AudioSource exitSfxSource;

    private void Start()
    {
        startPos = transform.position;
        blurBG.SetActive(false);
        exitText.SetActive(false);
    }

    public void SlideOut()
    {
        enterSfxSource.Play();
        StartCoroutine(LerpMovement(startPos, endPos, true));
    }

    public void SlideIn()
    {
        exitSfxSource.Play();
        StartCoroutine(LerpMovement(endPos, startPos, false));
        blurBG.SetActive(false);
        exitText.SetActive(false);
    }

    IEnumerator LerpMovement(Vector3 start, Vector3 end, bool outMov)
    {
        float elapsedTime = 0f;
        while (elapsedTime < speed)
        {
            transform.position = Vector3.Lerp(start, end, (elapsedTime / speed));
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        transform.position = end;

        if(outMov)
        {
            blurBG.SetActive(true);
            exitText.SetActive(true);
        }
    }
}