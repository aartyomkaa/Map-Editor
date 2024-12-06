using UnityEngine;

namespace CodeBase.Units
{
    public class Unit : MonoBehaviour
    {
        [SerializeField] private UnitType _unitType;

        public UnitType Type => _unitType;
    }
}