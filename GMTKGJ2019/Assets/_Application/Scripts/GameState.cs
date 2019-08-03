using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace GMTKGJ2019
{
    public class GameState : MonoBehaviour
    {

        [SerializeField] private Calibrator calibrator = null;
        [SerializeField] private Canvas canvas = null;
        [SerializeField] private SteeringWheel[] steeringWheels = null;
        [SerializeField] private GameObject[] playerObjects = null;
        [SerializeField] private TextMeshProUGUI countdownText = null;

        private int countdown;

        private List<Bike> bikes;

        private HashSet<int> remainingPlayers;

        private void Start()
        {
            calibrator.PlayerKeysSelected += StartGame;
        }

        private void StartGame(List<KeyCode> playersKeys)
        {
            Destroy(calibrator.gameObject);

            countdown = 3;
            countdownText.gameObject.SetActive(true);
            countdownText.text = countdown.ToString();

            remainingPlayers = new HashSet<int>();
            bikes = new List<Bike>();

            for (int i = 0; i < playersKeys.Count; ++i)
            {
                remainingPlayers.Add(i);

                steeringWheels[i].Resume();
                var bike = Instantiate(playerObjects[i]).GetComponentInChildren<Bike>();
                bikes.Add(bike);

                int player = i;
                bike.Destroyed += () => OnBikeDestroyed(player);
            }

            DOTween.Sequence().InsertCallback(1f, () =>
            {
                --countdown;
                if (countdown > 0)
                {
                    countdownText.text = countdown.ToString();
                }
                else
                {
                    Destroy(countdownText.gameObject);
                    foreach (var bike in bikes)
                        bike.StartBike();
                }
            }).SetLoops(3);
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
