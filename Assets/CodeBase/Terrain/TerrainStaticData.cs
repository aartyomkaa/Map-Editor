using UnityEngine;

namespace CodeBase.Terrain
{
    [CreateAssetMenu(fileName = "TerrainData", menuName = "StaticData/Terrain")]
    public class TerrainStaticData : ScriptableObject
    {
        public int width = 1000;
        public int depth = 1000;
        public float cellSize = 1f;
        public int chunkSize = 100;
        public float heightScale = 5f;
    }
}