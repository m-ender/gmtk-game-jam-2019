using DG.Tweening;
using System.Collections.Generic;
using TMPro;
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
        [SerializeField] private CanvasGroup wheelUI = null;
        [SerializeField] private TextMeshProUGUI keyIndicator = null;
        [SerializeField] private TextMeshProUGUI scoreIndicator = null;

        [Space(10)]

        [SerializeField] private Direction initialDirection = Direction.None;

        [Space(10)]

        [SerializeField] private Color disabledColor = Color.white;
        [SerializeField] private Color inactiveColor = Color.white;
        [SerializeField] private Color activeColor = Color.white;

        private GameParameters parameters;

        private bool suspended = true;

        private float angle;
        private float currentSpeed;
        private bool reverse;
        private Direction disabledSector;
        public Direction CurrentDirection { get; private set; }

        private Tween speedTimer;
        private Tween disabledSectorTimer;

        private Dictionary<Direction, Image> sectorMap;

        public void SetKeyIndicator(string key)
        {
            keyIndicator.text = key;
        }

        public void SetScore(int score)
        {
            scoreIndicator.text = score.ToString();
        }

        public void Reset()
        {
            angle = initialDirection.ToAngle();
            currentSpeed = parameters.BaseRotationSpeed;
            reverse = false;
            disabledSector = Direction.None;

            CurrentDirection = AngleToDirection(angle);

            Resume();
            RenderSectors();
        }

        public void Resume()
        {
            wheelUI.alpha = 1f;

            suspended = false;
        }

        public void Suspend()
        {
            speedTimer?.Complete(true);
            disabledSectorTimer?.Complete(true);

            wheelUI.alpha = 0.2f;

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

            currentSpeed = parameters.FastRotationModifier * parameters.BaseRotationSpeed;
            SetUpSpeedTimer(parameters.SpeedModifierDuration);
        }

        public void Slow()
        {
            if (suspended) return;

            speedTimer?.Complete();
            currentSpeed = parameters.SlowRotationModifier * parameters.BaseRotationSpeed;
            SetUpSpeedTimer(parameters.SpeedModifierDuration);
        }

        public void Freeze()
        {
            if (suspended) return;

            speedTimer?.Complete();
            currentSpeed = 0f;
            SetUpSpeedTimer(parameters.FreezeDuration);
        }

        public void DisableSector(Direction dir)
        {
            if (suspended) return;

            disabledSector = dir;
            SetUpDisabledSectorTimer(parameters.DisableSectorDuration);
        }

        public void EnableSectors()
        {
            disabledSector = Direction.None;
        }

        private void Awake()
        {
            sectorMap = new Dictionary<Direction, Image>
            {
                { Direction.North, northSector },
                { Direction.East, eastSector },
                { Direction.South, southSector },
                { Direction.West, westSector },
            };

            parameters = GameParameters.Instance;

            Reset();
            Suspend();
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
                () => currentSpeed = parameters.BaseRotationSpeed);
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