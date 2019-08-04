using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

namespace GMTKGJ2019
{
    [RequireComponent(typeof(AudioSource))]
    public class GameState : MonoBehaviour
    {

        [SerializeField] private CameraShake cameraShake = null;
        [SerializeField] private Calibrator calibrator = null;
        [SerializeField] private Transform arena = null;
        [SerializeField] private GameObject[] playerObjects = null;
        [SerializeField] private PlayerUI[] playerUIs = null;
        [SerializeField] private ResultScreen resultScreen = null;

        [SerializeField] private GameObject[] itemPrefabs = null;
        [SerializeField] private TextMeshProUGUI countdownText = null;

        [Space(10)]

        [SerializeField] private AudioClip countdownSound = null;
        [SerializeField] private AudioClip startGameSound = null;
        [SerializeField] private AudioClip explosionSound = null;
        [SerializeField] private AudioClip itemSpawnSound = null;
        [SerializeField] private AudioClip itemUseSound = null;

        private GameParameters parameters;

        private AudioSource audioSource;

        private int countdown;

        private float timeToNextItem = -1f;

        private int playerCount;
        private int[] scores;
        private int nextScore;
        private List<KeyCode> playerKeys;

        private List<Player> players;
        private List<GameObject> items;

        private HashSet<int> remainingPlayers;

        private bool matchInProgress;

        private int winner;
        private Color winnerColor;

        private string[] playerNames = new string[]
        {
            "Orange",
            "Green",
            "Pink",
            "Blue",
        };

        private void Awake()
        {
            parameters = GameParameters.Instance;
            audioSource = GetComponent<AudioSource>();
            audioSource.volume = parameters.MasterVolume;
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
            players = new List<Player>();
            items = new List<GameObject>();
            nextScore = 0;
            winner = -1;

            float width2 = parameters.ArenaWidth / 2;
            float height2 = parameters.ArenaHeight / 2;
            float dist = parameters.SpawningDistanceFromWall;

            List<Vector2> spawningLocations = new List<Vector2>
            {
                new Vector2(-width2 + dist, height2 - dist),
                new Vector2(width2 - dist, -height2 + dist),
                new Vector2(-width2 + dist, -height2 + dist),
                new Vector2(width2 - dist, height2 - dist),
            };

            for (int i = 0; i < playerCount; ++i)
            {
                remainingPlayers.Add(i);

                var player = Instantiate(
                    playerObjects[i],
                    spawningLocations[i],
                    Quaternion.identity,
                    arena).GetComponentInChildren<Player>();
                player.Initialize(playerKeys[i]);
                players.Add(player);

                int playerId = i;
                player.Destroyed += () => OnPlayerDestroyed(playerId);
                player.ItemCollected += (item) => OnPlayerItemCollected(playerId, item);
            }

            countdown = parameters.MatchCountDown;
            countdownText.gameObject.SetActive(true);
            countdownText.text = countdown.ToString();

            audioSource.PlayOneShot(countdownSound);
            DOTween.Sequence().InsertCallback(1f, AdvanceCountdown).SetLoops(parameters.MatchCountDown);
        }

        private void SpawnItem()
        {
            audioSource.PlayOneShot(itemSpawnSound);

            GameObject item = Instantiate(
                itemPrefabs[Random.RNG.Next(itemPrefabs.Length)],
                GetRandomItemLocation(),
                Quaternion.identity,
                arena);

            var filter = new ContactFilter2D().NoFilter();
            var results = new Collider2D[1];
            for (int i = 0; i < parameters.MaxItemSpawningAttempts; ++i)
            {
                if (item.GetComponent<Collider2D>().OverlapCollider(filter, results) == 0)
                    break;

                item.transform.localPosition = GetRandomItemLocation();
            }

            items.Add(item);
        }

        private Vector2 GetRandomItemLocation()
        {
            float width = parameters.ArenaWidth - 2;
            float height = parameters.ArenaHeight - 2;

            return new Vector2(
                (float)Random.RNG.NextDouble() * width - width / 2,
                (float)Random.RNG.NextDouble() * height - height / 2);
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
                foreach (var bike in players)
                    bike.StartBike();

                matchInProgress = true;
            }
        }

        private void OnPlayerItemCollected(int player, GameObject item)
        {
            audioSource.PlayOneShot(itemUseSound);
            item.GetComponent<Item>().CastEffect(player, players);
            Destroy(item);
            items.Remove(item);
        }

        private void OnPlayerDestroyed(int player)
        {
            audioSource.PlayOneShot(explosionSound);
            cameraShake.TriggerShake();
            remainingPlayers.Remove(player);

            scores[player] += nextScore;

            if (remainingPlayers.Count == 0)
                FinishMatch();
        }

        private void FinishMatch()
        {
            matchInProgress = false;

            for (int i = 0; i < playerCount; ++i)
                playerUIs[i].SetScore(scores[i]);

            foreach (var item in items)
                Destroy(item);

            DisplayResults();
        }

        private void DisplayResults()
        {
            int delay = parameters.NextMatchDelay;
            if (winner > -1)
                resultScreen.DisplayWinner(delay, playerNames[winner], winnerColor);
            else
                resultScreen.DisplayDraw(delay);

            DOTween.Sequence().InsertCallback(delay, () =>
            {
                resultScreen.Hide();
                StartMatch();
            });
        }

        private void Update()
        {
            if (!matchInProgress)
                return;

            timeToNextItem -= Time.deltaTime;

            if (timeToNextItem < 0)
            {
                float min = parameters.MinItemSpawnDelay;
                float max = parameters.MaxItemSpawnDelay;
                timeToNextItem = (float)Random.RNG.NextDouble() * (max - min) + min;

                if (items.Count < parameters.MaxItems)
                    SpawnItem();
            }

            if (remainingPlayers.Count > 1)
            {
                nextScore = playerCount - remainingPlayers.Count;

            }
            else if (remainingPlayers.Count == 1)
            {
                winner = remainingPlayers.ToArray()[0];
                winnerColor = players[winner].playerColor;
                nextScore = playerCount - 1;
                players[winner].DestroyPlayer();
            }
        }
    }
}
