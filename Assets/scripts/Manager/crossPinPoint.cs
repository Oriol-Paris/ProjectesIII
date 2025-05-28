using System.Collections;
using UnityEngine;

public class crossPinPoint : MonoBehaviour
{
    Animator animator;
    public bool crossedPinPoint;
    private void Start()
    {
        animator = GetComponent<Animator>();
        animator.SetBool("Start", true);
        crossedPinPoint = false;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")||other.CompareTag("Bullet"))
        {
            //animator.SetBool("Start", false);
            if(crossedPinPoint==false) {
                animator.SetBool("PointCrossed", true);
                crossedPinPoint = true;
            }
            StartCoroutine(WaitForAnimationAndDestroy());

        }
    }
    private IEnumerator WaitForAnimationAndDestroy()
    {
        yield return new WaitForSeconds(1f);
        GetComponent<SpriteRenderer>().sprite = null;
        GetComponent<SpriteRenderer>().enabled = false;
    }
}
