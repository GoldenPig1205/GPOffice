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
    class FreeForAll
    {
        public static FreeForAll Instance;

        public List<Player> pl = new List<Player>();
        public string ModeName = GPOffice.GetRandomValue(GPOffice.Instance.Maps.Keys.ToList()).ToString();

        public void OnEnabled()
        {
            Server.FriendlyFire = true;
            Round.IsLocked = true;
            Respawn.TimeUntilNextPhase = 10000;

            Timing.RunCoroutine(OnModeStarted());

            Exiled.Events.Handlers.Player.Spawned += OnSpawned;
            Exiled.Events.Handlers.Player.Died += OnDied;
        }

        public IEnumerator<float> OnModeStarted()
        {
            Server.ExecuteCommand($"/mp load {ModeName}");

            Player.List.ToList().CopyTo(pl);

            yield return 0f;
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

        public void OnSpawned(Exiled.Events.EventArgs.Player.SpawnedEventArgs ev)
        {
            if (ev.Player.Role.Type != PlayerRoles.RoleTypeId.NtfPrivate && pl.Contains(ev.Player))
            {
                ev.Player.Role.Set(PlayerRoles.RoleTypeId.NtfPrivate);
                ev.Player.Position = GPOffice.GetRandomValue(GPOffice.Instance.Maps[ModeName]);
            }
        }
    }
}
