using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public AudioClip[] tracks;        // Tus canciones
    private AudioSource audioSource;
    private int currentTrackIndex = 0;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (tracks.Length > 0)
        {
            audioSource.clip = tracks[currentTrackIndex];
            audioSource.loop = true;
            audioSource.Play();
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            NextTrack();
        }
    }

    void NextTrack()
    {
        currentTrackIndex = (currentTrackIndex + 1) % tracks.Length;
        audioSource.clip = tracks[currentTrackIndex];
        audioSource.Play();
    }
}
