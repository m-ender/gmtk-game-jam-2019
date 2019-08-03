using UnityEngine;

namespace GMTKGJ2019
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class Bike : MonoBehaviour
    {
        [SerializeField] private float baseSpeed;
        [SerializeField] private float fastModifier;
        [SerializeField] private float slowModifier;

        [SerializeField] private Color playerColor;
        [SerializeField] private PlayerWall wallPrefab;
        [SerializeField] private Direction initialDirection;

        private Rigidbody2D rigidBody;
        private PlayerWall previousWall;
        private PlayerWall currentWall;

        private Direction currentDirection;
        private float currentSpeed;

        private void Awake()
        {
            rigidBody = GetComponent<Rigidbody2D>();
            rigidBody.velocity = new Vector2(0, 5);

            currentSpeed = baseSpeed;
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
            rigidBody.velocity = baseSpeed * dir.ToVector2();

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
        }
    }
}