using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PhysicsUtils.Camera
{
    [RequireComponent(typeof(Transform))]
    public class CameraHolder : MonoBehaviour
    {
        private const string _mouseAxisX = "Mouse X";
        private const string _mouseAxisY = "Mouse Y";


        [SerializeField] private float _sensitivity = 1f;

        [SerializeField] private bool _grabOnStart = false;

        private Transform _transform;

        private float _restriction = 90f;

        private bool _grab = false;

        private void Start()
        {
            _transform = transform;
            SetGrab(_grabOnStart);
        }

        private Vector2 rotation;

        private Vector2 GetAxis()
        {
            return new Vector2(Input.GetAxisRaw(_mouseAxisX), Input.GetAxisRaw(_mouseAxisY));
        }

        private void Update()
        {
            if (_grab)
            {
                Vector2 motion = GetAxis() * _sensitivity;

                rotation.x = Mathf.Clamp(rotation.x - motion.y, -_restriction, _restriction);
                rotation.y += motion.x;

                _transform.eulerAngles = rotation;

            }
        }

        private void SetCursor(bool value)
        {
            CursorLockMode lockState = value ? CursorLockMode.Locked : CursorLockMode.Confined;
            Cursor.lockState = lockState;
            Cursor.visible = value;
        }

        public void Grab()
        {
            _grab = true;
            SetCursor(_grab);
        }

        public void UnGrub()
        {
            _grab = false;
            SetCursor(_grab);
        }

        public void SetGrab(bool value)
        {
            _grab = value;
            SetCursor(_grab);
        }

        public void SetSensetive(float value)
        {
            _sensitivity = value;
        }

        public float GetDirection()
        {
            return _transform.rotation.eulerAngles.y;
        }
    }
}
