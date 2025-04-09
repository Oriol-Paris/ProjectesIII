using System.Collections;
using UnityEngine;

public class crossPinPoint : MonoBehaviour
{
    Animator animator;
    private void Start()
    {
        animator = GetComponent<Animator>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            animator.SetBool("PointCrossed", true);
            StartCoroutine(WaitForAnimationAndDestroy());
        }
    }
    private IEnumerator WaitForAnimationAndDestroy()
    {
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        while (stateInfo.normalizedTime < 1.0f || !stateInfo.IsName("PointCrossed"))
        {
            yield return null;
            stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        }
        gameObject.SetActive(false);
    }
}
