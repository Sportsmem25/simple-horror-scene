using UnityEngine;

public class ChaseManager : MonoBehaviour
{
    public static ChaseManager Instance;

    public AudioSource ambientMusic;
    public AudioSource chaseAudio;
    public CameraShake cameraShake;
    public float shakeIntensity = 0.1f;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void StartChase()
    {
        ambientMusic.Stop();
        chaseAudio.Play();
        if (cameraShake) cameraShake.StartShake(shakeIntensity, 999f);
    }

    public void StopChase()
    {
        ambientMusic.Play();
        chaseAudio.Stop();
        if (cameraShake) cameraShake.StopShake();
    }
}
