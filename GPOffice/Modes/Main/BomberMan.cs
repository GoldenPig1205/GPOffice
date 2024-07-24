using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CustomRendering;
using Exiled.API.Features;
using Exiled.API.Features.Items;
using MEC;
using Mirror;
using UnityEngine;

namespace GPOffice.Modes
{
    class BomberMan
    {
        public static BomberMan Instance;

        public void OnEnabled()
        {
            Timing.RunCoroutine(OnModeStarted());
        }

        public IEnumerator<float> OnModeStarted()
        {

            while (true)
            {
                foreach (var player in Player.List)
                {
                    int r = UnityEngine.Random.Range(1, 5);

                    if (r == 1)
                    {
                        var g = (ExplosiveGrenade)Item.Create(ItemType.GrenadeHE, Server.Host);
                        g.FuseTime = 3f;
                        g.SpawnActive(player.Position + new Vector3(0f, 0.1f, 0f), Server.Host);
                    }
                    else if (r == 2)
                    {
                        var g = (FlashGrenade)Item.Create(ItemType.GrenadeFlash, Server.Host);
                        g.FuseTime = 3f;
                        g.SpawnActive(player.Position + new Vector3(0f, 0.1f, 0f), Server.Host);
                    }
                    else if (r == 3)
                    {
                        Server.ExecuteCommand($"/drop {player.Id} 31 1");
                    }
                    else if (r == 4)
                    {
                        Server.ExecuteCommand($"/drop {player.Id} {UnityEngine.Random.Range(44, 46)} 1");
                    }
                }

                yield return Timing.WaitForSeconds(10f);
            }
        }
    }
}
