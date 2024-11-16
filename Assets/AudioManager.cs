using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private AudioClip backgroundMusicClip;
    private AudioSource backgroundMusicSource;

    private void Start()
    {
        // Initialize background music
        backgroundMusicSource = gameObject.AddComponent<AudioSource>();
        backgroundMusicSource.clip = backgroundMusicClip;
        backgroundMusicSource.loop = true;
        backgroundMusicSource.volume = 0.5f; // Adjust volume if needed
        backgroundMusicSource.Play();
    }
}
