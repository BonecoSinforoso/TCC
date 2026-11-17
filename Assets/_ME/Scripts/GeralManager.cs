using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace TCC
{
    public class GeralManager : MonoBehaviour
    {
        #region membros

        public static GeralManager Instance { get; private set; }

        [FormerlySerializedAs("cg_pausa")]
        [SerializeField] private CanvasGroup _pauseCg;

        [FormerlySerializedAs("obj_fb")]
        [SerializeField] private GameObject _resultObject;
        [SerializeField] private TextMeshProUGUI _resultText;
        [FormerlySerializedAs("color_fb")]
        [SerializeField] private Color[] _resultColors;

        private bool _paused = false;

        #endregion

        #region metodos

        private void Start()
        {
            Instance = this;

            Application.targetFrameRate = 60;
            Time.timeScale = 1;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.P)) //redundancia de aviao para evitar bugs
            {
                PauseGame();
            }
        }

        private void PauseGame()
        {
            if (_paused)
            {
                if (Time.timeScale == 0)
                {
                    _paused = false;
                    Time.timeScale = 1;

                    _pauseCg.alpha = 0;
                    _pauseCg.blocksRaycasts = false;
                    _pauseCg.interactable = false;
                }
            }
            else
            {
                if (Time.timeScale == 1)
                {
                    _paused = true;
                    Time.timeScale = 0;

                    _pauseCg.alpha = 1;
                    _pauseCg.blocksRaycasts = true;
                    _pauseCg.interactable = true;
                }
            }
        }

        public void LoadMenuScene() => SceneManager.LoadScene(Scenes.Menu);
        public void LoadStartScene() => SceneManager.LoadScene(Scenes.Start);

        public void ShowResult(int value)
        {
            _resultObject.SetActive(true);
            _resultObject.GetComponent<Image>().color = _resultColors[value];

            Time.timeScale = 0;

            _resultText.text = value == 0 ? "VOCÊ PERDEU!" : "VOCÊ GANHOU!";
        }

        #endregion
    }
}
