using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Exiled.API.Features;
using MEC;

namespace GPOffice.Modes
{
    class FastBoy
    {
        public static FastBoy Instance;

        public void OnEnabled()
        {
            Timing.RunCoroutine(OnModeStarted());

            Exiled.Events.Handlers.Player.Spawned += OnSpawned;
        }

        public void OnDisabled()
        {
            Exiled.Events.Handlers.Player.Spawned -= OnSpawned;
        }

        public IEnumerator<float> OnModeStarted()
        {
            Timing.CallDelayed(0.1f, () =>
            {
                foreach (var player in Player.List)
                    player.Role.Set(player.Role);
            });

            yield return 0f;
        }

        public void OnSpawned(Exiled.Events.EventArgs.Player.SpawnedEventArgs ev)
        {
            ev.Player.EnableEffect(Exiled.API.Enums.EffectType.MovementBoost, 255);
            ev.Player.EnableEffect(Exiled.API.Enums.EffectType.Scp1853, 4);
            ev.Player.MaxHealth = ev.Player.Health / 2;
            ev.Player.Health = ev.Player.Health / 2;
        }
    }
}
