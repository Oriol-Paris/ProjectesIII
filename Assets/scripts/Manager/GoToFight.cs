using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GoToFight : MonoBehaviour
{
    [SerializeField] private AudioSource enterSfxSource;

    public void NextRound()
    {
        StartCoroutine(NextRoundwSfx());
    }

    private IEnumerator NextRoundwSfx()
    {
        enterSfxSource.Play();
        yield return new WaitForSeconds(enterSfxSource.clip.length);

        PlayerPrefs.SetString("LastLevelCleared", SceneManager.GetActiveScene().name);
        SceneManager.LoadScene("Node Map 2.0");
    }
}
