using UnityEngine;
using UnityEngine.UI;

public class ProgressBarController : MonoBehaviour
{
    public Slider progressBar;
    private OG_MovementByMouse movementScript;

    void Start()
    {
        if (progressBar == null)
        {
            progressBar = GetComponent<Slider>();
        }

        // Find the OG_MovementByMouse script in the scene
        movementScript = FindObjectOfType<OG_MovementByMouse>();
    }

    void Update()
    {
        if (movementScript != null)
        {
            // Update the progress bar based on the timer
            float progress = Mathf.Clamp01(movementScript.t / movementScript.movementTimeLimit);
            progressBar.value = progress;
        }
    }
}