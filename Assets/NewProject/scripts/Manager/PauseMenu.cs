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
            playerActionManager.enabled = true;
            pauseMenu.SetActive(false);
            combatUI.SetActive(true);
        }
        else
        {
            Cursor.visible = true;
            playerActionManager.enabled = false;
            pauseMenu.SetActive(true);
            combatUI.SetActive(false);
        }
    }

    public void ReturnToMainMenu()
    {
        SceneManager.LoadScene("Title Screen");
    }
}
