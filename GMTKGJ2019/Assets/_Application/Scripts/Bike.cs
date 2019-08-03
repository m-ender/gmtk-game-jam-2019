using DG.Tweening;
using System;
using UnityEngine;

namespace GMTKGJ2019
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class Bike : MonoBehaviour
    {
        [SerializeField] private Color playerColor = Color.white;
        [SerializeField] private PlayerWall wallPrefab = null;
        [SerializeField] private Direction initialDirection = Direction.None;

        private GameParameters parameters;

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
            currentSpeed = parameters.BikeBaseSpeed;
            Turn(initialDirection);
        }

        private void Awake()
        {
            rigidBody = GetComponent<Rigidbody2D>();

            parameters = GameParameters.Instance;

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
            currentSpeed = parameters.BikeBaseSpeed * parameters.BikeFastModifier;
            rigidBody.velocity = currentSpeed * currentDirection.ToVector2();
            SetUpSpeedTimer(parameters.BikeBoostDuration);
        }

        private void Decelerate()
        {
            currentSpeed = parameters.BikeBaseSpeed * parameters.BikeSlowModifier;
            rigidBody.velocity = currentSpeed * currentDirection.ToVector2();
            SetUpSpeedTimer(parameters.BikeBoostDuration);
        }

        private void SetUpSpeedTimer(float timeout)
        {
            speedTimer?.Complete();
            speedTimer = DOTween.Sequence().InsertCallback(
                timeout,
                () =>
                {
                    currentSpeed = parameters.BikeBaseSpeed;
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
