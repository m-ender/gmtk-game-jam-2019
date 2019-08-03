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

        private HashSet<int> remainingPlayers;

        private void Start()
        {
            calibrator.PlayerKeysSelected += StartGame;
        }

        private void StartGame(List<KeyCode> playersKeys)
        {
            Destroy(calibrator.gameObject);

            remainingPlayers = new HashSet<int>();

            for (int i = 0; i < playersKeys.Count; ++i)
            {
                remainingPlayers.Add(i);

                Instantiate(steeringWheels[i], canvas.transform);
                var bike = Instantiate(playerObjects[i]).GetComponentInChildren<Bike>();
                int player = i;
                bike.Destroyed += () => OnBikeDestroyed(player);
            }
        }

        private void OnBikeDestroyed(int player)
        {
            remainingPlayers.Remove(player);

            if (remainingPlayers.Count < 2)
            {
                Debug.Log("congration you done it");
            }
        }
    }

}
