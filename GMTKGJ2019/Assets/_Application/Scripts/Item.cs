using UnityEngine;

namespace GMTKGJ2019
{
    public class Item : MonoBehaviour
    {
        public ItemType Type;

        public void CastEffect(int castingPlayer, SteeringWheel[] steeringWheels)
        {
            for (int i = 0; i < steeringWheels.Length; ++i)
            {
                if (i == castingPlayer)
                    continue;

                switch (Type)
                {
                case ItemType.Fast:
                    steeringWheels[i].Fast();
                    break;
                case ItemType.Slow:
                    steeringWheels[i].Slow();
                    break;
                case ItemType.Freeze:
                    steeringWheels[i].Freeze();
                    break;
                case ItemType.Reverse:
                    steeringWheels[i].Reverse();
                    break;
                case ItemType.Disable:
                    steeringWheels[i].DisableSector((Direction)Random.RNG.Next(1, 5));
                    break;
                default:
                    break;
                }
            }
        }
    }
}

