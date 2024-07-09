using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
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
    class OnlyOneHuman
    {
        public static OnlyOneHuman Instance;

        public void OnEnabled()
        {
            Respawn.TimeUntilNextPhase = 10000;

            Timing.RunCoroutine(OnModeStarted());

            Exiled.Events.Handlers.Player.Died += OnDied;
            Exiled.Events.Handlers.Player.Escaping += OnEscaping;
        }

        public IEnumerator<float> OnModeStarted()
        {
            yield return Timing.WaitForSeconds(1f);

            Player One = GPOffice.GetRandomValue(Player.List.ToList());

            foreach (Player player in Player.List)
            {
                if (player == One)
                    player.Role.Set(PlayerRoles.RoleTypeId.ClassD);
                else
                {
                    List<PlayerRoles.RoleTypeId> Scps = new List<PlayerRoles.RoleTypeId>
                    {
                        PlayerRoles.RoleTypeId.Scp049, PlayerRoles.RoleTypeId.Scp173, PlayerRoles.RoleTypeId.Scp096, 
                        PlayerRoles.RoleTypeId.Scp079, PlayerRoles.RoleTypeId.Scp0492, PlayerRoles.RoleTypeId.Scp3114
                    };

                    player.Role.Set(GPOffice.GetRandomValue(Scps));
                }
            }
        }

        public void OnDied(Exiled.Events.EventArgs.Player.DiedEventArgs ev)
        {
            if (ev.Attacker != null && ev.Attacker.IsScp && ev.Player.Role.Type == PlayerRoles.RoleTypeId.ClassD)
                Player.List.ToList().ForEach(x => x.Broadcast(15, $"{ev.Attacker.DisplayNickname}(이)가 마지막 킬을 달성하였습니다!"));
        }

        public void OnEscaping(Exiled.Events.EventArgs.Player.EscapingEventArgs ev)
        {
            if (ev.Player.Role.Type == PlayerRoles.RoleTypeId.ClassD)
            {
                foreach (var player in Player.List)
                {
                    player.Broadcast(15, $"{ev.Player.DisplayNickname}(이)가 탈출에 성공하였습니다!");
                    if (player.IsScp)
                        player.Kill("죄수를 내보내면 안 됩니다!");
                };
            }
        }
    }
}
