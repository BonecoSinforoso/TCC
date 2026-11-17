using UnityEngine;

namespace SinforosoPackage
{
    public class AnimationEventController : MonoBehaviour
    {
        [SerializeField] private bool _debug;
        [SerializeField] private AnimationEventContainer[] _animationEventContainers;

        public void CallEvent(int index)
        {
            if (_debug)
                DebugManager.Log($"evento foi chamado: {_animationEventContainers[index].Description}, index: " + index);

            _animationEventContainers[index].UnityEvent?.Invoke();
        }
    }
}
