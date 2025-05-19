using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenu;
    public GameObject combatUI;

    private PlayerActionManager playerActionManager;

    private void Start()
    {
        pauseMenu.SetActive(false);
        playerActionManager = FindAnyObjectByType<PlayerActionManager>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ToggleActiveMenu();
        }
    }

    public void ToggleActiveMenu()
    {
        if(pauseMenu.activeInHierarchy)
        {
            Cursor.visible = false;
            if(playerActionManager != null)
            {
                playerActionManager.enabled = true;
            }
            pauseMenu.SetActive(false);
            if(combatUI != null)
            combatUI.SetActive(true);
        }
        else
        {
            Cursor.visible = true;
                        
            if (playerActionManager != null)
            {
                playerActionManager.enabled = false;
            }
            pauseMenu.SetActive(true);
            if(combatUI != null)
            combatUI.SetActive(false);
        }
    }

    public void ReturnToMainMenu()
    {
        SceneManager.LoadScene("Title Screen");
    }
}
