using DG.Tweening;
using System;
using UnityEngine;

namespace GMTKGJ2019
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class Player : MonoBehaviour
    {
        [SerializeField] private Transform bike = null;
        public SteeringWheel SteeringWheel = null;
        public Color playerColor = Color.white;
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

        private bool started = false;
        private bool destroyed = false;

        private void Awake()
        {
            rigidBody = GetComponent<Rigidbody2D>();

            parameters = GameParameters.Instance;

            Turn(initialDirection);
            SetSpeed(0f);

            previousWall = currentWall;
        }

        public void Initialize(KeyCode key)
        {
            this.key = key;
        }

        public void StartBike()
        {
            started = true;
            SetSpeed(parameters.BikeBaseSpeed);
            Turn(initialDirection);
        }

        public void StopBike()
        {
            SetSpeed(0f);
            started = false;
        }

        private void Update()
        {
            if (!started)
                return;

            UpdateWall();

            if (Input.GetKeyDown(key))
            {
                Direction dir = SteeringWheel.CurrentDirection;
                if (dir != Direction.None)
                {
                    SteeringWheel.AnimateInput();

                    if (dir == currentDirection)
                        Accelerate();
                    else if (dir == currentDirection.Reverse())
                        Decelerate();
                    else
                        Turn(dir);
                }
            }
        }

        private void UpdateWall()
        {
            currentWall.UpdateEnd(transform.localPosition);
        }

        public void DestroyPlayer()
        {
            if (destroyed) return;

            UpdateWall();
            speedTimer?.Complete();
            Destroy(gameObject);
            Destroyed?.Invoke();
            destroyed = true;
        }

        private void Accelerate()
        {
            SetSpeed(parameters.BikeBaseSpeed * parameters.BikeFastModifier);
            SetUpSpeedTimer(parameters.BikeBoostDuration);
        }

        private void Decelerate()
        {
            SetSpeed(parameters.BikeBaseSpeed * parameters.BikeSlowModifier);
            SetUpSpeedTimer(parameters.BikeBoostDuration);
        }

        private void SetUpSpeedTimer(float timeout)
        {
            speedTimer?.Complete();
            speedTimer = DOTween.Sequence().InsertCallback(
                timeout,
                () => SetSpeed(parameters.BikeBaseSpeed));
        }

        private void Turn(Direction dir)
        {
            currentDirection = dir;

            rigidBody.velocity = currentSpeed * dir.ToVector2();
            bike.localEulerAngles = Vector3.forward * dir.ToAngle();

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

        private void SetSpeed(float speed)
        {
            if (!started) return;

            currentSpeed = speed;
            rigidBody.velocity = currentSpeed * currentDirection.ToVector2();
        }
    }
}
