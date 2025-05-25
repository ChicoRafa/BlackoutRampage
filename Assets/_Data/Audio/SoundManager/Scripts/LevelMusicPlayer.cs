using UnityEngine;

public class LevelMusicPlayer : MonoBehaviour
{
    [Header("SFX")]
    [SerializeField] private SoundManagerSO soundManagerSO;
    [SerializeField] private AudioCueSO levelMusicCue;
    [SerializeField] private string audioClipId;
    [SerializeField] private bool loop = true;

    private void Start()
    {
        soundManagerSO.PlayMusic(levelMusicCue, audioClipId, loop);
    }
}
