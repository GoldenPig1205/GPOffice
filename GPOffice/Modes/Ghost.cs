using System;
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
            List<object> normal = new List<object>() { "unlock **", "lock **", "open **", "close **", "server_event detonation_start", "server_event detonation_cancel" };
            List<object> hard = new List<object>() { "destroy **", "server_event detonation_instant" };

            while (true)
            {
                int r1 = UnityEngine.Random.Range(1, 1000);

                if (r1 == 1)
                    Server.ExecuteCommand($"/{GPOffice.GetRandomValue(hard)}");

                else if (r1 > 950)
                    Server.ExecuteCommand($"/{GPOffice.GetRandomValue(normal)}");

                else
                {
                    int r2 = UnityEngine.Random.Range(1, 2);
                    Exiled.API.Features.Doors.Door RandomDoor = Exiled.API.Features.Doors.Door.Random();

                    if (r2 == 1)
                        RandomDoor.IsOpen = true;

                    else if (r2 == 2)
                        RandomDoor.IsOpen = false;

                }

                await Task.Delay(10);
            }
        }
    }
}
