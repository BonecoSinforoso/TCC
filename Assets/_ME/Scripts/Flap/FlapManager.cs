using UnityEngine;
using UnityEngine.Serialization;

namespace TCC
{
    public class FlapManager : MonoBehaviour
    {
        #region membros

        public static FlapManager Instance { get; private set; }

        [FormerlySerializedAs("obj_player")]
        [SerializeField] private GameObject _playerObject;
        [SerializeField] private GameObject _iaObject;

        [FormerlySerializedAs("obj_playerClone")]
        [SerializeField] private GameObject _playerCloneObject;
        [SerializeField] private GameObject _iaCloneObject;

        [FormerlySerializedAs("movel")]
        public bool Movable;
        [FormerlySerializedAs("pontuacaoParaVencer")]
        public int ScoreToWin;
        [FormerlySerializedAs("ac")]
        [SerializeField] private AudioClip[] _audioClips;

        private AudioSource _audioSource;

        #endregion

        #region metodos

        private void Start()
        {
            Instance = this;

            _audioSource = GetComponent<AudioSource>();
        }

        private void Update()
        {
            _playerCloneObject.transform.SetPositionAndRotation(_playerObject.transform.position, _playerObject.transform.rotation);

            _iaCloneObject.transform.SetPositionAndRotation(_iaObject.transform.position, _iaObject.transform.rotation);
        }

        public void CallShowResult(int value)
        {
            GeralManager.Instance.ShowResult(value);
        }

        public void PlayAudio(int index)
        {
            _audioSource.PlayOneShot(_audioClips[index]);
        }

        #endregion
    }
}
