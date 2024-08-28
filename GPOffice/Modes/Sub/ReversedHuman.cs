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

namespace GPOffice.SubModes
{
    class ReversedHuman
    {
        public static ReversedHuman Instance;

        public List<Player> pl = new List<Player>();

        public void OnEnabled()
        {
            Exiled.Events.Handlers.Player.Spawned += OnSpawned;
        }

        public void OnSpawned(Exiled.Events.EventArgs.Player.SpawnedEventArgs ev)
        {
            await Task.Delay(100);

            ev.Player.Scale = new Vector3(-1f, -1f, -1f);
        }
    }
}
