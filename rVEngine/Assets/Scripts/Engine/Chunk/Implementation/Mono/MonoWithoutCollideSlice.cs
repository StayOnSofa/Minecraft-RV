using Render.MeshUtils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer), typeof(MeshFilter), typeof(Renderer))]
public class MonoWithoutCollideSlice : MonoBehaviour
{
    private MeshFilter _meshFilter;
    private MeshRenderer _meshRenderer;
    public void Init(MeshChunk meshChunk, Material material, Transform parent)
    {
        _meshRenderer = GetComponent<MeshRenderer>();
        _meshFilter = GetComponent<MeshFilter>();

        _meshRenderer.material = material;
        _meshFilter.mesh = meshChunk.GetMeshWithoutCollider();

        transform.SetParent(parent);
        transform.localPosition = Vector3.zero;

        gameObject.layer = 6;
    }
}
