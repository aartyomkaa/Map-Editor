using System;
using CodeBase.Units;
using UnityEngine;

namespace CodeBase.Data
{
    [Serializable]
    public class SerializedUnit
    {
        [field: SerializeField] public UnitType Type { get; set; }
        [field: SerializeField] public Vector3 Position { get; set; }
    }
}