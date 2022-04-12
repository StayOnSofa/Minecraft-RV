using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PhysicsUtils
{
    [RequireComponent(typeof(CharacterController))]
    public class EntityPhysics : MonoBehaviour
    {
        private CharacterController _cotroller;
        private void Start()
        {
            _cotroller = GetComponent<CharacterController>();
        }

        private void Update()
        {
            float dt = Time.deltaTime;

            if (Input.GetKey(KeyCode.S))
            {
                _cotroller.Move(new Vector3(0, -20 * dt, 0));
            }

            if (Input.GetKey(KeyCode.W))
            {
                _cotroller.Move(new Vector3(0, 20 * dt, 0));
            }
        }
    }
}
