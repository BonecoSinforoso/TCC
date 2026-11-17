using UnityEngine;
using UnityEngine.Events;

namespace SinforosoPackage
{
    public class CommandController : MonoBehaviour
    {
        [SerializeField] private UnityEvent _mouseDownUnityEvent;
        [SerializeField] private UnityEvent _mouseEnterUnityEvent;
        [SerializeField] private UnityEvent _mouseExitUnityEvent;

        private void OnMouseDown() => _mouseDownUnityEvent?.Invoke();
        private void OnMouseEnter() => _mouseEnterUnityEvent?.Invoke();
        private void OnMouseExit() => _mouseExitUnityEvent?.Invoke();
    }
}
