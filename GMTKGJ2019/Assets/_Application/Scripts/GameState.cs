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
        private SteeringWheel[] steeringWheelPrefabs = null;

        [SerializeField]
        private GameObject[] playerObjects = null;

        private SteeringWheel[] steeringWheels = null;
        private HashSet<int> remainingPlayers;

        private void Start()
        {
            calibrator.PlayerKeysSelected += StartGame;
        }

        private void StartGame(List<KeyCode> playersKeys)
        {
            Destroy(calibrator.gameObject);

            remainingPlayers = new HashSet<int>();
            steeringWheels = new SteeringWheel[playersKeys.Count];

            for (int i = 0; i < playersKeys.Count; ++i)
            {
                remainingPlayers.Add(i);

                steeringWheels[i] = Instantiate(steeringWheelPrefabs[i], canvas.transform);
                var bike = Instantiate(playerObjects[i]).GetComponentInChildren<Bike>();

                int player = i;
                bike.Destroyed += () => OnBikeDestroyed(player);
            }
        }

        private void OnBikeDestroyed(int player)
        {
            remainingPlayers.Remove(player);

            steeringWheels[player].Suspend();

            if (remainingPlayers.Count < 2)
            {
                Debug.Log("congration you done it");
            }
        }
    }

}
