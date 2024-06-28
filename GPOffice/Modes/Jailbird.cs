using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Exiled.API.Features;
using MEC;
using Mirror;
using UnityEngine;

namespace GPOffice.Modes
{
    class Jailbird
    {
        public static Jailbird Instance;

        public void OnEnabled()
        {
            Timing.RunCoroutine(OnModeStarted());
        }

        public IEnumerator<float> OnModeStarted()
        {
            Timing.CallDelayed(0.1f, () =>
            {
                foreach (var player in Player.List)
                {
                    player.AddItem(ItemType.Jailbird);
                    Server.ExecuteCommand($"/forceeq {player.Id} jailbird");
                }
            });

            yield return 0f;
        }

    }
}
