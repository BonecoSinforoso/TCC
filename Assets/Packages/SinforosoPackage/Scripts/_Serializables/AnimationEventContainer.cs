using System;
using UnityEngine;
using UnityEngine.Events;

namespace SinforosoPackage
{
    [Serializable]
    public class AnimationEventContainer
    {
        [SerializeField] private string _description;
        [SerializeField] private UnityEvent _unityEvent;

        public string Description => _description;
        public UnityEvent UnityEvent => _unityEvent;
    }
}
