using System;
using TMPro;
using UnityEngine;

namespace GMTKGJ2019
{
    public class TestScript : MonoBehaviour
    {
        [SerializeField] private TextMeshPro text;

        private void Update()
        {
            int buttonCount = 0;
            foreach (KeyCode key in Enum.GetValues(typeof(KeyCode)))
            {
                if (Input.GetKey(key))
                    ++buttonCount;
            }

            text.text = buttonCount.ToString();
        }
    }
}