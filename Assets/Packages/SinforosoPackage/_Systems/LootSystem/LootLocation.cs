using System;
using UnityEngine;

namespace SinforosoPackage
{
    [Serializable]
    public class LootLocation
    {
        [SerializeField] private string _name;
        [SerializeField] private Transform _transform;
    }
}
