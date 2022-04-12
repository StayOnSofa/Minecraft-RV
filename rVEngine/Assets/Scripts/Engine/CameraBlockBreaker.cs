using BlockUtils;
using ChunkUtils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WorldUtils;

public class CameraBlockBreaker : MonoBehaviour
{
    [SerializeField] private LayerMask IgnoreLayer;
    [SerializeField] private bool _crazyMod = false;
    [SerializeField] private float _maxCrazyTick = 0.2f;
    private float _crazyTimer = 0.0f;

    [SerializeField] private Vector3 _blockLastCoordinates;
    [SerializeField] private GameObject _oWorld;
    [SerializeField] private GameObject _indicator;

    [SerializeField] private KeyCode _keyCodeBreak = KeyCode.Mouse0;
    [SerializeField] private KeyCode _keyCodePlace = KeyCode.Mouse1;

    [SerializeField] private float _breakDistance = 5f;

    private IWorld _world;


    private void OnValidate()
    {
        if (_oWorld != null)
        {
            IWorld world = _oWorld.GetComponent<IWorld>();
            if (world != null)
            {
                return;
            }

            _oWorld = null;
        }
    }

    private void Start()
    {
        _world = _oWorld.GetComponent<IWorld>();
    }

    private Vector3 sphere;

    private bool IsCollide(int blockId)
    {
        if (blockId > BlockUtils.BlockHandler.SpecialBlockWithColliderBorder && blockId < BlockUtils.BlockHandler.RegularBlockWithColliderBorder)
        {
            return true;
        }

        return false;
    }

    private void Update()
    {
        bool collideWithoutCollider = false;

        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, _breakDistance-1, ~IgnoreLayer))
        {
            collideWithoutCollider = true;
        }

        bool iSeeBlock = false;

        Vector3Int blockPlace = Vector3Int.zero;
        Vector3Int block = Vector3Int.zero;
        int lastBlockID = 0;

        for (float i = 0; i < _breakDistance; i += 0.01f)
        {
            Vector3 forward = transform.position + transform.forward * i;
            Vector3Int roundForward = Vector3Int.RoundToInt(forward);

            if (_world.HasBlock(roundForward.x, roundForward.y, roundForward.z))
            {
                int blockId = _world.GetBlock(roundForward.x, roundForward.y, roundForward.z);
                lastBlockID = blockId;

                float distance = Vector3.Distance(hit.point, roundForward);

                if (IsCollide(blockId) || (collideWithoutCollider && distance < 0.6f))
                {
                    blockPlace = Vector3Int.RoundToInt(forward - transform.forward * 0.01f);
                    block = roundForward;

                    iSeeBlock = true;
                    _indicator.transform.position = roundForward;
                    break;
                }
            }
        }

        if (iSeeBlock)
        {
            _blockLastCoordinates = blockPlace;

            if (!_crazyMod)
            {
                if (Input.GetKeyDown(_keyCodeBreak))
                {
                    _world.BreackBlock(block.x, block.y, block.z);
                }
            }
            else 
            {
                if (Input.GetKey(_keyCodeBreak))
                {
                    _crazyTimer += Time.deltaTime;
                    if (_crazyTimer > _maxCrazyTick)
                    {
                        _world.BreackBlock(block.x, block.y, block.z);
                        _crazyTimer = 0;
                    }
                }
            }

            if (!_crazyMod)
            {
                if (Input.GetKeyDown(_keyCodePlace))
                {
                    _world.PlaceBlock(blockPlace.x, blockPlace.y, blockPlace.z, BlockRegister.CarrotSeed);
                }
            }
            else {
                if (Input.GetKey(_keyCodePlace))
                {
                    _crazyTimer += Time.deltaTime;
                    if (_crazyTimer > _maxCrazyTick)
                    {
                        _world.PlaceBlock(blockPlace.x, blockPlace.y, blockPlace.z, BlockRegister.Granite);
                        _crazyTimer = 0;
                    }
                }
            }

            if (Input.GetKeyDown(KeyCode.C))
            {
                int r = 120;

                for (int x = -r; x <= r; x++)
                {
                    for (int y = -r; y <= r; y++)
                    {
                        for (int z = -r; z <= r; z++)
                        {
                            _world.BreackBlock(blockPlace.x + x, blockPlace.y + y, blockPlace.z + z);
                        }
                    }
                }
            }

            if (Input.GetKeyDown(KeyCode.G))
            {
               _world.PlaceBlock(blockPlace.x, blockPlace.y, blockPlace.z, BlockRegister.Granite);
            }

            if (Input.GetKeyDown(KeyCode.V))
            {
                _world.PlaceBlock(blockPlace.x, blockPlace.y, blockPlace.z, BlockRegister.Sphere);
            }

            Block _block = _world.GetBlockHandler().GetBlock(lastBlockID);
          
            _indicator.transform.localScale = _block.PointerSize;
            _indicator.transform.position += _block.PointerPosition;
        }

        _indicator.SetActive(iSeeBlock);

    }
}

