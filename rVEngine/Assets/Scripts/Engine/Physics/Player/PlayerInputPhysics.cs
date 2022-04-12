using PhysicsUtils.Camera;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PhysicsUtils
{
    [RequireComponent(typeof(BumpGravity))]
    public class PlayerInputPhysics : MonoBehaviour
    {
        [SerializeField] private CameraHolder _cameraHolder;

        [SerializeField] private float _speed = 2f;
        [SerializeField] private float _sprintSpeed = 4f;

        [SerializeField] private KeyCode _jumpButton = KeyCode.Space;
        [SerializeField] private KeyCode _sprintButton = KeyCode.LeftShift;
        [SerializeField] private KeyCode _crouchButton = KeyCode.C;

        [SerializeField] private KeyCode _forwardButton = KeyCode.W;
        [SerializeField] private KeyCode _backwardButton = KeyCode.S;
        [SerializeField] private KeyCode _leftButton = KeyCode.A;
        [SerializeField] private KeyCode _rightButton = KeyCode.D;

        private BumpGravity _gravity;
        private BumpPhysics _physics;

        private bool _isCrouch = false;

        private float startCollider;

        private void Start()
        {
            _gravity = GetComponent<BumpGravity>();
            _physics = GetComponent<BumpPhysics>();
        }

        private void Jump()
        {
            if (Input.GetKey(_jumpButton))
            {
                _gravity.Jump();
            }
        }

        private void Crouch()
        {
            if (Input.GetKeyDown(_crouchButton))
            {
                _isCrouch = !_isCrouch;
                if (_isCrouch)
                {
                    startCollider = _physics.GetCollider();
                    _physics.SetCollider(startCollider/2f);
                }
                else
                {
                    _physics.SetCollider(startCollider);
                }
            }
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

        private void Update()
        {
            _gravity.Move(Vector3.zero);

            Crouch();
            Jump();

            float dt = Time.deltaTime;

            var angle = _cameraHolder.GetDirection();
            float speed = _speed;

            bool isMoving = false;

            Vector2 inputDirection = Vector2.zero;

            if (Input.GetKey(_sprintButton))
            {
                speed = _sprintSpeed;
            }

            if (Input.GetKey(_forwardButton))
            {
                inputDirection.y = 1;
                isMoving = true;
            }

            if (Input.GetKey(_backwardButton))
            {
                inputDirection.y = -1;
                isMoving = true;
            }

            if (Input.GetKey(_leftButton))
            {
                inputDirection.x = -1;
                isMoving = true;
            }

            if (Input.GetKey(_rightButton))
            {
                inputDirection.x = 1;
                isMoving = true;
            }

            if (isMoving)
            {
                float inputAngle = Angle(inputDirection);
                angle += inputAngle;

                Vector3 direction = new Vector3(Mathf.Sin(Mathf.Deg2Rad * angle), Mathf.Cos(Mathf.Deg2Rad * angle), 0);
                _gravity.Move(direction * speed * dt);
            }
        }
    }
}