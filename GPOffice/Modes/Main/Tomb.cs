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
    class Tomb
    {
        public static Tomb Instance;

        CoroutineHandle timing_OnModeStarted;

        public List<Player> pl = new List<Player>();

        public void OnEnabled()
        {
            Server.FriendlyFire = true;
            Round.IsLocked = true;
            Respawn.TimeUntilNextPhase = 10000;

            timing_OnModeStarted = Timing.RunCoroutine(OnModeStarted());

            Exiled.Events.Handlers.Player.Died += OnDied;
            Exiled.Events.Handlers.Player.Spawned += OnSpawned;
        }

        public void OnDisabled()
        {
            Timing.KillCoroutines(timing_OnModeStarted);

            Exiled.Events.Handlers.Player.Died -= OnDied;
            Exiled.Events.Handlers.Player.Spawned -= OnSpawned;
        }

        public Vector3 RandomPosition()
        {
            return new Vector3(UnityEngine.Random.Range(-27.92969f, 44.88281f), 1042.989f, UnityEngine.Random.Range(-75.78906f, -2.71875f));
        }

        public IEnumerator<float> OnModeStarted()
        {
            Server.ExecuteCommand($"/mp load plane");

            Player Dummy = Player.List.ToList()[0];
            Player.List.ToList().CopyTo(pl);

            for (int i=1; i<250; i++)
            {
                Dummy.Position = RandomPosition();
                Server.ExecuteCommand($"/drop {Dummy.Id} {UnityEngine.Random.Range(0, 55)} {UnityEngine.Random.Range(1, 3)}");
            }
            
            foreach (var player in Player.List)
            {
                player.Role.Set(PlayerRoles.RoleTypeId.Tutorial);
                Timing.CallDelayed(0.01f, () =>
                {
                    player.Position = RandomPosition();
                });
            }

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
            Player.List.ToList().ForEach(x => x.DisableEffect(Exiled.API.Enums.EffectType.FogControl));
            Timing.CallDelayed(0.1f, () => Player.List.ToList().ForEach(x => x.EnableEffect(Exiled.API.Enums.EffectType.FogControl)));
        }
    }
}
