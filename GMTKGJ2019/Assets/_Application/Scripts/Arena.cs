using UnityEngine;

namespace GMTKGJ2019
{
    public class Arena : MonoBehaviour
    {
        [SerializeField] private Transform northWall = null;
        [SerializeField] private Transform westWall = null;
        [SerializeField] private Transform southWall = null;
        [SerializeField] private Transform eastWall = null;
        [SerializeField] private Transform floor = null;

        private void Awake()
        {
            float width = GameParameters.Instance.ArenaWidth;
            float height = GameParameters.Instance.ArenaHeight;

            floor.localScale = new Vector2(width, height);

            northWall.localScale = new Vector2(width + 2, 1);
            northWall.localPosition = new Vector2(0, height / 2 + 0.5f);

            southWall.localScale = new Vector2(width + 2, 1);
            southWall.localPosition = new Vector2(0, -height / 2 - 0.5f);

            eastWall.localScale = new Vector2(1, height + 2);
            eastWall.localPosition = new Vector2(width / 2 + 0.5f, 0);

            westWall.localScale = new Vector2(1, height + 2);
            westWall.localPosition = new Vector2(-width / 2 - 0.5f, 0);
        }
    }
}