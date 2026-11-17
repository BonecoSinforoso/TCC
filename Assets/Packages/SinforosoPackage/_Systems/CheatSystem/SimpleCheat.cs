using System;
using UnityEngine;

namespace SinforosoPackage
{
    [Serializable]
    public class SimpleCheat
    {
        [SerializeField] private bool _active;
        [SerializeField] private CheatType _cheatType;
        [SerializeField] private bool _canRunInBuild;

        public bool Active => _active;
        public CheatType CheatType => _cheatType;
        public bool CanRunInBuild => _canRunInBuild;
    }
}
