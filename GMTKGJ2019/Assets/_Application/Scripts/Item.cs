using System.Collections.Generic;
using UnityEngine;

namespace GMTKGJ2019
{
    public class Item : MonoBehaviour
    {
        public ItemType Type;

        public void CastEffect(int castingPlayer, List<Player> players)
        {
            for (int i = 0; i < players.Count; ++i)
            {
                if (i == castingPlayer || !players[i])
                    continue;

                var steeringWheel = players[i].SteeringWheel;

                switch (Type)
                {
                case ItemType.Fast:
                    steeringWheel.Fast();
                    break;
                case ItemType.Slow:
                    steeringWheel.Slow();
                    break;
                case ItemType.Freeze:
                    steeringWheel.Freeze();
                    break;
                case ItemType.Reverse:
                    steeringWheel.Reverse();
                    break;
                case ItemType.Disable:
                    steeringWheel.DisableSector((Direction)Random.RNG.Next(1, 5));
                    break;
                default:
                    break;
                }
            }
        }
    }
}

