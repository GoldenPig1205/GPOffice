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
    class SpeedRun
    {
        public static SpeedRun Instance;

        public List<Player> pl = new List<Player>();

        public void OnEnabled()
        {
            Round.IsLocked = true;
            Server.FriendlyFire = true;

            Timing.RunCoroutine(OnModeStarted());

            Exiled.Events.Handlers.Player.Spawned += OnSpawned;
            Exiled.Events.Handlers.Player.Died += OnDied;
            Exiled.Events.Handlers.Player.Escaping += OnEscaping;
        }

        public IEnumerator<float> OnModeStarted()
        {
            Player.List.ToList().CopyTo(pl);

            Warhead.Start();

            yield return 0f;
        }

        public void OnSpawned(Exiled.Events.EventArgs.Player.SpawnedEventArgs ev)
        {
            if (pl.Contains(ev.Player))
            {
                if (ev.Player.Role.Type == PlayerRoles.RoleTypeId.ChaosConscript)
                {

                }
                else
                {
                    if (ev.Player.Role.Type != PlayerRoles.RoleTypeId.ClassD)
                        ev.Player.Role.Set(PlayerRoles.RoleTypeId.ClassD);
                }
            }
        }

        public void OnDied(Exiled.Events.EventArgs.Player.DiedEventArgs ev)
        {
            if (pl.Contains(ev.Player))
            {
                pl.Remove(ev.Player);

                if (pl.Count < 2)
                    Round.IsLocked = false;
            }
        }

        public void OnEscaping(Exiled.Events.EventArgs.Player.EscapingEventArgs ev)
        {
            Round.IsLocked = false;

            Player.List.ToList().ForEach(x => x.Broadcast(15, $"<b><size=30><탈출자 : {ev.Player.DisplayNickname}></size></b>"));
        }
    }
}
