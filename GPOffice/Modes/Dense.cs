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
    class Dense
    {
        public static Dense Instance;

        public List<string> pl = new List<string>();

        public void OnEnabled()
        {
            Timing.RunCoroutine(OnModeStarted());
        }

        public IEnumerator<float> OnModeStarted()
        {
            Timing.CallDelayed(0.1f, () =>
            {
                Vector3 pos = Exiled.API.Features.Doors.Door.Random().Position;
                pos.y += 1;

                foreach (var player in Player.List)
                {
                    player.Position = pos;
                    Player.List.ToList().ForEach(x => x.Position = pos);
                }
            });

            yield return 0f;
        }

    }
}
