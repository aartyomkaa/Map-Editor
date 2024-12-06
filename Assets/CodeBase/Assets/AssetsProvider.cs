using System;
using System.Collections.Generic;
using CodeBase.Units;

namespace CodeBase.Assets
{
    public static class AssetsProvider
    {
        private static readonly Dictionary<UnitType, string> UnitPrefabPaths = new()
        {
            { UnitType.AntiAircraftWeapons, "Units/Anti-aircraft_weapons" },
            { UnitType.Leopard, "Units/Leopard" },
            { UnitType.PzH2000, "Units/PzH 2000" },
        };

        public static string GetPath(UnitType type)
        {
            if (UnitPrefabPaths.TryGetValue(type, out string path))
            {
                return path;
            }
            throw new Exception($"No prefab path found for unit type {type}");
        }
    }
}