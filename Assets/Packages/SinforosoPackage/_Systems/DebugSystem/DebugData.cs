using System;
using UnityEngine;

namespace SinforosoPackage
{
    [Serializable]
    public class DebugData
    {
        [SerializeField] private string _name;
        [SerializeField] private bool _isActive;
        [SerializeField] private DebugType _debugType;
        [SerializeField] private Color _color = Color.white;

        public string Name { get => _name; set => _name = value; }
        public bool IsActive { get => _isActive; set => _isActive = value; }
        public DebugType DebugType { get => _debugType; set => _debugType = value; }
        public Color Color => _color;
    }
}
