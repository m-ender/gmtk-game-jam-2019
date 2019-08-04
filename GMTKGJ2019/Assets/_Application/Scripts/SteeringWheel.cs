using DG.Tweening;
using System;
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

        [SerializeField] private ItemEffect fastEffectPrefab = null;
        [SerializeField] private ItemEffect slowEffectPrefab = null;
        [SerializeField] private ItemEffect freezeEffectPrefab = null;
        [SerializeField] private ItemEffect cwEffectPrefab = null;
        [SerializeField] private ItemEffect ccwEffectPrefab = null;
        [SerializeField] private ItemEffect disableEffectPrefab = null;

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
        private Direction currentDirection;

        public Direction SelectedDirection
            => currentDirection == disabledSector
                ? Direction.None
                : currentDirection;

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

            StartItemEffect(reverse ? ccwEffectPrefab : cwEffectPrefab);
        }

        public void Fast()
        {
            currentSpeed = parameters.FastRotationModifier * parameters.BaseRotationSpeed;
            SetUpSpeedTimer(parameters.SpeedModifierDuration);

            StartItemEffect(fastEffectPrefab);
        }

        public void Slow()
        {
            speedTimer?.Complete();
            currentSpeed = parameters.SlowRotationModifier * parameters.BaseRotationSpeed;
            SetUpSpeedTimer(parameters.SpeedModifierDuration);

            StartItemEffect(slowEffectPrefab);
        }

        public void Freeze()
        {
            speedTimer?.Complete();
            currentSpeed = 0f;
            SetUpSpeedTimer(parameters.FreezeDuration);

            StartItemEffect(freezeEffectPrefab);
        }

        public void DisableSector(Direction dir)
        {
            disabledSector = dir;
            DOTween.Complete(sectorMap[disabledSector]);
            sectorMap[disabledSector].color = disabledColor;

            SetUpDisabledSectorTimer(parameters.DisableSectorDuration);

            StartItemEffect(disableEffectPrefab, disabledSector.ToAngle());
        }

        public void AnimateInput()
        {
            sectorMap[SelectedDirection].transform
                .DOLocalMove(
                    parameters.InputBumpStrength * SelectedDirection.ToVector2(),
                    parameters.InputBumpDuration)
                .SetRelative(true)
                .SetLoops(2, LoopType.Yoyo);
        }

        public void Explode()
        {
            var seq = DOTween.Sequence();
            foreach (var (dir, sector) in sectorMap)
            {
                seq.Join(sector.transform.DOLocalMove(dir.ToVector2() * parameters.WheelExplosionDistance, parameters.WheelExplosionDuration));
                seq.Join(sector.transform.DORotate(Vector3.forward * 360f * parameters.WheelExplosionRotations, parameters.WheelExplosionDuration, RotateMode.FastBeyond360));
            }
            seq.AppendCallback(() => Destroy(gameObject));
        }

        private void EnableSectors()
        {
            if (disabledSector == currentDirection)
                sectorMap[disabledSector].color = activeColor;
            else
                sectorMap[disabledSector].color = inactiveColor;

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

            currentDirection = AngleToDirection(angle);

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
            Direction oldDir = currentDirection;
            currentDirection = AngleToDirection(angle);

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
                    sector.color = (dir == currentDirection)
                        ? activeColor
                        : inactiveColor;
            }
        }

        private void UpdateSectors(Direction oldDir)
        {
            hand.localEulerAngles = Vector3.forward * angle;

            if (oldDir != currentDirection)
            {
                if (oldDir != disabledSector)
                    sectorMap[oldDir].DOColor(inactiveColor, parameters.SectorFadeDuration);

                if (currentDirection != disabledSector)
                    sectorMap[currentDirection].DOColor(activeColor, parameters.SectorFadeDuration);
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

            return result;
        }

        private void StartItemEffect(ItemEffect effectPrefab)
            => StartItemEffect(effectPrefab, 0f);

        private void StartItemEffect(ItemEffect effectPrefab, float angle)
        {
            var effect = Instantiate(effectPrefab, transform);
            effect.transform.localPosition = Vector2.zero;
            effect.transform.localEulerAngles = Vector3.forward * angle;
        }
    }
}