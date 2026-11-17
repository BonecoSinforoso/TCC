using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.SceneManagement;

namespace TCC
{
    public class MenuManager : MonoBehaviour
    {
        #region membros

        [FormerlySerializedAs("cenaNome")]
        [SerializeField] private string[] _sceneNames;
        [FormerlySerializedAs("txt_flapPontos")]
        [SerializeField] private TextMeshProUGUI _flapScoreText;
        private int _score = 0;

        #endregion

        #region metodos

        private void Start()
        {
            Invoke(nameof(SetScore), 0.1f);
        }

        public void ChangeScene(int value)
        {
            SceneManager.LoadScene(_sceneNames[value]);
        }

        private void SetScore()
        {
            _score++;

            SetFlapScoreText();
            Invoke(nameof(SetScore), 0.1f);
        }

        private void SetFlapScoreText()
        {
            _flapScoreText.text = _score.ToString();
        }

        public void LoadStartScene()
        {
            SceneManager.LoadScene(Scenes.Start);
        }

        #endregion
    }
}
