using BlockUtils.Texturing;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Render.MeshUtils
{
    public class ExpandableMesh
    {
        private Mesh _mesh;

        private List<Vector3> _vertices = new List<Vector3>();
        private List<Vector2> _uvs = new List<Vector2>();
        private List<Vector3> _normals = new List<Vector3>();
        private List<int> _triangles = new List<int>();

        public ExpandableMesh()
        {
            _mesh = new Mesh();
        }

        public void Clear()
        {
            _mesh.Clear();
            _vertices.Clear();
            _uvs.Clear();
            _normals.Clear();
            _triangles.Clear();
        }

        public void AddMesh(Mesh mesh, Rect textureCoordinates, Vector3 position)
        {
            int pastVerticeCount = _vertices.Count;

            foreach (Vector3 vertice in mesh.vertices)
            {
                _vertices.Add((vertice * 0.5f) + position);
            }

            foreach (Vector3 normal in mesh.normals)
            {
                _normals.Add(normal);
            }

            float unitPerPointX = 1f / Atlas.PixelScale.x;
            float unitPerPointY = 1f / Atlas.PixelScale.y;

            float sizeInUnitsX = 1f / (Atlas.PixelScale.x / textureCoordinates.width);
            float sizeInUnitsY = 1f / (Atlas.PixelScale.y / textureCoordinates.height);

            Vector2 padding = new Vector2(textureCoordinates.x * unitPerPointX, textureCoordinates.y * unitPerPointY);
            Vector2 texuteUnitScale = new Vector2(sizeInUnitsX, sizeInUnitsY);

            foreach (Vector2 uv in mesh.uv)
            {
                _uvs.Add((uv * texuteUnitScale) + padding);
            }

            foreach (int triangle in mesh.triangles)
            {
                int id = pastVerticeCount + triangle;
                _triangles.Add(id);
            }
        }

        public void AddSide(Rect textureCoordinates, Vector3 position, Vector3[] vertices, Vector2[] uvs, Vector3 normal, Vector3[] triangles)
        {
            int pastVerticeCount = _vertices.Count;

            foreach (Vector3 vertice in vertices)
            {
                _vertices.Add((vertice * 0.5f) + position);
                _normals.Add(normal);
            }

            float unitPerPointX = 1f / Atlas.PixelScale.x;
            float unitPerPointY = 1f / Atlas.PixelScale.y;

            float sizeInUnitsX = 1f / (Atlas.PixelScale.x / textureCoordinates.width);
            float sizeInUnitsY = 1f / (Atlas.PixelScale.y / textureCoordinates.height);

            Vector2 padding = new Vector2(textureCoordinates.x * unitPerPointX, textureCoordinates.y * unitPerPointY);
            Vector2 texuteUnitScale = new Vector2(sizeInUnitsX, sizeInUnitsY);


            foreach (Vector2 uv in uvs)
            {
                _uvs.Add((uv * texuteUnitScale) + padding);
            }

            foreach (Vector3 triangle in triangles)
            {
                int id = pastVerticeCount + (int)triangle.x;
                _triangles.Add(id);
            }
        }
    
        public void Apply()
        {
            _mesh.vertices = _vertices.ToArray();
            _mesh.uv = _uvs.ToArray();
            _mesh.normals = _normals.ToArray();
            _mesh.triangles = _triangles.ToArray();
        }

        public Mesh GetMesh()
        {
            return _mesh;
        }
    }
}