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
    class PetShop
    {
        public static PetShop Instance;

        public Dictionary<Player, List<Player>> Pets = new Dictionary<Player, List<Player>>();

        public void OnEnabled()
        {
            Timing.RunCoroutine(OnModeStarted());

            Exiled.Events.Handlers.Player.Hurting += OnHurting;
            Exiled.Events.Handlers.Player.Died += OnDied;
        }

        public IEnumerator<float> OnModeStarted()
        {
            while (true)
            {
                foreach (var player in Player.List)
                {
                    if (Pets.ContainsKey(player))
                    {
                        foreach (var pet in Pets[player])
                            pet.Position = player.Position;
                    }
                }

                yield return Timing.WaitForSeconds(5f);
            }
        }

        public void OnHurting(Exiled.Events.EventArgs.Player.HurtingEventArgs ev)
        {
            if (ev.Attacker != null && ev.Player != null)
            {
                if (Pets.ContainsKey(ev.Player) && Pets[ev.Player].Contains(ev.Attacker))
                    ev.IsAllowed = false;
            }
        }

        public async void OnDied(Exiled.Events.EventArgs.Player.DiedEventArgs ev)
        {
            if (ev.Attacker != null && ev.Player != null)
            {
                if (Pets.ContainsKey(ev.Attacker))
                    Pets[ev.Attacker].Add(ev.Player);

                else
                    Pets.Add(ev.Attacker, new List<Player> { ev.Player });

                for (int i = 1; i < 6; i++)
                {
                    ev.Player.ShowHint($"{6 - i}초 뒤 펫으로 생성됩니다.", 1.2f);
                    await Task.Delay(1000);
                }

                ev.Player.Role.Set(PlayerRoles.RoleTypeId.Tutorial);
                ev.Player.Scale = new Vector3(0.5f, 0.5f, 0.5f);
                ev.Player.Group = new UserGroup { BadgeText = $"{ev.Attacker.DisplayNickname}의 펫", BadgeColor = "pink" };
                Server.ExecuteCommand($"/god {ev.Player.Id} 1");
            }
        }
    }
}
