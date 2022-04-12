using Bump;
using UnityEngine;
using WorldUtils;

namespace Entity
{
    public abstract class AbstractEntity : MonoBehaviour
    {
        public enum EntityCollisionState
        {
            HitTop,
            InAir,
            Grounded,
        }

        private Box[] _collideBoxes;

        protected Box _entityCollision;
        private IWorld _world;

        public EntityCollisionState CollisionState = EntityCollisionState.InAir;

        private void Start()
        {
            OnLoad();
        }

        public abstract void OnLoad();

        public void Init(IWorld world, Box collision)
        {
            _entityCollision = collision;
            _world = world;
        }

        private void FixedUpdate()
        {
            _collideBoxes = WorldAABB.BoxToWorldKeys(_entityCollision, _world);
        }

        private void Update()
        {
            float dt = Time.deltaTime;
            Tick(dt);
        }

        public void SetPosition(Vector3 position)
        {
            transform.position = position;
            _entityCollision.Position = position;
        }

        private int normalY = 0;
        public void Move(Vector3 direction)
        {
            _entityCollision.Velocity = direction;

            _entityCollision.Position = AABB.Move(_entityCollision, _collideBoxes, ref normalY);
            transform.position = _entityCollision.Position;

            CollisionState = (EntityCollisionState)normalY + 1;

            transform.position = _entityCollision.Position;
        }
        public abstract void Tick(float dt);
    }
}