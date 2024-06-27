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

            Exiled.Events.Handlers.Player.Dying += OnDying;
            Exiled.Events.Handlers.Player.Spawned += OnSpawned;
        }

        public IEnumerator<float> OnModeStarted()
        {
            Server.ExecuteCommand($"/mp load {ModeName}");

            Player.List.ToList().CopyTo(pl);

            yield return 0f;
        }

        public void OnDying(Exiled.Events.EventArgs.Player.DyingEventArgs ev)
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
            Player.List.ToList().ForEach(x => x.DisableEffect(Exiled.API.Enums.EffectType.FogControl));
            Timing.CallDelayed(0.1f, () => Player.List.ToList().ForEach(x => x.EnableEffect(Exiled.API.Enums.EffectType.FogControl)));

            if (ev.Player.Role.Type != PlayerRoles.RoleTypeId.NtfSpecialist && pl.Contains(ev.Player))
            {
                ev.Player.Role.Set(PlayerRoles.RoleTypeId.NtfSpecialist);
                ev.Player.Position = GPOffice.GetRandomValue(GPOffice.Instance.Maps[ModeName]);
            }
        }
    }
}
