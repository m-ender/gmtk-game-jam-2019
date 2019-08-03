using DG.Tweening;
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
        [SerializeField] private float boostDuration = 0f;

        [SerializeField] private Color playerColor = Color.white;
        [SerializeField] private PlayerWall wallPrefab = null;
        [SerializeField] private Direction initialDirection = Direction.None;

        public event Action Destroyed;

        public event Action<GameObject> ItemCollected;

        private Rigidbody2D rigidBody;
        private PlayerWall previousWall;
        private PlayerWall currentWall;

        private Direction currentDirection;
        private float currentSpeed;
        private Tween speedTimer;

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
                {
                    if (dir == currentDirection)
                        Accelerate();
                    else if (dir == currentDirection.Reverse())
                        Decelerate();
                    else
                        Turn(dir);
                }
            }
        }

        private void Accelerate()
        {
            currentSpeed = baseSpeed * fastModifier;
            rigidBody.velocity = currentSpeed * currentDirection.ToVector2();
            SetUpSpeedTimer(boostDuration);
        }

        private void Decelerate()
        {
            currentSpeed = baseSpeed * slowModifier;
            rigidBody.velocity = currentSpeed * currentDirection.ToVector2();
            SetUpSpeedTimer(boostDuration);
        }

        private void SetUpSpeedTimer(float timeout)
        {
            speedTimer?.Complete();
            speedTimer = DOTween.Sequence().InsertCallback(
                timeout,
                () =>
                {
                    currentSpeed = baseSpeed;
                    rigidBody.velocity = currentSpeed * currentDirection.ToVector2();
                });
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
                case ColliderType.Item:
                    ItemCollected?.Invoke(collision.gameObject);
                    break;
            }
        }

        public void DestroyPlayer()
        {
            if (destroyed) return;

            speedTimer?.Complete();
            Destroy(transform.parent.gameObject);
            Destroyed?.Invoke();
            destroyed = true;
        }
    }
}
