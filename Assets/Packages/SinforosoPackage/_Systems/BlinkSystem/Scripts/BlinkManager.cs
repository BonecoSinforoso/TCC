using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

namespace SinforosoPackage
{
    public class BlinkManager : MonoBehaviour
    {
        #region membros

        public static BlinkManager Instance { get; private set; }

        [SerializeField] private CanvasGroup _cg;

        #endregion

        #region metodos

        private void Start()
        {
            Instance = this;
        }

        public void StartBlink(float duration)
        {
            Util.SetCg(_cg, true);

            Invoke(nameof(StopBlink), duration);
        }

        public void StartFadedBlink(float blinkDuration, float fadeDuration)
        {
            StartCoroutine(FadedBlink_Coroutine(blinkDuration, fadeDuration));
        }

        private void StopBlink()
        {
            Util.SetCg(_cg, false);
        }

        private IEnumerator FadedBlink_Coroutine(float blinkDuration, float fadeDuration)
        {
            Task fadeTask = Util.SetCg(_cg, true, fadeDuration);

            yield return new WaitUntil(() => fadeTask.IsCompleted);

            yield return new WaitForSeconds(blinkDuration);

            _ = Util.SetCg(_cg, false, fadeDuration);
        }

        #region testes

        [SinforosoButton]
        private void TestBlink() => StartBlink(0.5f);

        [SinforosoButton]
        private void TestFadedBlink() => StartFadedBlink(1f, 1f);

        #endregion

        #endregion
    }
}
