using UnityEngine;

namespace GMTKGJ2019
{
    public class GameParameters : MonoBehaviour
    {
        [Header("World")]
        public float VerticalWorldSpaceUnits;
        public float ArenaWidth;
        public float ArenaHeight;

        [Header("Bike")]
        public float SpawningDistanceFromWall;
        public float BikeBaseSpeed;
        public float BikeFastModifier;
        public float BikeSlowModifier;
        public float BikeBoostDuration;
        public float PlayerWallWidth;

        [Header("Controls")]
        public float BaseRotationSpeed;
        public float FastRotationModifier;
        public float SlowRotationModifier;

        [Header("Items")]
        public int MaxItems;
        public float SpeedModifierDuration;
        public float FreezeDuration;
        public float DisableSectorDuration;
        public float MinItemSpawnDelay;
        public float MaxItemSpawnDelay;
        public int MaxItemSpawningAttempts;

        [Header("Audio")]
        public float MasterVolume;

        [Header("Timeouts")]
        public int MatchCountDown;
        public int NextMatchDelay;
        public float CalibrationTime;

        [Header("Juice")]
        public float CameraShakeDuration;
        public float CameraShakeStrength;
        public int CameraShakeVibrato;
        public float CameraShakeRandomness;
        public float InputBumpStrength;
        public float InputBumpDuration;
        public float SectorFadeDuration;

        public static GameParameters Instance { get; private set; }

        private void Awake()
        {
            Instance = this;
        }
    }
}