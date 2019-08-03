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
        private SteeringWheel steeringWheel = null;

        private void Start()
        {
            calibrator.OnPlayersKeysSelected += StartGame;
        }

        private void StartGame(List<KeyCode> playersKeys)
        {
            Destroy(calibrator.gameObject);

            Vector3[] positions = new Vector3[]
            {
                new Vector3(0, 0, 0),
                new Vector3(500, 0, 0),
                new Vector3(0, -250, 0),
                new Vector3(500, -250, 0)
            };

            for (int i = 0; i < playersKeys.Count; i++)
            {
                var wheel = Instantiate(steeringWheel, canvas.transform);
                wheel.transform.Translate(positions[i]);
            }
        }
    }

}
