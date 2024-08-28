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
    class PaperHuman
    {
        public static PaperHuman Instance;

        public List<Player> pl = new List<Player>();

        public void OnEnabled()
        {
            Exiled.Events.Handlers.Player.Spawned += OnSpawned;
        }

        public async void OnSpawned(Exiled.Events.EventArgs.Player.SpawnedEventArgs ev)
        {
            await Task.Delay(100);

            ev.Player.Scale = new Vector3(0.01f, 1, 1f);
        }
    }
}
