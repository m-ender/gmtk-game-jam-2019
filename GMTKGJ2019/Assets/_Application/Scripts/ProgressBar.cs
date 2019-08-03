using System;
using UnityEngine;

namespace GMTKGJ2019
{
    public class ProgressBar : MonoBehaviour
    {

        [SerializeField] private UnityEngine.UI.Image fillImage = null;

        private float fullWidth;

        private float currentWidth;

        public void SetProgress(float percentage)
        {
            currentWidth = Math.Max(0f, (fullWidth / 100) * percentage);
        }

        private void Start()
        {
            fullWidth = fillImage.rectTransform.rect.width;
            currentWidth = fullWidth;
        }

        private void Update()
        {
            var delta = fillImage.rectTransform.sizeDelta;
            fillImage.rectTransform.sizeDelta = new Vector2(currentWidth, delta.y);
        }
    }
}
