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

        public void StartBike()
        {
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
            currentWall.UpdateEnd(transform.localPosition);

            if (Input.GetKeyDown(KeyCode.D))
            {
                Turn(Direction.East);
            }
            else if (Input.GetKeyDown(KeyCode.W))
            {
                Turn(Direction.North);
            }
            else if (Input.GetKeyDown(KeyCode.S))
            {
                Turn(Direction.South);
            }
            else if (Input.GetKeyDown(KeyCode.A))
            {
                Turn(Direction.West);
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

        private void DestroyPlayer()
        {
            Destroy(transform.parent.gameObject);
            Destroyed?.Invoke();
        }
    }
}