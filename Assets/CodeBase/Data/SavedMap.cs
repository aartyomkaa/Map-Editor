using System;
using UnityEngine;

namespace CodeBase.Data
{
    [Serializable]
    public class SavedMap
    {
        [field: SerializeField] public string Grid;
        [field: SerializeField] public string Units;
    }
}