using Bump;
using PhysicsUtils.Camera;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WorldUtils;

namespace Entity
{
    public class LocalPlayer : AbstractEntity
    {
        [SerializeField] private CameraHolder _cameraHolder;

        [SerializeField] private float _walkSpeed;
        [SerializeField] private float _runSpeed;

        [SerializeField] private GameObject _worldObject;
        private IWorld _world;

        private void OnValidate()
        {
            if (_worldObject != null)
            {
                _worldObject.TryGetComponent<IWorld>(out IWorld world);
                if (world == null)
                {
                    _worldObject = null;
                }
            }
        }

        private Box _player = new Box()
        {
            Position = Vector3.zero,
            Scale = new Vector3(0.5f, 1.75f, 0.5f),
            Velocity = Vector3.zero
        };

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawCube(transform.position + _player.Scale / 2, _player.Scale);
        }

        public override void OnLoad()
        {
            _world = _worldObject.GetComponent<IWorld>();
            _player.Position = transform.position;

            Init(_world, _player);
        }

        public static float Angle(Vector2 p_vector2)
        {
            if (p_vector2.x < 0)
            {
                return 360 - (Mathf.Atan2(p_vector2.x, p_vector2.y) * Mathf.Rad2Deg * -1);
            }
            else
            {
                return Mathf.Atan2(p_vector2.x, p_vector2.y) * Mathf.Rad2Deg;
            }
        }

        private float _maxGravity = -16;
        [SerializeField]private float _gravity = 0;
        private float _gravityPower = 3f;

        private bool _afterAir = true;
       [SerializeField] private bool _isJumping = false;
 
        public override void Tick(float dt)
        {

            if (CollisionState == EntityCollisionState.Grounded)
            {
                _gravity = -0.1f;
                _isJumping = false;
            }

            var angle = _cameraHolder.GetDirection();

                bool isMoving = false;
                float speed = _walkSpeed;

                Vector3 Direction = Vector3.zero;

                if (Input.GetKey(KeyCode.LeftShift))
                {
                    speed = _runSpeed;
                }

                Vector2 inputDirection = Vector2.zero;

                if (Input.GetKey(KeyCode.W))
                {
                    inputDirection.y = 1;
                    isMoving = true;
                }

                if (Input.GetKey(KeyCode.S))
                {
                    inputDirection.y = -1;
                    isMoving = true;
                }

                if (Input.GetKey(KeyCode.A))
                {
                    inputDirection.x = -1;
                    isMoving = true;
                }

                if (Input.GetKey(KeyCode.D))
                {
                    inputDirection.x = 1;
                    isMoving = true;
                }

                Vector3 direction = Vector3.zero;

                if (isMoving)
                {
                    float inputAngle = Angle(inputDirection);
                    angle += inputAngle;

                    direction = new Vector3(Mathf.Sin(Mathf.Deg2Rad * angle), 0, Mathf.Cos(Mathf.Deg2Rad * angle));
                }

            if (CollisionState == EntityCollisionState.InAir)
            {
                if (_gravity > _maxGravity)
                {
                    _gravity -= _gravityPower * dt;
                }
            }

            if (Input.GetKey(KeyCode.Space) && _isJumping == false)
            {
                if (CollisionState == EntityCollisionState.Grounded)
                {
                    _gravity = 1.1f;
                    CollisionState = EntityCollisionState.InAir;

                    _isJumping = true;
                }
            }

            if (CollisionState == EntityCollisionState.HitTop)
            {
                if (_gravity > 0)
                {
                    _gravity = -_gravityPower * dt;
                    CollisionState = EntityCollisionState.InAir;
                }
            }


            Direction.x = direction.x * speed * dt;
            Direction.z = direction.z * speed * dt;

            Direction.y = _gravity * _runSpeed * dt;

            Move(Direction);
        }
    }
}