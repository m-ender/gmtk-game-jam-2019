using System;
using UnityEngine;

namespace GMTKGJ2019
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class Bike : MonoBehaviour
    {
        [SerializeField] private float baseSpeed = 0f;
        [SerializeField] private float fastModifier = 0f;
        [SerializeField] private float slowModifier = 0f;

        [SerializeField] private Color playerColor = Color.white;
        [SerializeField] private PlayerWall wallPrefab = null;
        [SerializeField] private Direction initialDirection = Direction.None;

        public event Action Destroyed;

        private Rigidbody2D rigidBody;
        private PlayerWall previousWall;
        private PlayerWall currentWall;

        private Direction currentDirection;
        private float currentSpeed;

        private KeyCode key;
        private SteeringWheel steeringWheel;

        private bool started = false;
        private bool destroyed = false;

        public void Initialize(KeyCode key, SteeringWheel steeringWheel)
        {
            this.key = key;
            this.steeringWheel = steeringWheel;
        }

        public void StartBike()
        {
            started = true;
            currentSpeed = baseSpeed;
            Turn(initialDirection);
        }

        private void Awake()
        {
            rigidBody = GetComponent<Rigidbody2D>();

            currentSpeed = 0f;
            Turn(initialDirection);

            previousWall = currentWall;
        }

        private void Update()
        {
            if (!started)
                return;

            currentWall.UpdateEnd(transform.localPosition);

            if (Input.GetKeyDown(key))
            {
                Direction dir = steeringWheel.CurrentDirection;
                if (dir != Direction.None)
                    Turn(dir);
            }
        }

        private void Turn(Direction dir)
        {
            currentDirection = dir;

            rigidBody.MoveRotation(dir.ToAngle());
            rigidBody.velocity = currentSpeed * dir.ToVector2();

            previousWall = currentWall;
            currentWall = Instantiate(wallPrefab, transform.parent);
            currentWall.Initialize(currentDirection.IsHorizontal(), transform.localPosition, playerColor);
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            switch (collision.GetComponent<ColliderTypeHolder>().Type)
            {
            case ColliderType.Wall:
                if (collision.gameObject != currentWall.gameObject
                    && collision.gameObject != previousWall.gameObject)
                {
                    DestroyPlayer();
                }
                break;
            }
        }

        public void DestroyPlayer()
        {
            if (destroyed) return;

            Destroy(transform.parent.gameObject);
            Destroyed?.Invoke();
            destroyed = true;
        }
    }
}