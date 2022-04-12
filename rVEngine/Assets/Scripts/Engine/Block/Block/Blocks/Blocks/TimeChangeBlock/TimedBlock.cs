using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WorldUtils;

namespace BlockUtils
{
    [CreateAssetMenu(fileName = "TimedBlock", menuName = "Block/Time/TimedBlock")]
    public class TimedBlock : SegmentBlock
    {
        [SerializeField] private List<SegmentBlock> _blockSegments = new List<SegmentBlock>();
        [SerializeField] private Block _lastStateBlock;

        [SerializeField] private float _procentSegmentGrow = 10;

        public override void Init(int id)
        {
            base.Init(id);

            if (_blockSegments.Count > 0)
            {
                PostInit(_procentSegmentGrow, _blockSegments[0]);

                for (int i = 0; i < _blockSegments.Count-1; i++)
                {
                    _blockSegments[i].PostInit(_procentSegmentGrow, _blockSegments[i + 1]);
                }

                _blockSegments[_blockSegments.Count - 1].PostInit(_procentSegmentGrow, _lastStateBlock);
            }
        }
    }
}