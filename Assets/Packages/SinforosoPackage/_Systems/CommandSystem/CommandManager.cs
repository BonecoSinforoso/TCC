using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SinforosoPackage
{
    public class CommandManager : MonoBehaviour
    {
        #region membros

        public static CommandManager Instance { get; private set; }

        #region serializadas

        [SerializeField] private CanvasGroup _commandCg;
        [SerializeField] private Image _iconImg;

        [Header("textos")]
        [SerializeField] private TextMeshProUGUI _charTxt;
        [SerializeField] private TextMeshProUGUI _descriptionTxt;

        [Header("sprites")]
        [SerializeField] private Sprite _mouseSpr;
        [SerializeField] private Sprite _keySpr;

        #endregion

        #region nao serializadas

        private Action _action;
        private bool _unique = false;
        private KeyCode _currentKeyCode;
        private bool _isMouse = false;

        #endregion

        #endregion

        #region metodos

        private void Start()
        {
            if (Instance == null)
                Instance = this;
        }

        private void Update()
        {
            if (_isMouse)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    StartAction();
                }
            }
            else
            {
                if (Input.GetKeyDown(_currentKeyCode))
                {
                    StartAction();
                }
            }
        }

        public void Call_StartAction() => StartAction();

        [SinforosoButton]
        private void StartAction()
        {
            if (_action != null)
            {
                _action();

                if (_unique)
                {
                    ClearAction();
                }
            }
        }

        public void SetAction(Action action, string @char, string description, bool unique, float delay = 0.1f)
        {
            StartCoroutine(SetAction_Coroutine(delay, action, @char, description, unique));
        }

        public void SetAction(Action action, string @char, string description, bool unique)
        {
            _action = action;
            _unique = unique;
            _isMouse = @char == "Left";

            if (@char == "Left")
            {
                _iconImg.sprite = _mouseSpr;

                Color color = _charTxt.color;
                color.a = 0f;

                _charTxt.color = color;
            }
            else
            {
                _currentKeyCode = (KeyCode)Enum.Parse(typeof(KeyCode), @char);

                _iconImg.sprite = _keySpr;

                Color color = _charTxt.color;
                color.a = 1f;

                _charTxt.color = color;
                _charTxt.text = @char;
            }

            _descriptionTxt.text = description;

            Util.SetCg(_commandCg, true);
        }

        private IEnumerator SetAction_Coroutine(float delay, Action action, string @char, string description, bool unique)
        {
            yield return new WaitForSeconds(delay);

            SetAction(action, @char, description, unique);
        }

        public void ClearAction()
        {
            _action = null;

            Util.SetCg(_commandCg, false);
        }

        #endregion
    }
}
