using UnityEngine;
using UnityEngine.Video;

public class PlayVhsEffect : MonoBehaviour
{
    [SerializeField] private VHSPostProcessEffect vhsEffect;
    [SerializeField] private VideoPlayer videoPlayer;
    private ControllerNPC controllerNPC;

    private void Start()
    {
        controllerNPC = FindObjectOfType<ControllerNPC>();
    }

    private void Update()
    {
        if (controllerNPC.isChasing)
        {
            vhsEffect.enabled = true;
            videoPlayer.enabled = true;
        }
    }
}