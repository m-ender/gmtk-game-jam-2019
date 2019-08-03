using DG.Tweening;
using System.Collections.Generic;
using System.Linq;
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

        private int playerCount;
        private List<Bike> bikes;
        private int[] scores;
        private int nextScore;

        private HashSet<int> remainingPlayers;

        private void Start()
        {
            calibrator.PlayerKeysSelected += StartGame;
        }

        private void StartGame(List<KeyCode> playersKeys)
        {
            playerCount = playersKeys.Count;

            Destroy(calibrator.gameObject);

            countdown = 3;
            countdownText.gameObject.SetActive(true);
            countdownText.text = countdown.ToString();

            remainingPlayers = new HashSet<int>();
            bikes = new List<Bike>();
            scores = new int[playerCount];
            nextScore = 0;

            for (int i = 0; i < playerCount; ++i)
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
            scores[player] += nextScore;

            if (remainingPlayers.Count < 2)
            {
                if (remainingPlayers.Count == 1)
                    scores[remainingPlayers.ToArray()[0]] += playerCount - 1;

                for (int i = 0; i < playerCount; ++i)
                    steeringWheels[i].SetScore(scores[i]);
            }
        }

        private void Update()
        {
            if (remainingPlayers != null)
                nextScore = playerCount - remainingPlayers.Count;
        }
    }

}
