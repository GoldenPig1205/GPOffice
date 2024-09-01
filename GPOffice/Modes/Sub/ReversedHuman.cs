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

            foreach (var player in Player.List)
                Spawned(player);
        }

        public void OnSpawned(Exiled.Events.EventArgs.Player.SpawnedEventArgs ev)
        {
            Spawned(ev.Player);
        }

        public void Spawned(Player player)
        {
            player.Scale = new Vector3(-1f, -1f, -1f);
        }
    }
}
