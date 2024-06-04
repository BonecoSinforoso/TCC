using UnityEngine;

public class Script_AnimationEvent : MonoBehaviour
{
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip[] ac_footstep;

    public void AudioClip_Play()
    {
        audioSource.PlayOneShot(ac_footstep[Random.Range(0, ac_footstep.Length)]);
    }
}