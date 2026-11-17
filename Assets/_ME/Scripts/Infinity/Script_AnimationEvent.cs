using UnityEngine;

public class Script_AnimationEvent : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip[] ac_footstep;

    public void AudioClip_Play()
    {
        audioSource.PlayOneShot(ac_footstep[Random.Range(0, ac_footstep.Length)]);
    }
}