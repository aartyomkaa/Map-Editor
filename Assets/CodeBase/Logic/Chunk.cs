using UnityEngine;

namespace CodeBase.Logic
{
    public class Chunk : MonoBehaviour
    {
        public Vector2Int LocalPosition { get; }
        public Vector3[] Mesh { get; set; }
        public Color[,] Texture { get; set; }
    }
}