using System;
using System.Collections.Generic;
using UnityEngine;

namespace SinforosoPackage
{
    /// <summary>
    /// classe feita pra centralizar alguns debugs repetitivos no momento de desenvolvimento do jogo
    /// </summary>
    public class DebugManager : MonoBehaviour
    {
        #region membros

        [SerializeField] private bool _logActive;
        [SerializeField] private bool _warningActive;
        [SerializeField] private bool _errorActive;

        [Header("tipos")]
        [SerializeField] private List<DebugData> _datas = new();

        #region propriedades

        private static bool LogActive { get; set; }
        private static bool WarningActive { get; set; }
        private static bool ErrorActive { get; set; }

        private static List<DebugData> Datas { get; set; } = new();

        #endregion

        #endregion

        #region metodos

        private void Start()
        {
            LogActive = _logActive;
            WarningActive = _warningActive;
            ErrorActive = _errorActive;
        }

        public static void Log(object message, DebugType debugType = DebugType.General)
        {
            if (!LogActive || !GetDebugIsActive(debugType)) return;

            string coloredMessage = $"<color=#{ColorUtility.ToHtmlStringRGB(GetColor(debugType))}>{message}</color>";

            Debug.Log(coloredMessage);
        }

        public static void Warning(object message, DebugType debugType = DebugType.General)
        {
            if (!WarningActive || !GetDebugIsActive(debugType)) return;
            Debug.LogWarning(message);
        }

        public static void Error(object message, DebugType debugType = DebugType.General)
        {
            if (!ErrorActive || !GetDebugIsActive(debugType)) return;
            Debug.LogError(message);
        }

        private static bool GetDebugIsActive(DebugType debugType)
        {
            foreach (DebugData condition in Datas)
            {
                if (condition.DebugType == debugType)
                {
                    return condition.IsActive;
                }
            }

            return true;
        }

        [SinforosoButton]
        private void CreateDebugDatas()
        {
            _datas.Clear();
            Datas.Clear();

            foreach (DebugType debugType in Enum.GetValues(typeof(DebugType)))
            {
                _datas.Add(new DebugData
                {
                    Name = debugType.ToString(),
                    IsActive = true,
                    DebugType = debugType
                });
            }

            Datas = _datas;

#if UNITY_EDITOR

            UnityEditor.EditorUtility.SetDirty(this); // marca este componente como alterado
#endif
        }

        private void OnValidate()
        {
            LogActive = _logActive;
            WarningActive = _warningActive;
            ErrorActive = _errorActive;

            Datas = _datas;
        }

        private static Color GetColor(DebugType debugType)
        {
            foreach (DebugData condition in Datas)
            {
                if (condition.DebugType == debugType)
                {
                    return condition.Color;
                }
            }
            return Color.white;
        }

        #region tests

        [SinforosoButton]
        private void TestDebugLog() => Log("This is a test log message.");

        [SinforosoButton]
        private void TestColoredDebugLog() => Log($"Finalizando o estado: <color=red>bomba</color> com a razao: <color=yellow>lixo</color>, indo para <color=red>rodrigo</color>", DebugType.Combat);

        [SinforosoButton]
        private void TestDebugWarning() => Warning("This is a test warning message.");

        [SinforosoButton]
        private void TestDebugError() => Error("This is a test error message.");

        #endregion

        #endregion
    }
}
