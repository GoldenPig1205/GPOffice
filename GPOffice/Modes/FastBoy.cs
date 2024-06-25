using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Exiled.API.Features;

namespace GPOffice.Modes
{
    class FastBoy
    {
        public static FastBoy Instance;

        public List<string> pl = new List<string>();

        Task TaskA = new Task(() => FastBoy.Instance.OnModeStarted());

        public void OnEnabled()
        {
            TaskA.Start();

            Exiled.Events.Handlers.Player.Left += OnLeft;
        }

        public void OnDisabled()
        {
            TaskA.Dispose();

            Exiled.Events.Handlers.Player.Left -= OnLeft;
        }

        public async void OnModeStarted()
        {
            while (true)
            {
                foreach (var player in Player.List)
                {
                    if (!pl.Contains(player.UserId))
                    {
                        player.EnableEffect(Exiled.API.Enums.EffectType.MovementBoost, 255);
                        player.EnableEffect(Exiled.API.Enums.EffectType.Scp1853, 4);
                        player.MaxHealth = player.MaxHealth / 2;
                        pl.Add(player.UserId);
                    }
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
