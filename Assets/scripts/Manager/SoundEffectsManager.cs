using UnityEngine;

public class SoundEffectsManager : MonoBehaviour
{
    public static SoundEffectsManager instance;

    [SerializeField] private AudioSource soundFXObject;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
    }


    public void PlaySoundFXClip(AudioClip[] audioclip, Transform spawnTransform, float volume)
    {
        int rand = Random.Range(0,audioclip.Length-1);
        AudioSource source = Instantiate(soundFXObject, spawnTransform);

        source.clip = audioclip[rand];
        source.volume = volume;
        source.Play();

        float clipLength = source.clip.length;

        Destroy(source.gameObject,clipLength);
    }
}
