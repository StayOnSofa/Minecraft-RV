using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace ChunkUtils.Implementation
{
    public class MonoChunkLoader : MonoBehaviour, IChunkLoader
    {
        [SerializeField] public UnityEvent<IEnumerable<IChunk>> OnAcceptChunks;
        private Vector3 _position;

        public void Accept(IEnumerable<IChunk> chunksLoad)
        {
            OnAcceptChunks?.Invoke(chunksLoad);
        }

        public Vector3 GetPosition()
        {
            return _position;
        }

        public void Reload()
        {
            _position = transform.position;
        }
    }
}
