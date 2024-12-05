using System;
using UnityEngine;

namespace CodeBase.Logic
{
    [Serializable]
    public class SerializedChunk
    {
        [field:SerializeField] public Vector2Int LocalPosition { get; set; }
        
        [field:SerializeField] public Color[] Texture { get; set; }
        [field:SerializeField] public Vector3[] Vertices { get; set; }
        [field:SerializeField] public int[] Triangles { get; set; }
        [field:SerializeField] public string TexturePath { get; set; }
        
    }
}