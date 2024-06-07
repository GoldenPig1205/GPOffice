﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Exiled.API.Features;

namespace GPOffice.Modes
{
    class Ghost
    {
        public static Ghost Instance;

        Task TaskA = new Task(() => Ghost.Instance.OnModeStarted());

        public void OnEnabled()
        {
            TaskA.Start();
        }

        public void OnDisabled()
        {
            TaskA.Dispose();
        }

        public async void OnModeStarted()
        {
            List<object> normal = new List<object>() { "unlock **", "lock **", "open **", "close **", "server_event detonation_start", "server_event detonation_cancel",
                                                       "el u all", "el l all", "el s all", "overcharge 0 1" };
            List<object> hard = new List<object>() { "server_event detonation_instant" };

            while (true)
            {
                int r1 = UnityEngine.Random.Range(1, 10000);

                if (r1 == 1)
                    Server.ExecuteCommand($"/{GPOffice.GetRandomValue(hard)}");

                else if (r1 > 8950)
                    Server.ExecuteCommand($"/{GPOffice.GetRandomValue(normal)}");

                else
                {
                    int r2 = UnityEngine.Random.Range(1, 6);
                    Exiled.API.Features.Doors.Door door = Exiled.API.Features.Doors.Door.Random();

                    if (r2 == 1)
                        door.IsOpen = true;
                    else if (r2 == 2)
                        door.IsOpen = false;
                    else if (r2 == 3)
                        door.Lock(3, Exiled.API.Enums.DoorLockType.AdminCommand);
                    else if (r2 == 4)
                        door.Unlock();
                    else if (r2 == 5)
                    {
                        if (!door.IsElevator && !door.IsGate)
                            door.As<Exiled.API.Features.Doors.BreakableDoor>().Break();
                    }
                    else
                    {
                        if (UnityEngine.Random.Range(1, 100) == 1)
                        {
                            Cassie.Message(message: $".G{UnityEngine.Random.Range(1, 7)}", isNoisy: false);
                        }
                    }

                }

                await Task.Delay(10);
            }
        }
    }
}
