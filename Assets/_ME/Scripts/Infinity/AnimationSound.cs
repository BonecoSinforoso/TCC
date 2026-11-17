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

        // chamado por evento de animacao
        public void PlayAudioClip()
        {
            if (_footstepClips.Length == 0)
                return;

            _audioSource.PlayOneShot(_footstepClips[Random.Range(0, _footstepClips.Length)]);
        }

        #endregion
    }
}
