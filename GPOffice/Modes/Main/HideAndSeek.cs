using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CustomRendering;
using Exiled.API.Features;
using MEC;
using Mirror;
using UnityEngine;

namespace GPOffice.Modes
{
    class HideAndSeek
    {
        public static HideAndSeek Instance;

        public void OnEnabled()
        {
            Respawn.TimeUntilNextPhase = 10000;

            Timing.RunCoroutine(OnModeStarted());
        }

        public IEnumerator<float> OnModeStarted()
        {
            yield return Timing.WaitForSeconds(10f);

            Server.ExecuteCommand($"/mp load HideAndSeek");

            Player Finder = Plugin.GetRandomValue(Player.List.ToList());

            Player.List.ToList().ForEach(x => Server.ExecuteCommand($"/god {x.Id} 1"));

            foreach (var player in Player.List.Where(x => x != Finder))
            {
                player.Role.Set(PlayerRoles.RoleTypeId.ClassD);
                player.Position = new Vector3(40.46484f, -1013.662f, 140.2266f);
            }

            for (int i = 1; i < 10; i++)
            {
                foreach (var player in Player.List)
                {
                    player.ClearBroadcasts();
                    player.Broadcast(2, $"<size=25><b><color=red>{10 - i}초 뒤 술래가 출몰합니다.</color></b></size>");
                }

                yield return Timing.WaitForSeconds(1f);
            }

            int Remaining = Player.List.Count() * 10;

            Finder.Role.Set(PlayerRoles.RoleTypeId.Scp939);
            Finder.Position = new Vector3(40.46484f, -1013.662f, 140.2266f);
            Finder.CurrentRoom.Blackout(Remaining + 1);

            yield return Timing.WaitForSeconds(1f);

            Player.List.ToList().ForEach(x => Server.ExecuteCommand($"/god {x.Id} 0"));

            for (int i = 1; i < Remaining; i++)
            {
                foreach (var player in Player.List)
                {
                    player.ClearBroadcasts();
                    player.Broadcast(2, $"<size=25><b><color=#2EFEF7>{Remaining - i}초 뒤 술래가 패배합니다.</color></b></size>");
                }

                yield return Timing.WaitForSeconds(1f);
            }

            Finder.Kill($"제한 시간 안에 생존자를 전부 죽이지 못했습니다.");
        }
    }
}
