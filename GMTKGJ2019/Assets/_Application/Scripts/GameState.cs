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
        [SerializeField] private Transform arena = null;
        [SerializeField] private SteeringWheel[] steeringWheels = null;
        [SerializeField] private GameObject[] playerObjects = null;

        [SerializeField] private GameObject[] itemPrefabs = null;
        [SerializeField] private TextMeshProUGUI countdownText = null;

        [Space(10)]

        [SerializeField] private AudioClip CountdownSound = null;
        [SerializeField] private AudioClip StartGameSound = null;
        [SerializeField] private AudioClip ExplosionSound = null;

        private AudioSource audioSource;

        private int countdown;

        private int playerCount;
        private int[] scores;
        private int nextScore;
        private List<KeyCode> playerKeys;

        private List<Bike> bikes;
        private List<GameObject> items;

        private HashSet<int> remainingPlayers;

        private void Awake()
        {
            audioSource = GetComponent<AudioSource>();
        }

        private void Start()
        {
            calibrator.PlayerKeysSelected += StartGame;
        }

        private void StartGame(List<KeyCode> playerKeys)
        {
            this.playerKeys = playerKeys;
            playerCount = playerKeys.Count;
            Destroy(calibrator.gameObject);

            scores = new int[playerCount];
            StartMatch();
        }

        private void StartMatch()
        {
            remainingPlayers = new HashSet<int>();
            bikes = new List<Bike>();
            items = new List<GameObject>();
            nextScore = 0;

            for (int i = 0; i < playerCount; ++i)
            {
                remainingPlayers.Add(i);

                steeringWheels[i].Resume();
                var bike = Instantiate(playerObjects[i], arena).GetComponentInChildren<Bike>();
                bike.Initialize(playerKeys[i], steeringWheels[i]);
                bikes.Add(bike);

                int player = i;
                bike.Destroyed += () => OnBikeDestroyed(player);
                bike.ItemCollected += (item) => OnBikeItemCollected(player, item);
            }

            items.Add(Instantiate(itemPrefabs[Random.RNG.Next(itemPrefabs.Length)], arena));

            countdown = 3;
            countdownText.gameObject.SetActive(true);
            countdownText.text = countdown.ToString();

            audioSource.PlayOneShot(CountdownSound);
            DOTween.Sequence().InsertCallback(1f, AdvanceCountdown).SetLoops(3);
        }

        private void AdvanceCountdown()
        {
            --countdown;
            if (countdown > 0)
            {
                audioSource.PlayOneShot(CountdownSound);
                countdownText.text = countdown.ToString();
            }
            else
            {
                audioSource.PlayOneShot(StartGameSound);
                countdownText.gameObject.SetActive(false);
                foreach (var bike in bikes)
                    bike.StartBike();
            }
        }

        private void OnBikeItemCollected(int player, GameObject item)
        {
            item.GetComponent<Item>().CastEffect(player, steeringWheels);
            Destroy(item);
            items.Remove(item);
        }

        private void OnBikeDestroyed(int player)
        {
            audioSource.PlayOneShot(ExplosionSound);
            remainingPlayers.Remove(player);

            steeringWheels[player].Suspend();
            scores[player] += nextScore;

            if (remainingPlayers.Count == 0)
                FinishMatch();
        }

        private void FinishMatch()
        {
            for (int i = 0; i < playerCount; ++i)
                steeringWheels[i].SetScore(scores[i]);

            foreach (var item in items)
                Destroy(item);

            StartMatch();
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
                // bikes[winner].DestroyPlayer();
            }
        }
    }
}
