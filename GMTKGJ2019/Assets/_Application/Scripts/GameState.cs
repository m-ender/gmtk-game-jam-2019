using DG.Tweening;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

namespace GMTKGJ2019
{
    [RequireComponent(typeof(AudioSource))]
    public class GameState : MonoBehaviour
    {

        [SerializeField] private Calibrator calibrator = null;
        [SerializeField] private Canvas canvas = null;
        [SerializeField] private SteeringWheel[] steeringWheels = null;
        [SerializeField] private GameObject[] playerObjects = null;
        [SerializeField] private TextMeshProUGUI countdownText = null;

        [SerializeField] private AudioClip CountdownSound = null;
        [SerializeField] private AudioClip StartGameSound = null;

        private AudioSource audioSource;

        private int countdown;

        private int playerCount;
        private List<Bike> bikes;
        private int[] scores;
        private int nextScore;

        private HashSet<int> remainingPlayers;

        private void Awake()
        {
            audioSource = GetComponent<AudioSource>();
        }

        private void Start()
        {
            calibrator.PlayerKeysSelected += StartGame;
        }

        private void StartGame(List<KeyCode> playersKeys)
        {
            playerCount = playersKeys.Count;
            Destroy(calibrator.gameObject);

            scores = new int[playerCount];
            StartMatch();
        }

        private void StartMatch()
        {
            remainingPlayers = new HashSet<int>();
            bikes = new List<Bike>();
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

            countdown = 3;
            countdownText.gameObject.SetActive(true);
            countdownText.text = countdown.ToString();

            audioSource.PlayOneShot(CountdownSound, 1f);
            DOTween.Sequence().InsertCallback(1f, AdvanceCountdown).SetLoops(3);
        }

        private void AdvanceCountdown()
        {
            --countdown;
            if (countdown > 0)
            {
                audioSource.PlayOneShot(CountdownSound, 1f);
                countdownText.text = countdown.ToString();
            }
            else
            {
                audioSource.PlayOneShot(StartGameSound, 1f);
                countdownText.gameObject.SetActive(false);
                foreach (var bike in bikes)
                    bike.StartBike();
            }
        }

        private void OnBikeDestroyed(int player)
        {
            remainingPlayers.Remove(player);

            steeringWheels[player].Suspend();
            scores[player] += nextScore;

            if (remainingPlayers.Count == 0)
            {
                for (int i = 0; i < playerCount; ++i)
                    steeringWheels[i].SetScore(scores[i]);

                StartMatch();
            }
        }

        private void Update()
        {
            if (remainingPlayers == null)
                return;

            if (remainingPlayers.Count > 1)
                nextScore = playerCount - remainingPlayers.Count;
            else if (remainingPlayers.Count == 1)
            {
                int winner = remainingPlayers.ToArray()[0];
                nextScore = playerCount - 1;
                bikes[winner].DestroyPlayer();
            }

        }
    }

}
