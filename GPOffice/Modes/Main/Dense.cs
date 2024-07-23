using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Exiled.API.Features;
using MEC;
using Mirror;
using UnityEngine;

namespace GPOffice.Modes
{
    class Dense
    {
        public static Dense Instance;

        CoroutineHandle timing_OnModeStarted;

        public List<string> pl = new List<string>();

        public void OnEnabled()
        {
            timing_OnModeStarted = Timing.RunCoroutine(OnModeStarted());
        }

        public void OnDisabled()
        {
            Timing.KillCoroutines(timing_OnModeStarted);
        }

        public IEnumerator<float> OnModeStarted()
        {
            yield return Timing.WaitForSeconds(10f);

            while (true)
            {
                Vector3 pos = Exiled.API.Features.Doors.Door.Random().Position;
                pos.y += 1;

                foreach (var player in Player.List)
                {
                    player.Position = pos;
                    Player.List.ToList().ForEach(x => x.Position = pos);
                }

                int r = UnityEngine.Random.Range(1, 180);
                for (int i=1; i<r; i++)
                {
                    foreach (var player in Player.List)
                    {
                        player.ClearBroadcasts();
                        player.Broadcast(2, $"<size=25><b>{r - i}초 뒤에 순간이동합니다.</b></size>");
                    }
                    yield return Timing.WaitForSeconds(1f);
                }
            }
        }

    }
}
