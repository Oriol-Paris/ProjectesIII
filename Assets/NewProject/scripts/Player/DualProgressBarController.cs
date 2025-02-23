using UnityEngine;
using UnityEngine.UI;

public class DualProgressBarController : MonoBehaviour
{
    public Slider tProgressBar;
    public Slider timerProgressBar;
    //private OG_MovementByMouse movementScript;

    void Start()
    {
        if (tProgressBar == null || timerProgressBar == null)
        {
            Debug.LogError("Progress bars are not assigned.");
            return;
        }

        // Find the OG_MovementByMouse script in the scene
       // movementScript = FindObjectOfType<OG_MovementByMouse>();
    }

    void Update()
    {
       /* if (movementScript != null)
        {
            // Update the t progress bar based on the t value
            float tProgress = Mathf.Clamp01(movementScript.t);
            tProgressBar.value = tProgress;

            // Update the timer progress bar based on the timer value
            float timerProgress = Mathf.Clamp01(movementScript.timer / movementScript.movementTimeLimit);
            timerProgressBar.value = timerProgress;
        }*/
    }
}