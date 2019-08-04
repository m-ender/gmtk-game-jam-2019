using UnityEngine;

namespace GMTKGJ2019
{
    public class QuitOnEscape : MonoBehaviour
    {
        private void Update()
        {
            if (Input.GetKey("escape"))
            {
                Application.Quit();
            }
        }
    }
}
