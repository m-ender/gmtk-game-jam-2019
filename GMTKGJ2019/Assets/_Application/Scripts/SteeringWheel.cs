using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

namespace GMTKGJ2019
{
    public class SteeringWheel : MonoBehaviour
    {
        [SerializeField] private Transform hand = null;
        [SerializeField] private SpriteRenderer northSector = null;
        [SerializeField] private SpriteRenderer eastSector = null;
        [SerializeField] private SpriteRenderer southSector = null;
        [SerializeField] private SpriteRenderer westSector = null;

        [Space(10)]

        [SerializeField] private Direction initialDirection = Direction.None;

        [Space(10)]

        [SerializeField] private Color disabledColor = Color.white;
        [SerializeField] private Color inactiveColor = Color.white;
        [SerializeField] private Color activeColor = Color.white;

        private int tweenID;

        private GameParameters parameters;

        private float angle;
        private float currentSpeed;
        private bool reverse;
        private Direction disabledSector;
        public Direction CurrentDirection { get; private set; }

        private Tween speedTimer;
        private Tween disabledSectorTimer;

        private Dictionary<Direction, SpriteRenderer> sectorMap;

        private bool suspended;

        public void Suspend()
        {
            suspended = true;
        }

        public void Reverse()
        {
            reverse = !reverse;
        }

        public void Fast()
        {
            currentSpeed = parameters.FastRotationModifier * parameters.BaseRotationSpeed;
            SetUpSpeedTimer(parameters.SpeedModifierDuration);
        }

        public void Slow()
        {
            speedTimer?.Complete();
            currentSpeed = parameters.SlowRotationModifier * parameters.BaseRotationSpeed;
            SetUpSpeedTimer(parameters.SpeedModifierDuration);
        }

        public void Freeze()
        {
            speedTimer?.Complete();
            currentSpeed = 0f;
            SetUpSpeedTimer(parameters.FreezeDuration);
        }

        public void AnimateInput()
        {
            sectorMap[CurrentDirection].transform
                .DOLocalMove(
                    parameters.InputBumpStrength * CurrentDirection.ToVector2(),
                    parameters.InputBumpDuration)
                .SetRelative(true)
                .SetLoops(2, LoopType.Yoyo);
        }

        public void DisableSector(Direction dir)
        {
            disabledSector = dir;
            SetUpDisabledSectorTimer(parameters.DisableSectorDuration);
        }

        public void EnableSectors()
        {
            disabledSector = Direction.None;
        }

        private void Awake()
        {
            sectorMap = new Dictionary<Direction, SpriteRenderer>
            {
                { Direction.North, northSector },
                { Direction.East, eastSector },
                { Direction.South, southSector },
                { Direction.West, westSector },
            };

            tweenID = 100000 + (int)initialDirection;

            parameters = GameParameters.Instance;

            angle = initialDirection.ToAngle();
            currentSpeed = parameters.BaseRotationSpeed;
            reverse = false;
            disabledSector = Direction.None;

            CurrentDirection = AngleToDirection(angle);

            RenderSectors();
        }

        private void OnDestroy()
        {
            speedTimer?.Complete();
            disabledSectorTimer?.Complete();
            foreach (var sector in sectorMap.Values)
                DOTween.Complete(sector.transform);
        }

        private void Update()
        {
            if (suspended) return;

            angle += Time.deltaTime * currentSpeed * (reverse ? 1 : -1);
            Direction oldDir = CurrentDirection;
            CurrentDirection = AngleToDirection(angle);

            UpdateSectors(oldDir);
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

        private void UpdateSectors(Direction oldDir)
        {
            hand.localEulerAngles = Vector3.forward * angle;

            if (oldDir != CurrentDirection)
            {
                sectorMap[oldDir].DOColor(inactiveColor, parameters.SectorFadeDuration);
                sectorMap[CurrentDirection].DOColor(activeColor, parameters.SectorFadeDuration);
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