using UnityEngine;

namespace GMTKGJ2019
{
    [RequireComponent(typeof(Camera))]
    public class CameraParameters : MonoBehaviour
    {
        private void Awake()
        {
            GetComponent<Camera>().orthographicSize = GameParameters.Instance.VerticalWorldSpaceUnits / 2;
        }
    }
}