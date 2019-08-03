using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace GMTKGJ2019
{
    public class Calibrator : MonoBehaviour
    {
        private static readonly double TotalCalibrationTime = TimeSpan.FromSeconds(5).TotalSeconds;

        [SerializeField] private TextMeshProUGUI playersText = null;

        [SerializeField] private ProgressBar timeLeft = null;

        private bool isCalibrationRunning = false;
        private double calibrationTimeLeft = TotalCalibrationTime;

        private readonly List<KeyCode> keys = new List<KeyCode>();

        public List<KeyCode> GetPlayersKeys()
        {
            return new List<KeyCode>(keys);
        }

        private void Update()
        {
            if (Input.anyKeyDown)
            {
                isCalibrationRunning = true;
            }

            if (!isCalibrationRunning)
            {
                return;
            }

            calibrationTimeLeft -= Time.deltaTime;

            if (calibrationTimeLeft > 0f)
            {
                foreach (KeyCode key in Enum.GetValues(typeof(KeyCode)))
                {
                    if (Input.GetKeyDown(key))
                    {
                        keys.Add(key);
                    }

                    if (Input.GetKeyUp(key))
                    {
                        keys.Remove(key);
                    }
                }

                if (keys.Count == 0)
                {
                    isCalibrationRunning = false;
                }

                timeLeft.SetProgress((float)((100f / TotalCalibrationTime) * calibrationTimeLeft));
                playersText.text = String.Join(" ", keys);
            }
            else
            {
                timeLeft.SetProgress(0f);
            }
        }
    }
}
