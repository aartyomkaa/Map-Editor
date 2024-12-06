using System;
using CodeBase.Logic.Chunks;
using UnityEngine;

namespace CodeBase.Data
{
    [Serializable]
    public class SerializedChunk
    {
        [field:SerializeField] public Vector2Int LocalPosition { get; set; }
        [field:SerializeField] public Vector3[] Vertices { get; set; }
        [field:SerializeField] public int[] Triangles { get; set; }
        [field:SerializeField] public float[] ColorsR { get; set; }
    }
}