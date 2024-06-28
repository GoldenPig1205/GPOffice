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

        public List<string> pl = new List<string>();

        public void OnEnabled()
        {
            Timing.RunCoroutine(OnModeStarted());
        }

        public IEnumerator<float> OnModeStarted()
        {
            Timing.CallDelayed(0.1f, () =>
            {
                Vector3 DoorTP = Exiled.API.Features.Doors.Door.Random().Position;
                Player.List.ToList().ForEach(x => x.Position = DoorTP);
            });

            yield return 0f;
        }

    }
}
