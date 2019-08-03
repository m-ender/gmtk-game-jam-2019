using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GMTKGJ2019
{
    public class Item : MonoBehaviour
    {

        public void CastEffect(int castingPlayer, SteeringWheel[] steeringWheels)
        {
            steeringWheels[castingPlayer].Freeze();
        }
    }
}

