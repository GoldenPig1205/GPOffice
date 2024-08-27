﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CustomRendering;
using Exiled.API.Features;
using MEC;
using Mirror;
using UnityEngine;
using Exiled.API.Features.Items;
using System.Runtime.InteropServices;

namespace GPOffice.Modes
{
    class Juggernaut
    {
        public static Juggernaut Instance;
        public Player juggernaut;

        public void OnEnabled()
        {
            Server.FriendlyFire = true;
            Round.IsLocked = true;

            Timing.RunCoroutine(OnModeStarted());

            Exiled.Events.Handlers.Player.SearchingPickup += OnSearchingPickup;
            Exiled.Events.Handlers.Player.DroppingItem += OnDroppingItem;
            Exiled.Events.Handlers.Player.Shooting += OnShooting;
            Exiled.Events.Handlers.Player.ChangingItem += OnChangingItem;
            Exiled.Events.Handlers.Player.Hurting += OnHurting;
        }

        public IEnumerator<float> OnModeStarted()
        {
            yield return Timing.WaitForSeconds(10);

            juggernaut = Plugin.GetRandomValue(Player.List.ToList());

            juggernaut.Role.Set(PlayerRoles.RoleTypeId.Tutorial);
            juggernaut.Scale = new Vector3(1.5f, 1.15f, 1.2f);
            juggernaut.MaxHealth = 200 * (Player.List.Count() - 1);
            juggernaut.Health = juggernaut.MaxHealth;
            juggernaut.EnableEffect(Exiled.API.Enums.EffectType.SinkHole);
            juggernaut.Broadcast(10, "<b><size=30>당신은 <color=#298A08>저거너트</color>입니다.</size></b>\n<size=25><i>본인을 제외한 모두를 사살하십시오.</i></size>");
            juggernaut.Position = new Vector3(123.8387f, 988.7921f, 25.39412f);
            
            List<ItemType> Items = new List<ItemType>() { ItemType.GunLogicer };
            foreach (var Item in Items)
                juggernaut.AddItem(Item);

            bool IsEnd = false;
            while (!IsEnd)
            {
                if (juggernaut.IsAlive)
                {
                    if (Player.List.Count() <= 1)
                    {
                        Player.List.ToList().ForEach(x => x.Broadcast(20, "<color=#298A08>저거너트</color>의 승리입니다."));
                        IsEnd = true;
                    }
                }
                else
                {
                    Player.List.ToList().ForEach(x => x.Broadcast(20, "<b>Site-76 구성원</b>들의 승리입니다."));
                    IsEnd = true;
                }

                yield return Timing.WaitForSeconds(1f);
            }

            Round.IsLocked = false;
        }

        public void OnSearchingPickup(Exiled.Events.EventArgs.Player.SearchingPickupEventArgs ev)
        {
            if (ev.Player == juggernaut)
                ev.IsAllowed = false;
        }

        public void OnDroppingItem(Exiled.Events.EventArgs.Player.DroppingItemEventArgs ev)
        {
            if (ev.Player == juggernaut)
                ev.IsAllowed = false;
        }

        public void OnShooting(Exiled.Events.EventArgs.Player.ShootingEventArgs ev)
        {
            if (ev.Player == juggernaut)
                ev.Player.CurrentItem.As<Firearm>().Ammo = 250;
        }

        public void OnChangingItem(Exiled.Events.EventArgs.Player.ChangingItemEventArgs ev)
        {
            if (ev.Player == juggernaut)
            {
                ev.Player.CurrentItem = Item.Get(92);
            }
        }

        public void OnHurting(Exiled.Events.EventArgs.Player.HurtingEventArgs ev)
        {
            if (ev.Attacker != null)
            {
                if (ev.Player == juggernaut || ev.Attacker == juggernaut)
                {
                    if (ev.Attacker == juggernaut && ev.Player != juggernaut)
                    {
                        ev.DamageHandler.Damage = ev.DamageHandler.Damage * 3;
                    }
                    else if (ev.Attacker != juggernaut && ev.Player == juggernaut)
                    {
                        if (ev.DamageHandler.Damage == -1 || ev.DamageHandler.Damage > 300)
                            ev.DamageHandler.Damage = 300;
                    }   
                }
                else
                {
                    ev.IsAllowed = false;
                }
            }
        }
    }
}
