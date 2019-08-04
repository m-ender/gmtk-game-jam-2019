using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace GMTKGJ2019
{
    public class Calibrator : MonoBehaviour
    {
        private static readonly int MaximumPlayers = 4;

        //private static readonly double TotalCalibrationTime = TimeSpan.FromSeconds(2).TotalSeconds;

        [SerializeField] private PlayerUI[] playerUIs = null;

        [SerializeField] private ProgressBar timeLeft = null;

        private double totalCalibrationTime;
        private double calibrationTimeLeft;

        private readonly List<KeyCode> keys = new List<KeyCode>();

        public event Action<List<KeyCode>> PlayerKeysSelected;

        private void Awake()
        {
            totalCalibrationTime = GameParameters.Instance.CalibrationTime;
            calibrationTimeLeft = totalCalibrationTime;
        }

        private void Update()
        {
            foreach (KeyCode key in Enum.GetValues(typeof(KeyCode)))
            {
                if (Input.GetKey(key) && keys.Count < MaximumPlayers)
                {
                    if (keys.IndexOf(key) < 0)
                        keys.Add(key);
                }

                if (Input.GetKeyUp(key))
                {
                    keys.Remove(key);
                }
            }

            if (keys.Count < 2)
                calibrationTimeLeft = totalCalibrationTime;
            else
                calibrationTimeLeft -= Time.deltaTime;

            UpdateUI();

            if (calibrationTimeLeft <= 0f)
                PlayerKeysSelected?.Invoke(new List<KeyCode>(keys));
        }

        private void UpdateUI()
        {
            for (int i = 0; i < keys.Count; ++i)
            {
                playerUIs[i].gameObject.SetActive(true);
                playerUIs[i].SetKeyIndicator(keys[i].ToString());
            }

            for (int i = keys.Count; i < MaximumPlayers; ++i)
                playerUIs[i].gameObject.SetActive(false);

            timeLeft.SetProgress((float)((100f / totalCalibrationTime) * calibrationTimeLeft));
        }
    }
}
