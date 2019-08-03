﻿using UnityEngine;

namespace GMTKGJ2019
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class PlayerWall : MonoBehaviour
    {
        [SerializeField] private float width = 0f;

        private Vector2 start;
        private bool horizontal;

        public void Initialize(bool horizontal, Vector2 start, Color playerColor)
        {
            this.start = start;
            this.horizontal = horizontal;

            transform.localScale = Vector3.zero;

            GetComponent<SpriteRenderer>().color = playerColor;
        }

        public void UpdateEnd(Vector2 end)
        {
            transform.localScale = horizontal
                ? new Vector3((end - start).magnitude + width, width, 1f)
                : new Vector3(width, (end - start).magnitude + width, 1f);

            transform.localPosition = (start + end) / 2;
        }
    }
}