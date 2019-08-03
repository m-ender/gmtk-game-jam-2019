using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = System.Random;

namespace GMTKGJ2019
{
    [RequireComponent(typeof(CanvasGroup))]
    public class SteeringWheel : MonoBehaviour
    {
        [SerializeField] private RectTransform hand = null;
        [SerializeField] private Image northSector = null;
        [SerializeField] private Image eastSector = null;
        [SerializeField] private Image southSector = null;
        [SerializeField] private Image westSector = null;

        [Space(10)]

        [SerializeField] private Direction initialDirection = Direction.None;

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

        private CanvasGroup canvasGroup;

        private bool suspended;

        private float angle;
        private float currentSpeed;
        private bool reverse;
        private Direction disabledSector;
        public Direction CurrentDirection { get; private set; }

        private Tween speedTimer;
        private Tween disabledSectorTimer;

        private Dictionary<Direction, Image> sectorMap;

        public void Reset()
        {
            angle = initialDirection.ToAngle();
            currentSpeed = baseRotationSpeed;
            reverse = false;
            disabledSector = Direction.None;

            Resume();
            RenderSectors();
        }

        public void Resume()
        {
            canvasGroup.alpha = 1f;

            suspended = false;
        }

        public void Suspend()
        {
            speedTimer?.Complete(true);
            disabledSectorTimer?.Complete(true);

            canvasGroup.alpha = 0.2f;

            suspended = true;
        }

        public void Reverse()
        {
            if (suspended) return;

            reverse = !reverse;
        }

        public void Fast()
        {
            if (suspended) return;

            currentSpeed = fastModifier * baseRotationSpeed;
            SetUpSpeedTimer(speedChangeDuration);
        }

        public void Slow()
        {
            if (suspended) return;

            speedTimer?.Complete();
            currentSpeed = slowModifier * baseRotationSpeed;
            SetUpSpeedTimer(speedChangeDuration);
        }

        public void Freeze()
        {
            if (suspended) return;

            speedTimer?.Complete();
            currentSpeed = 0f;
            SetUpSpeedTimer(freezeDuration);
        }

        public void DisableSector(Direction dir)
        {
            if (suspended) return;

            disabledSector = dir;
            SetUpDisabledSectorTimer(disabledSectorDuration);
        }

        public void EnableSectors()
        {
            disabledSector = Direction.None;
        }

        private void Awake()
        {
            canvasGroup = GetComponent<CanvasGroup>();

            sectorMap = new Dictionary<Direction, Image>
            {
                { Direction.North, northSector },
                { Direction.East, eastSector },
                { Direction.South, southSector },
                { Direction.West, westSector },
            };

            Reset();
        }

        private void Update()
        {
            if (suspended) return;

            angle += Time.deltaTime * currentSpeed * (reverse ? 1 : -1);
            CurrentDirection = AngleToDirection(angle);

            RenderSectors();
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
            disabledSectorTimer = DOTween.Sequence().InsertCallback(timeout, EnableSectors);
        }

        private void RenderSectors()
        {
            hand.localEulerAngles = Vector3.forward * angle;

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
            case float x when (x >= 45f && x < 135f): result = Direction.West; break;
            case float x when (x >= 135f && x < 225f): result = Direction.South; break;
            case float x when (x >= 225f && x < 315f): result = Direction.East; break;
            }

            return result == disabledSector
                ? Direction.None
                : result;
        }
    }
}