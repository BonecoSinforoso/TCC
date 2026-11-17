using System.Threading.Tasks;
using UnityEngine;

namespace SinforosoPackage
{
    public static class Util
    {
        #region mais proximo

        public static bool IsClosest(Vector3 position1, Vector3 position2, Vector3 position3)
        {
            return Vector3.Distance(position1, position2) < Vector3.Distance(position1, position3);
        }

        public static bool IsClosest(Transform transform1, Transform transform2, Transform transform3)
        {
            return Vector3.Distance(transform1.position, transform2.position) < Vector3.Distance(transform1.position, transform3.position);
        }

        public static bool IsClosest2D(Vector3 value1, Vector3 value2, Vector3 value3)
        {
            return Vector2.Distance(value1, value2) < Vector2.Distance(value1, value3);
        }

        #endregion

        #region direcao

        public static float GetDirection2D(Vector2 pos, Vector2 targetPos, float increment = 0)
        {
            Vector2 dir = targetPos - pos;
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            return angle + increment;
        }

        public static float GetDirection3D(Vector3 pos, Vector3 targetPos, float offset = 0)
        {
            Vector3 dir = targetPos - pos;
            float angle = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg; // Eixo X em relação ao Z
            return angle + offset;
        }

        public static float GetFlatDirection3D(Vector3 pos, Vector3 targetPos, float offset = 0)
        {
            Vector3 flatPos = new(pos.x, 0, pos.z);
            Vector3 flatTarget = new(targetPos.x, 0, targetPos.z);
            Vector3 dir = flatTarget - flatPos;

            float angle = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg;
            return angle + offset;
        }

        #endregion

        public static float GetRandomFloat(Vector2 v2) => Random.Range(v2.x, v2.y);

        public static int GetRandomInt(Vector2 v2) => Random.Range((int)v2.x, (int)(v2.y + 1));

        #region canvas group

        public static void ToggleCg(CanvasGroup cg)
        {
            cg.alpha = cg.alpha == 0 ? 1 : 0;
            cg.blocksRaycasts = cg.alpha == 1;
            cg.interactable = cg.alpha == 1;
        }

        public static void SetCg(CanvasGroup cg, bool value)
        {
            cg.alpha = value ? 1 : 0;
            cg.blocksRaycasts = value;
            cg.interactable = value;
        }

        public static async Task SetCg(CanvasGroup cg, bool value, float duration)
        {
            float startAlpha = cg.alpha;
            float time = 0;
            float targetAlpha = value ? 1 : 0;

            while (time < duration)
            {
                time += Time.deltaTime;
                cg.alpha = Mathf.Lerp(startAlpha, targetAlpha, time / duration);

                await Task.Yield();
            }

            cg.alpha = targetAlpha;
            cg.blocksRaycasts = value;
            cg.interactable = value;
        }

        public static void ToggleCg(CanvasGroup cg, float duration)
        {
            _ = SetCg(cg, cg.alpha == 0, duration);
        }

        #endregion

        public static void PlayAudio(AudioSource audioSource, AudioClip[] audioClips)
        {
            audioSource.PlayOneShot(audioClips[Random.Range(0, audioClips.Length)]);
        }

        public static float GetFlatDistance(Vector3 pos, Vector3 targetPos)
        {
            Vector2 v1 = new(pos.x, pos.z);
            Vector2 v2 = new(targetPos.x, targetPos.z);

            return Vector2.Distance(v1, v2);
        }
    }
}
