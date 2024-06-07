using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Exiled.API.Features;

namespace GPOffice.Modes
{
    class SuperStar
    {
        public static SuperStar Instance;

        Task TaskA = new Task(() => SuperStar.Instance.OnModeStarted());

        public void OnEnabled()
        {
            TaskA.Start();
        }

        public void OnDisabled()
        {
            TaskA.Dispose();
        }

        public void OnModeStarted()
        {
            foreach (var player in Player.List)
                Server.ExecuteCommand($"/speak {player.Id} enable");
        }
    }
}
