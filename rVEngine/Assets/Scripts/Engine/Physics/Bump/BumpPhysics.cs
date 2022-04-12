using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace PhysicsUtils
{
    [RequireComponent(typeof(CharacterController))]
    public class BumpPhysics : MonoBehaviour
    {
        private const float c_MaxDistance = 50;
        private const float c_MaxTopDistance = 0.25f;

        [SerializeField] private float _colliderHeight;
        [SerializeField] private float _colliderWidth;

        [HideInInspector] public UnityEvent OnStopByTopCollider = new UnityEvent();

        private CharacterController _controller;
        private Vector3 _topHeightPosition;

        private bool _isOnTop = false;

        private void Start()
        {
            transform.localScale = Vector3.one;

            _controller = GetComponent<CharacterController>();

            _controller.radius = _colliderWidth;
            _controller.height = _colliderHeight;
        }

        private void OnValidate()
        {
            _controller = GetComponent<CharacterController>();

            _controller.height = _colliderHeight;
            _controller.radius = _colliderWidth;

        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;

            Vector3 topPosition = transform.position + new Vector3(0, (_colliderHeight / 2.05f));
            Vector3 boxCastBoxSize = Vector3.one * _colliderWidth * 1.5f;
            boxCastBoxSize.y = 0;

            RaycastHit hit;
            bool isVisible = Physics.BoxCast(topPosition, boxCastBoxSize * 0.5f, transform.up, out hit, Quaternion.identity);
            _topHeightPosition = (topPosition + transform.up * c_MaxDistance) + new Vector3(0, boxCastBoxSize.y / 2f, 0);

            if (isVisible)
            {
                _topHeightPosition = (topPosition + transform.up * hit.distance) + new Vector3(0, boxCastBoxSize.y/2f, 0);
                Gizmos.DrawSphere(_topHeightPosition, 0.05f);
            }

            if (Vector3.Distance(_topHeightPosition, topPosition) < c_MaxTopDistance)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawSphere(topPosition, 0.07f);
                Gizmos.color = Color.red;
            }


            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(transform.position + new Vector3(0, (_colliderHeight/2.05f), 0), 0.05f);
            Gizmos.DrawRay(topPosition, transform.up * 100);


            Gizmos.DrawWireCube(transform.position, new Vector3(_colliderWidth*2f, _colliderHeight, _colliderWidth*2f));
        }

        public bool IsOnFloor()
        {
            return _controller.isGrounded;
        }

        public void Move(Vector3 direction)
        {
            _controller.Move(direction);
        }

        public void SetCollider(float size)
        {
            _colliderHeight = size;
            _controller.height = _colliderHeight;
        }

        public float GetCollider()
        {
            return _colliderHeight;
        }

        private void IsOnTopCheck()
        {
            _isOnTop = false;

            Vector3 topPosition = transform.position + new Vector3(0, (_colliderHeight / 2.05f));
            Vector3 boxCastBoxSize = Vector3.one * _colliderWidth * 1.5f;
            boxCastBoxSize.y = 0;

            RaycastHit hit;
            bool isVisible = Physics.BoxCast(topPosition, boxCastBoxSize * 0.5f, transform.up, out hit, Quaternion.identity);
            _topHeightPosition = (topPosition + transform.up * c_MaxDistance) + new Vector3(0, boxCastBoxSize.y / 2f, 0);

            if (isVisible)
            {
                _topHeightPosition = (topPosition + transform.up * hit.distance) + new Vector3(0, boxCastBoxSize.y / 2f, 0);
            }

            if (Vector3.Distance(_topHeightPosition, topPosition) < c_MaxTopDistance)
            {
                _isOnTop = true;
            }
        }

        private void Update()
        {
            IsOnTopCheck();
        }

        public bool IsOnTop()
        {
            return _isOnTop;
        }
    }
}