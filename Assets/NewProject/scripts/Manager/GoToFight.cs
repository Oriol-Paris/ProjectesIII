using UnityEngine;
using UnityEngine.SceneManagement;

public class GoToFight : MonoBehaviour
{
    public void NextRound()
    {
        SceneManager.LoadScene("Node Map");
    }
}
