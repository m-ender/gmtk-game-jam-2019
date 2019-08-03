using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GMTKGJ2019
{
    public class SteeringWheel : MonoBehaviour
    {
        [SerializeField] private RectTransform hand = null;
        [SerializeField] private Image northSector = null;
        [SerializeField] private Image eastSector = null;
        [SerializeField] private Image southSector = null;
        [SerializeField] private Image westSector = null;

        [Space(10)]

        [SerializeField] private Color inactiveColor = Color.white;
        [SerializeField] private Color activeColor = Color.white;
        [SerializeField] private float baseRotationSpeed = 0f;
        [SerializeField] private float fastModifier = 0f;
        [SerializeField] private float slowModifier = 0f;

        private float angle;
        private float currentSpeed;
        public Direction CurrentDirection { get; private set; }

        private Dictionary<Direction, Image> sectorMap;

        private void Awake()
        {
            angle = 0;
            currentSpeed = baseRotationSpeed;

            sectorMap = new Dictionary<Direction, Image>
            {
                { Direction.North, northSector },
                { Direction.East, eastSector },
                { Direction.South, southSector },
                { Direction.West, westSector },
            };

            RenderSectors();
        }

        private void Update()
        {
            angle += Time.deltaTime * currentSpeed;
            CurrentDirection = AngleToSector(angle);

            RenderSectors();
        }

        private void RenderSectors()
        {
            hand.localEulerAngles = Vector3.back * angle;

            foreach (var (dir, sector) in sectorMap)
                sector.color = (dir == CurrentDirection) ? activeColor : inactiveColor;
        }

        private Direction AngleToSector(float angle)
        {
            switch ((angle % 360f + 360f) % 360f)
            {
            case float x when (x >= 45f && x < 135f): return Direction.East;
            case float x when (x >= 135f && x < 225f): return Direction.South;
            case float x when (x >= 225f && x < 315f): return Direction.West;
            default: return Direction.North;
            }
        }
    }
}