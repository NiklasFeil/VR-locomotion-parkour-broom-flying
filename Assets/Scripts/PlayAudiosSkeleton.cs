using UnityEngine;

public class PlayAudiosSkeleton : MonoBehaviour
{

    [SerializeField] private AudioSource skeletonAppearingAudio;
    [SerializeField] private AudioSource skeletonLeavingAudio;
    [SerializeField] private AudioSource skeletonGimmeAudio;

    public void PlayAppearingAudio()
    {
        skeletonAppearingAudio.Play(0);
    }
    public void PlayLeavingAudio()
    {
        skeletonLeavingAudio.Play(0);
    }

    public void PlayGimmeAudio()
    {
        skeletonGimmeAudio.Play(0);
    }
    
}
