using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PhysicsUtils
{
    [RequireComponent(typeof(BumpPhysics))]
    public class BumpGravity : MonoBehaviour
    {
        [SerializeField] private bool _infinityJump = false;
        [SerializeField] private float _gravityPower = 9;
        [SerializeField] private float _maxGravitySpeed = 6;
        [SerializeField] private float _jumpPower = 3.5f;


        private bool _inJump = false;
        private bool _inBumpOnTop = false;
        private bool _isFalling = false;

        private float _bodyGravity = 0;

        private BumpPhysics _physics;

        private Vector2 _moveDirection = Vector2.zero;

        private void Start()
        {
            _physics = GetComponent<BumpPhysics>();
        }

        public void Jump()
        {
            if (_physics.IsOnFloor() || _infinityJump)
            {
                if (_inJump == false || _infinityJump)
                {
                    _bodyGravity = _jumpPower;
                    _inJump = true;
                }
            }
        }

        private void Update()
        {
            float dt = Time.deltaTime;

            if (_physics.IsOnFloor())
            {
                if (_inJump == false)
                {
                    _bodyGravity = 0;
                }

                _inBumpOnTop = false;
                _inJump = false;
                _isFalling = false;
            }

            if (_bodyGravity > -_maxGravitySpeed)
            {
                _bodyGravity -= _gravityPower * dt;
                _isFalling = true;
            }

            if (_physics.IsOnTop())
            {
                if (_bodyGravity > 0)
                {
                    if (_inBumpOnTop == false)
                    {
                        _bodyGravity = 0;
                        _inBumpOnTop = true;
                    }
                }
            }

            _physics.Move(new Vector3(_moveDirection.x, _bodyGravity * dt, _moveDirection.y));
        }

        public void Move(Vector2 vector)
        {
            _moveDirection = vector;
        }

        public bool IsFalling()
        {
            return _isFalling;
        }
    }
}
