using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace ChunkUtils.Implementation
{
    public class InitChunksJob : MonoBehaviour
    {
        [SerializeField] private MonoChunkLoader _monoChunkLoader;
        [SerializeField] public UnityEvent<IEnumerable<MonoChunk>> OnChunkRealized;

        private Dictionary<IChunk, MonoChunk> _chunks = new Dictionary<IChunk, MonoChunk>();

        private void Start()
        {
            _monoChunkLoader.OnAcceptChunks.AddListener(Accept);
        }
        private void Accept(IEnumerable<IChunk> chunks)
        {
            List<IChunk> sortedChunks = chunks.OrderBy(
                chunk => Vector3.Distance(chunk.GetPosition(), transform.position)
            ).ToList();

            StartCoroutine(GenerationJob(sortedChunks));
        }

        private MonoChunk InitChunk(IChunk chunk)
        {
            MonoChunk monoChunk = new GameObject("[CHUNK]").AddComponent<MonoChunk>();
            monoChunk.Init(chunk);

            return monoChunk;
        }

        private IEnumerator GenerationJob(IEnumerable<IChunk> chunks)
        {
            foreach (IChunk chunk in chunks)
            {
                if (!_chunks.ContainsKey(chunk))
                {
                        IChunk _chunk = chunk;

                        chunk.OnChunkDestroyed().AddListener(()=> {
                            ChunkDestroyed(_chunk);
                        });

                    MonoChunk monoChunk = InitChunk(chunk);
                    _chunks.Add(chunk, monoChunk);
                }

                yield return null;
            }

            OnChunkRealized?.Invoke(GetRealizedChunks());
        }

        private void ChunkDestroyed(IChunk chunk)
        {
            _chunks.Remove(chunk);
        }

        private IEnumerable<MonoChunk> GetRealizedChunks()
        {
            List<MonoChunk> monoChunks = new List<MonoChunk>();

            for (int i = 0; i < _chunks.Values.Count; i++)
            {
                MonoChunk monoChunk = _chunks.ElementAt(i).Value;
                if (monoChunk != null)
                {
                    monoChunks.Add(_chunks.ElementAt(i).Value);
                }
            }

            return monoChunks;
        }

        private void OnDestroy()
        {
            _monoChunkLoader.OnAcceptChunks.RemoveListener(Accept);
        }
    }
}