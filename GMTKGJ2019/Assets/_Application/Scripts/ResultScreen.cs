using TMPro;
using UnityEngine;

namespace GMTKGJ2019
{
    public class ResultScreen : MonoBehaviour
    {
        [SerializeField] private GameObject winnerDisplay = null;
        [SerializeField] private GameObject drawDisplay = null;
        [SerializeField] private TextMeshProUGUI winnerText = null;

        public void DisplayWinner(string name, Color playerColor)
        {
            gameObject.SetActive(true);
            winnerDisplay.SetActive(true);
            winnerText.text = name;
            winnerText.color = playerColor;
            drawDisplay.SetActive(false);
        }

        public void DisplayDraw()
        {
            gameObject.SetActive(true);
            winnerDisplay.SetActive(false);
            drawDisplay.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}