using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace GMTKGJ2019
{
    public class Calibrator : MonoBehaviour
    {
        private static readonly int MaximumPlayers = 4;

        private static readonly double TotalCalibrationTime = TimeSpan.FromSeconds(2).TotalSeconds;

        [SerializeField] private SteeringWheel[] steeringWheels = null;

        [SerializeField] private ProgressBar timeLeft = null;

        private bool isCalibrationRunning = false;
        private double calibrationTimeLeft = TotalCalibrationTime;

        private readonly List<KeyCode> keys = new List<KeyCode>();

        public event Action<List<KeyCode>> PlayerKeysSelected;

        private void UpdateUI()
        {
            timeLeft.SetProgress((float)((100f / TotalCalibrationTime) * calibrationTimeLeft));
        }

        private void Update()
        {
            if (Input.anyKeyDown)
            {
                isCalibrationRunning = true;
            }

            if (!isCalibrationRunning)
            {
                calibrationTimeLeft = TotalCalibrationTime;
                UpdateUI();
                return;
            }

            calibrationTimeLeft -= Time.deltaTime;
            if (calibrationTimeLeft > 0f)
            {
                foreach (KeyCode key in Enum.GetValues(typeof(KeyCode)))
                {
                    if (Input.GetKey(key) && keys.Count < MaximumPlayers)
                    {
                        if (keys.IndexOf(key) < 0)
                        {
                            keys.Add(key);
                        }
                    }

                    if (Input.GetKeyUp(key))
                    {
                        keys.Remove(key);
                    }
                }

                for (int i = 0; i < keys.Count; ++i)
                {
                    steeringWheels[i].gameObject.SetActive(true);
                    steeringWheels[i].SetKeyIndicator(keys[i].ToString());
                }

                for (int i = keys.Count; i < MaximumPlayers; ++i)
                    steeringWheels[i].gameObject.SetActive(false);

                if (keys.Count == 0)
                {
                    isCalibrationRunning = false;
                }

                UpdateUI();
            }
            else
            {
                PlayerKeysSelected?.Invoke(new List<KeyCode>(keys));
            }
        }
    }
}
