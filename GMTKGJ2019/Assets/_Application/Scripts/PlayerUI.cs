using TMPro;
using UnityEngine;

namespace GMTKGJ2019
{
    public class PlayerUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI keyIndicator = null;
        [SerializeField] private TextMeshProUGUI scoreIndicator = null;
        [SerializeField] private Color playerColor = Color.white;


        public void Awake()
        {
            keyIndicator.color = playerColor;
            scoreIndicator.color = playerColor;
        }

        public void SetKeyIndicator(string key)
        {
            keyIndicator.text = key;
        }

        public void SetScore(int score)
        {
            scoreIndicator.text = score.ToString();
        }
    }
}