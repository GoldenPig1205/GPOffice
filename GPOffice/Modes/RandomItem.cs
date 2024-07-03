﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using CustomRendering;
using Exiled.API.Features;
using MEC;
using Mirror;
using UnityEngine;

namespace GPOffice.Modes
{
    class RandomItem
    {
        public static RandomItem Instance;

        public void OnEnabled()
        {
            Timing.RunCoroutine(OnModeStarted());

            Exiled.Events.Handlers.Player.Spawned += OnSpawned;
        }

        public IEnumerator<float> OnModeStarted()
        {
            while (true)
            {
                foreach (var player in Player.List)
                {
                    int ri = UnityEngine.Random.Range(0, 55);

                    if (player.IsScp)
                    {
                        player.ClearInventory();

                        Server.ExecuteCommand($"/give {player.Id} {ri}");
                        Server.ExecuteCommand($"/forceeq {player.Id} {ri}");
                    }

                    else
                        Server.ExecuteCommand($"/give {player.Id} {ri}");
                }

                yield return Timing.WaitForSeconds(60f);
            }
        }

        public void OnSpawned(Exiled.Events.EventArgs.Player.SpawnedEventArgs ev)
        {
            ev.Player.ClearInventory();

            for (int i=1; i<9; i++)
                Server.ExecuteCommand($"/give {ev.Player.Id} {UnityEngine.Random.Range(0, 55)}");
        }
    }
}
