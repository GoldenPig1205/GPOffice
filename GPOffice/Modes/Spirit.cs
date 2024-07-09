using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CustomRendering;
using Exiled.API.Features;
using MEC;
using Mirror;
using UnityEngine;

namespace GPOffice.Modes
{
    class Spirit
    {
        public static Spirit Instance;

        List<Player> spirits = new List<Player>();

        public void OnEnabled()
        {
            Timing.RunCoroutine(OnModeStarted());

            Exiled.Events.Handlers.Player.Died += OnDied;
            Exiled.Events.Handlers.Player.Shooting += OnShooting;
        }

        public IEnumerator<float> OnModeStarted()
        {
            yield return Timing.WaitForSeconds(1f);

            while (true)
            {
                foreach (var player in Player.List)
                {
                    if (spirits.Contains(player))
                        player.EnableEffect(Exiled.API.Enums.EffectType.Invisible);
                }

                yield return Timing.WaitForSeconds(1f);
            }
        }

        public async void OnDied(Exiled.Events.EventArgs.Player.DiedEventArgs ev)
        {
            if (spirits.Contains(ev.Player))
            {
                ev.Player.ShowHint($"성불했습니다..", 3);
                Server.ExecuteCommand($"/speak {ev.Player.Id} disable");
                spirits.Remove(ev.Player);
            }
            else
            {
                for (int i = 1; i < 6; i++)
                {
                    ev.Player.ShowHint($"{6 - i}초 뒤 영혼 상태에 돌입합니다.", 1.2f);
                    await Task.Delay(1000);
                }

                Server.ExecuteCommand($"/speak {ev.Player.Id} enable");
                spirits.Add(ev.Player);

                Server.ExecuteCommand($"/fc {ev.Player.Id} Tutorial 1");
            }
        }

        public void OnShooting(Exiled.Events.EventArgs.Player.ShootingEventArgs ev)
        {
            ev.Player.DisableEffect(Exiled.API.Enums.EffectType.Invisible);
        }
    }
}
