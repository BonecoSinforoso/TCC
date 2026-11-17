using UnityEngine;
using UnityEngine.Serialization;

namespace TCC
{
    public class AnimationSound : MonoBehaviour
    {
        #region membros

        [FormerlySerializedAs("audioSource")]
        [SerializeField] private AudioSource _audioSource;
        [SerializeField] private AudioClip[] _footstepClips;

        #endregion

        #region metodos

        public void PlayAudioClip()
        {
            _audioSource.PlayOneShot(_footstepClips[Random.Range(0, _footstepClips.Length)]);
        }

        #endregion
    }
}
