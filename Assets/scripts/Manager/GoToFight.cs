using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GoToFight : MonoBehaviour
{
    [SerializeField] private AudioSource enterSfxSource;

    public void NextRound()
    {
        enterSfxSource.Play();
        DontDestroyOnLoad(enterSfxSource);
        PlayerPrefs.SetString("LastLevelCleared", SceneManager.GetActiveScene().name);
        SceneManager.LoadScene(PlayerPrefs.GetString("EneteredMap"));
    }
}