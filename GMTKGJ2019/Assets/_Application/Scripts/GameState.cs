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
        [SerializeField] private int MaxItems = 0;
        [SerializeField] private Calibrator calibrator = null;
        [SerializeField] private Transform arena = null;
        [SerializeField] private SteeringWheel[] steeringWheels = null;
        [SerializeField] private GameObject[] playerObjects = null;

        [SerializeField] private GameObject[] itemPrefabs = null;
        [SerializeField] private TextMeshProUGUI countdownText = null;

        [Space(10)]

        [SerializeField] private AudioClip countdownSound = null;
        [SerializeField] private AudioClip startGameSound = null;
        [SerializeField] private AudioClip explosionSound = null;
        [SerializeField] private AudioClip itemSpawnSound = null;
        [SerializeField] private AudioClip itemUseSound = null;

        private AudioSource audioSource;

        private int countdown;

        private float timeToNextItem = -1f;

        private int playerCount;
        private int[] scores;
        private int nextScore;
        private List<KeyCode> playerKeys;

        private List<Bike> bikes;
        private List<GameObject> items;

        private HashSet<int> remainingPlayers;

        private bool matchInProgress;

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

            countdown = 3;
            countdownText.gameObject.SetActive(true);
            countdownText.text = countdown.ToString();

            audioSource.PlayOneShot(countdownSound);
            DOTween.Sequence().InsertCallback(1f, AdvanceCountdown).SetLoops(3);
        }

        private void SpawnItem()
        {
            System.Random rng = Random.RNG;
            audioSource.PlayOneShot(itemSpawnSound);
            items.Add(Instantiate(
                itemPrefabs[rng.Next(itemPrefabs.Length)],
                new Vector3((float)rng.NextDouble()*8f - 4f, (float)rng.NextDouble() * 8f - 4f, 0),
                Quaternion.identity,
                arena));
        }

        private void AdvanceCountdown()
        {
            --countdown;
            if (countdown > 0)
            {
                audioSource.PlayOneShot(countdownSound);
                countdownText.text = countdown.ToString();
            }
            else
            {
                audioSource.PlayOneShot(startGameSound);
                countdownText.gameObject.SetActive(false);
                foreach (var bike in bikes)
                    bike.StartBike();

                matchInProgress = true;
            }
        }

        private void OnBikeItemCollected(int player, GameObject item)
        {
            audioSource.PlayOneShot(itemUseSound);
            item.GetComponent<Item>().CastEffect(player, steeringWheels);
            Destroy(item);
            items.Remove(item);
        }

        private void OnBikeDestroyed(int player)
        {
            audioSource.PlayOneShot(explosionSound);
            remainingPlayers.Remove(player);

            steeringWheels[player].Suspend();
            scores[player] += nextScore;

            if (remainingPlayers.Count == 0)
                FinishMatch();
        }

        private void FinishMatch()
        {
            matchInProgress = false;

            for (int i = 0; i < playerCount; ++i)
                steeringWheels[i].SetScore(scores[i]);

            foreach (var item in items)
                Destroy(item);

            StartMatch();
        }

        private void Update()
        {
            if (!matchInProgress)
                return;

            timeToNextItem -= Time.deltaTime;

            if (timeToNextItem < 0)
            {
                timeToNextItem = (float)Random.RNG.NextDouble() * 2 + 1;

                if (items.Count < MaxItems)
                    SpawnItem();
            }

            if (remainingPlayers.Count > 1)
            {
                nextScore = playerCount - remainingPlayers.Count;

            }
            else if (remainingPlayers.Count == 1)
            {
                int winner = remainingPlayers.ToArray()[0];
                nextScore = playerCount - 1;
                // bikes[winner].DestroyPlayer();
            }
        }
    }
}
