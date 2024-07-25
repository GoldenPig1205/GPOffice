using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Exiled.API.Features;
using MEC;

namespace GPOffice.SubModes
{
    class SpeedWagon
    {
        public static SpeedWagon Instance;

        public void OnEnabled()
        {
            Exiled.Events.Handlers.Player.Spawned += OnSpawned;

            foreach (var player in Player.List)
                Spawned(player);
        }

        public void OnSpawned(Exiled.Events.EventArgs.Player.SpawnedEventArgs ev)
        {
            Spawned(ev.Player);
        }

        public void Spawned(Player player)
        {
            player.EnableEffect(Exiled.API.Enums.EffectType.MovementBoost, 255);
            player.EnableEffect(Exiled.API.Enums.EffectType.Scp1853, 4);
            player.MaxHealth = player.Health / 4;
            player.Health = player.Health / 4;
        }
    }
}
