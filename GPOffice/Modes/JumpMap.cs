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
    class JumpMap
    {
        public void OnEnabled()
        {
            Server.FriendlyFire = true;
            Round.IsLocked = true;
            Respawn.TimeUntilNextPhase = 10000;

            Timing.CallDelayed(1f, () =>
            {
                Timing.RunCoroutine(OnModeStarted());

                site02.site02 site02 = new site02.site02();

                site02.OnEnabled();
                site02.OnRoundStarted();

                foreach (var player in Player.List)
                    site02.Verified(player);
            });
        }

        public IEnumerator<float> OnModeStarted()
        {
            while (true)
            {
                foreach (var player in Player.List)
                {
                    if (Physics.Raycast(player.Position, Vector3.down, out RaycastHit hit, 5, (LayerMask)1))
                    {
                        if (hit.transform.name == "Stage 7")
                        {
                            Player.List.ToList().ForEach(x => x.Broadcast(15, $"<size=25><color=yellow>{player.DisplayNickname}</color>(이)가 Stage 7에 도달했습니다!</size>"));
                            Round.IsLocked = false;
                            break;
                        }
                    }
                }

                yield return Timing.WaitForSeconds(1f);
            }
        }
    }
}
