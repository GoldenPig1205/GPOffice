using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using CustomRendering;
using Exiled.API.Features;
using MEC;
using Mirror;
using PlayerRoles.FirstPersonControl;
using UnityEngine;

namespace GPOffice.Modes
{
    class PrisonLife
    {
        Prison_Life.Prison_Life pl = new Prison_Life.Prison_Life();

        public void OnEnabled()
        {
            Timing.RunCoroutine(OnModeStarted());

            Timing.CallDelayed(10f, () =>
            {
                Server.FriendlyFire = true;
                Round.IsLocked = true;
                Respawn.TimeUntilNextPhase = 10000;


                pl.OnEnabled();
                pl.OnWaitingForPlayers();
                pl.OnRoundStarted();

                foreach (var player in Player.List)
                    pl.Verified(player);
            });
        }

        public IEnumerator<float> OnModeStarted()
        {
            yield return Timing.WaitForSeconds(300f);

            foreach (var player in Player.List)
            {
                if (player.IsAlive)
                    Server.ExecuteCommand($"/fc {player.Id} Tutorial 1");
            }
        }
    }
}
