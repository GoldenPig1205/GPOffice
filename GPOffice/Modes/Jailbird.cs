using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Exiled.API.Features;
using MEC;
using Mirror;
using UnityEngine;

namespace GPOffice.Modes
{
    class Jailbird
    {
        public static Jailbird Instance;

        public void OnEnabled()
        {
            Exiled.Events.Handlers.Player.Spawned += OnSpawned;
        }

        public void OnSpawned(Exiled.Events.EventArgs.Player.SpawnedEventArgs ev)
        {
            ev.Player.AddItem(ItemType.Jailbird);
            Server.ExecuteCommand($"/forceeq {Player.UserIdsCache} 50");
        }
    }
}
