using System;
using CodeBase.Logic;
using CodeBase.Units;
using UnityEngine;

namespace CodeBase.UI.Buttons
{
    public class UnitButton : BaseButton
    {
        [SerializeField] private Unit _unit;

        public event Action<Unit> UnitChanged; 
        
        protected override void OnClick()
        {
            UnitChanged?.Invoke(_unit);
        }
    }
}