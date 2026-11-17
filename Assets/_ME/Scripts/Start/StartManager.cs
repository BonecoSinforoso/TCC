using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace TCC
{
    public class StartManager : MonoBehaviour
    {
        #region membros

        public static StartManager Instance { get; private set; }

        [SerializeField] private CanvasGroup _creditsCg;
        [SerializeField] private CanvasGroup _tutorialCg;
        [SerializeField] private TextMeshProUGUI[] _backgroundTexts;
        [SerializeField] private float _backgroundChangeCooldown;
        [SerializeField] private TextMeshProUGUI _versionText;

        [SerializeField] private Image _tutorialImage;
        [SerializeField] private TextMeshProUGUI _tutorialText;

        [SerializeField] private int _gameId;
        [SerializeField] private int _infoId;
        [SerializeField] private Sprite[] _infinitySprites;
        [SerializeField] private string[] _infinityTexts;
        [SerializeField] private Sprite[] _flappySprites;
        [SerializeField] private string[] _flappyTexts;
        [SerializeField] private Sprite[] _pongSprites;
        [SerializeField] private string[] _pongTexts;

        #endregion

        #region metodos

        private void Start()
        {
            Application.targetFrameRate = 60;

            Time.timeScale = 1;

            _versionText.text = Application.version;

            Invoke(nameof(SetBackgroundTexts), _backgroundChangeCooldown);
        }

        // chamado por botao
        public void ChangeScene() => SceneManager.LoadScene(Scenes.Menu);

        // chamado por botao
        public void ToggleCreditsGroup()
        {
            _creditsCg.alpha = _creditsCg.alpha == 0 ? 1 : 0;
            _creditsCg.blocksRaycasts = _creditsCg.alpha == 1;
            _creditsCg.interactable = _creditsCg.alpha == 1;
        }

        // chamado por botao
        public void ToggleTutorialGroup()
        {
            _tutorialCg.alpha = _tutorialCg.alpha == 0 ? 1 : 0;
            _tutorialCg.blocksRaycasts = _tutorialCg.alpha == 1;
            _tutorialCg.interactable = _tutorialCg.alpha == 1;
        }

        // chamado por botao
        public void QuitGame() => Application.Quit();

        private void SetBackgroundTexts()
        {
            foreach (TextMeshProUGUI backgroundText in _backgroundTexts)
            {
                string text = "";

                for (int i = 0; i < 20; i++)
                {
                    text += Random.Range(0, 2).ToString();
                }

                backgroundText.text = text;
            }

            Invoke(nameof(SetBackgroundTexts), _backgroundChangeCooldown);
        }

        public void SetGameId(int value)
        {
            _gameId = value;
            _infoId = 0;

            SetTutorialImage();
            SetTutorialText();
        }

        public void SetInfoId(int value)
        {
            _infoId += value;

            if (_infoId < 0) _infoId = 2;
            if (_infoId > 2) _infoId = 0;

            SetTutorialImage();
            SetTutorialText();
        }

        private void SetTutorialImage()
        {
            switch (_gameId)
            {
                case 0:
                    _tutorialImage.sprite = _infinitySprites[_infoId];
                    break;
                case 1:
                    _tutorialImage.sprite = _flappySprites[_infoId];
                    break;
                case 2:
                    _tutorialImage.sprite = _pongSprites[_infoId];
                    break;
            }
        }

        private void SetTutorialText()
        {
            switch (_gameId)
            {
                case 0:
                    _tutorialText.text = _infinityTexts[_infoId];
                    break;
                case 1:
                    _tutorialText.text = _flappyTexts[_infoId];
                    break;
                case 2:
                    _tutorialText.text = _pongTexts[_infoId];
                    break;
            }
        }

        #endregion
    }
}
