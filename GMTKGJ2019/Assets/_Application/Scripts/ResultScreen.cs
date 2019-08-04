using TMPro;
using UnityEngine;

namespace GMTKGJ2019
{
    public class ResultScreen : MonoBehaviour
    {
        [SerializeField] private GameObject winnerDisplay = null;
        [SerializeField] private GameObject drawDisplay = null;
        [SerializeField] private TextMeshProUGUI winnerText = null;
        [SerializeField] private ProgressBar timeLeftProgressBar = null;

        private double totalTime, timeLeft = 1f;

        public void DisplayWinner(float timeout, string name, Color playerColor)
        {
            SetupTimeout(timeout);
            gameObject.SetActive(true);
            winnerDisplay.SetActive(true);
            winnerText.text = name;
            winnerText.color = playerColor;
            drawDisplay.SetActive(false);
        }

        public void DisplayDraw(float timeout)
        {
            SetupTimeout(timeout);
            gameObject.SetActive(true);
            winnerDisplay.SetActive(false);
            drawDisplay.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        private void Update()
        {
            timeLeft -= Time.deltaTime;
            timeLeftProgressBar.SetProgress((float)((100f / totalTime) * timeLeft));
        }

        private void SetupTimeout(float timeout)
        {
            totalTime = timeout;
            timeLeft = totalTime;
        }
    }
}
