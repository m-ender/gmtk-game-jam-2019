using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = System.Random;

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

        [SerializeField] private Color disabledColor = Color.white;
        [SerializeField] private Color inactiveColor = Color.white;
        [SerializeField] private Color activeColor = Color.white;
        [SerializeField] private float baseRotationSpeed = 0f;
        [SerializeField] private float fastModifier = 0f;
        [SerializeField] private float slowModifier = 0f;
        [SerializeField] private float speedChangeDuration = 0f;
        [SerializeField] private float freezeDuration = 0f;
        [SerializeField] private float disabledSectorDuration = 0f;

        private float angle;
        private float currentSpeed;
        private bool reverse;
        private Direction disabledSector;
        public Direction CurrentDirection { get; private set; }

        private Tween speedTimer;
        private Tween disabledSectorTimer;

        private Dictionary<Direction, Image> sectorMap;

        public void Reverse()
        {
            reverse = !reverse;
        }

        public void Fast()
        {
            currentSpeed = fastModifier * baseRotationSpeed;
            SetUpSpeedTimer(speedChangeDuration);
        }

        public void Slow()
        {
            speedTimer?.Complete();
            currentSpeed = slowModifier * baseRotationSpeed;
            SetUpSpeedTimer(speedChangeDuration);
        }

        public void Freeze()
        {
            speedTimer?.Complete();
            currentSpeed = 0f;
            SetUpSpeedTimer(freezeDuration);
        }

        public void DisableSector(Direction dir)
        {
            disabledSector = dir;
            SetUpDisabledSectorTimer(disabledSectorDuration);
        }

        public void EnableSectors()
        {
            disabledSector = Direction.None;
        }

        private void Awake()
        {
            angle = 0;
            currentSpeed = baseRotationSpeed;
            reverse = false;
            disabledSector = Direction.None;

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
            angle += Time.deltaTime * currentSpeed * (reverse ? -1 : 1);
            CurrentDirection = AngleToDirection(angle);

            RenderSectors();

            if (Input.GetKeyDown(KeyCode.R))
                Reverse();
            else if (Input.GetKeyDown(KeyCode.D))
                DisableSector((Direction)new Random().Next(1, 5));
            else if (Input.GetKeyDown(KeyCode.F))
                Fast();
            else if (Input.GetKeyDown(KeyCode.S))
                Slow();
            else if (Input.GetKeyDown(KeyCode.Z))
                Freeze();
        }

        private void SetUpSpeedTimer(float timeout)
        {
            speedTimer?.Complete();
            speedTimer = DOTween.Sequence().InsertCallback(
                timeout,
                () => currentSpeed = baseRotationSpeed);
        }

        private void SetUpDisabledSectorTimer(float timeout)
        {
            disabledSectorTimer?.Complete();
            disabledSectorTimer = DOTween.Sequence().InsertCallback(
                timeout,
                () => disabledSector = Direction.None);
        }

        private void RenderSectors()
        {
            hand.localEulerAngles = Vector3.back * angle;

            foreach (var (dir, sector) in sectorMap)
            {
                if (dir == disabledSector)
                    sector.color = disabledColor;
                else
                    sector.color = (dir == CurrentDirection)
                        ? activeColor
                        : inactiveColor;
            }
        }

        private Direction AngleToDirection(float angle)
        {
            Direction result = Direction.North;
            switch ((angle % 360f + 360f) % 360f)
            {
            case float x when (x >= 45f && x < 135f): result = Direction.East; break;
            case float x when (x >= 135f && x < 225f): result = Direction.South; break;
            case float x when (x >= 225f && x < 315f): result = Direction.West; break;
            }

            return result == disabledSector
                ? Direction.None
                : result;
        }
    }
}