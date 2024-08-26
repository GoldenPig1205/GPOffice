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
    class Juggernaut
    {
        public static Juggernaut Instance;
        public Player juggernaut;

        public void OnEnabled()
        {
            Timing.RunCoroutine(OnModeStarted());

            Exiled.Events.Handlers.Player.SearchingPickup += OnSearchingPickup;
            Exiled.Events.Handlers.Player.DroppingItem += OnDroppingItem;
            Exiled.Events.Handlers.Player.Shooting += OnShooting;
            Exiled.Events.Handlers.Player.ChangingItem += OnChangingItem;
            Exiled.Events.Handlers.Player.Hurting += OnHurting;
        }

        public IEnumerator<float> OnModeStarted()
        {
            yield return Timing.WaitForSeconds(1f);

            Player juggernaut = Plugin.GetRandomValue(Player.List.ToList());

            juggernaut.Role.Set(PlayerRoles.RoleTypeId.Tutorial);
            juggernaut.Scale = new Vector3(1.5f, 1.15f, 1.2f);
            juggernaut.MaxHealth = 200 * (Player.List.Count() - 1);
            juggernaut.Health = juggernaut.MaxHealth;
            juggernaut.EnableEffect(Exiled.API.Enums.EffectType.SinkHole);
            juggernaut.Broadcast(10, "<b><size=30>당신은 <color=#298A08>저거너트</color>입니다.</size></b>\n<size=25><i>본인을 제외한 모두를 사살하십시오.</i></size>");
            
            List<ItemType> Items = new List<ItemType>() { ItemType.GunLogicer };
            foreach (var Item in Items)
                juggernaut.AddItem(Item);
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
                ev.Player.CurrentItem.As<Exiled.API.Features.Items.Firearm>().Ammo += 250;
        }

        public void OnChangingItem(Exiled.Events.EventArgs.Player.ChangingItemEventArgs ev)
        {
            if (ev.Player == juggernaut)
                ev.IsAllowed = false;
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
                }
                else
                {
                    ev.IsAllowed = false;
                }
            }
        } 
    }
}
