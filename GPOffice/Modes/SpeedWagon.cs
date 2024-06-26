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
    class SpeedWagon
    {
        public static SpeedWagon Instance;

        public void OnEnabled()
        {
            Exiled.Events.Handlers.Player.Spawned += OnSpawned;
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
