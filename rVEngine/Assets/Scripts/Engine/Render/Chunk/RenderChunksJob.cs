using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace ChunkUtils.Implementation
{
    public class RenderChunksJob : MonoBehaviour
    {
        private const int cAllocatedTrashMaximum = 64;
        private int _allocatedTrash = 0;

        private bool _taksInProcess = false;

        private List<MonoChunkSlice> _slices = new List<MonoChunkSlice>();
        private List<MonoChunkSlice> _slicesSafe = new List<MonoChunkSlice>();
        public void AddSlice(MonoChunkSlice slice)
        {
            if (!_slices.Contains(slice))
            {
                _slices.Add(slice);
            }
        }

        public void RemoveSlice(MonoChunkSlice slice)
        {
            if (_slices.Contains(slice))
            {
                _slices.Remove(slice);
            }
        }

        private void FixedUpdate()
        {
            if (!_taksInProcess)
            {
                if (_allocatedTrash > cAllocatedTrashMaximum)
                {
                    Resources.UnloadUnusedAssets();

                    _allocatedTrash = 0;
                }

                RemoveSafety();
                UpdateSafetyList();

                _taksInProcess = true;
                StartCoroutine(BuildingMeshes(_slicesSafe));
            }
        }

        private void UpdateSafetyList()
        {
            for (int i = 0; i < _slices.Count; i++)
            {
                MonoChunkSlice slice = _slices[i];

                if (slice.IsVisible())
                {
                    _slicesSafe.Add(_slices[i]);
                }
            }
        }

        private void RemoveSafety()
        {
            for (int i = 0; i < _slicesSafe.Count; i++)
            {
                if (_slices.Contains(_slicesSafe[i]))
                {
                    _allocatedTrash++;
                    _slices.Remove(_slicesSafe[i]);
                }
            }

            _slicesSafe.Clear();
        }

        private static Stopwatch stopwatch = new Stopwatch();

        private IEnumerator BuildingMeshes(List<MonoChunkSlice> slices)
        {
            for (int i = 0; i < slices.Count; i++)
            {
                stopwatch.Start();
                MonoChunkSlice slice = slices[i];

                if (slice != null)
                {
                    if (!slice.IsBuilded())
                    {
                        slice.BuildMesh();
                        //UnityEngine.Debug.LogWarning("Builded: " + stopwatch.ElapsedMilliseconds);
                        stopwatch.Restart();

                        yield return null;
                    }
                }

                yield return null;
            }

            _taksInProcess = false;
        }
    }
}