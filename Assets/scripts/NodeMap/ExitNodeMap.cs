using UnityEngine;
using UnityEngine.SceneManagement;

public class ExitNodeMap : MonoBehaviour
{
    public void ExitToMainMenu()
    {
        SceneManager.LoadScene("Title Screen");
    }
}
