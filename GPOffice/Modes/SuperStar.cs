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

        public List<string> pl = new List<string>();

        Task TaskA = new Task(() => SuperStar.Instance.OnModeStarted());

        public void OnEnabled()
        {
            TaskA.Start();

            Exiled.Events.Handlers.Player.Left += OnLeft;
        }

        public async void OnModeStarted()
        {
            while (true)
            {
                foreach (var player in Player.List)
                    if (!pl.Contains(player.UserId))
                    {
                        Server.ExecuteCommand($"/speak {player} enable");
                        pl.Add(player.UserId);
                    }

                await Task.Delay(1000);
            }
        }

        public void OnLeft(Exiled.Events.EventArgs.Player.LeftEventArgs ev)
        {
            if (pl.Contains(ev.Player.UserId))
                pl.Remove(ev.Player.UserId);
        }
    }
}
