using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CustomPlayerEffects;
using CustomRendering;
using Exiled.API.Features;
using MEC;
using Mirror;
using UnityEngine;

namespace GPOffice.Modes
{
    class HIDE
    {
        public static HIDE Instance;

        public List<Player> pl = new List<Player>();
        public Player monster = null;
        public float invisible = 1f;

        public void OnEnabled()
        {
            Respawn.TimeUntilNextPhase = 10000;
            Server.ExecuteCommand($"/el l all");
            Server.ExecuteCommand($"/close **");

            Timing.RunCoroutine(OnModeStarted());

            Exiled.Events.Handlers.Player.Hurting += OnHurting;
        }

        public IEnumerator<float> OnModeStarted()
        {
            Player.List.ToList().CopyTo(pl);
            monster = GPOffice.GetRandomValue(Player.List.ToList());

            foreach (var player in Player.List)
            {
                if (player == monster)
                {
                    player.Role.Set(PlayerRoles.RoleTypeId.Scp3114);
                    Server.ExecuteCommand($"/dtp {player} ESCAPE_PRIMARY");
                    Server.ExecuteCommand($"/open {player} ESCAPE_PRIMARY");
                }
                else
                    player.Role.Set(PlayerRoles.RoleTypeId.NtfCaptain);
            }

            while (true)
            {
                Timing.CallDelayed(0.1f, () =>
                {
                    if (invisible > 0)
                    {
                        invisible -= 0.1f;

                        monster.DisableEffect(Exiled.API.Enums.EffectType.Invisible);
                    }

                    else
                        monster.EnableEffect(Exiled.API.Enums.EffectType.Invisible);
                });
            }
        }

        public void OnHurting(Exiled.Events.EventArgs.Player.HurtingEventArgs ev)
        {
            if (ev.Player == monster || ev.Attacker == monster)
                invisible = 1.5f;
        }   
    }
}
