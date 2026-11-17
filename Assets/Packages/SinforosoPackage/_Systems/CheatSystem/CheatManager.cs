using UnityEngine;

namespace SinforosoPackage
{
    public class CheatManager : MonoBehaviour
    {
        #region membros

        public static CheatManager Instance { get; private set; }

        [SerializeField] private bool _canCheat;
        [SerializeField] private HotKeyCheat[] _hotKeyCheats;
        [SerializeField] private SimpleCheat[] _simpleCheats;

        #endregion

        #region metodos

        private void Start()
        {
            Instance = this;
        }

        private void Update()
        {
            if (!_canCheat) return;

            foreach (HotKeyCheat hotKeyCommand in _hotKeyCheats)
            {
                if (Input.GetKeyDown(hotKeyCommand.KeyCode)) hotKeyCommand.UnityEvent?.Invoke();
            }
        }

        public bool CheckCheat(CheatType cheatType)
        {
            if (!_canCheat) return false;

            foreach (SimpleCheat simpleCheat in _simpleCheats)
            {
                if (simpleCheat.CheatType == cheatType)
                {
#if !UNITY_EDITOR
                    if (!simpleCheat.CanRunInBuild) return false;
#endif

                    return simpleCheat.Active;
                }
            }

            return false;
        }

        #endregion
    }
}
