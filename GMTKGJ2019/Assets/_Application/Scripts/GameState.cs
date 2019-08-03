using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GMTKGJ2019
{
    public class GameState : MonoBehaviour
    {

        [SerializeField]
        private Calibrator calibrator = null;

        [SerializeField]
        private Canvas canvas = null;

        [SerializeField]
        private SteeringWheel[] steeringWheels = null;

        [SerializeField]
        private GameObject[] playerObjects = null;

        private void Start()
        {
            calibrator.OnPlayersKeysSelected += StartGame;
        }

        private void StartGame(List<KeyCode> playersKeys)
        {
            Destroy(calibrator.gameObject);

            for (int i = 0; i < playersKeys.Count; i++)
            {
                Instantiate(steeringWheels[i], canvas.transform);
                Instantiate(playerObjects[i]);
            }
        }
    }

}
