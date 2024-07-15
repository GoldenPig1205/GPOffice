using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using CustomRendering;
using Exiled.API.Features;
using HarmonyLib;
using MEC;
using Mirror;
using UnityEngine;

namespace GPOffice.Modes
{
    class FreeForAll
    {
        public static FreeForAll Instance;

        public List<Player> pl = new List<Player>();
        public string ModeName = Plugin.GetRandomValue(Plugin.Instance.Maps.Keys.ToList()).ToString();
        public List<ItemType> StartupItems = null;

        public void OnEnabled()
        {
            Server.FriendlyFire = true;
            Round.IsLocked = true;
            Respawn.TimeUntilNextPhase = 10000;

            Timing.RunCoroutine(OnModeStarted());

            Exiled.Events.Handlers.Player.Dying += OnDying;
            Exiled.Events.Handlers.Player.Spawned += OnSpawned;
        }

        public List<ItemType> Items()
        {
            List<ItemType> Guns = new List<ItemType>() { ItemType.GunA7, ItemType.GunE11SR, ItemType.GunShotgun, ItemType.GunCom45, ItemType.GunFSP9, ItemType.GunRevolver, 
                ItemType.GunCOM18, ItemType.GunCrossvec, ItemType.GunLogicer, ItemType.GunFRMG0, ItemType.GunAK, ItemType.Jailbird, ItemType.ParticleDisruptor };
            List<ItemType> Ammos = new List<ItemType>() { ItemType.Ammo12gauge, ItemType.Ammo44cal, ItemType.Ammo556x45, ItemType.Ammo762x39, ItemType.Ammo9x19 };
            List<ItemType> CDItems = new List<ItemType>() { ItemType.Medkit, ItemType.Painkillers, ItemType.Radio, ItemType.GrenadeFlash, ItemType.GrenadeHE };
            List<ItemType> Items = new List<ItemType>();

            Items.Add(Plugin.GetRandomValue(Guns));
            
            foreach (var ammo in Ammos)
            {
                for (int i= 0; i < 10; i++)
                    Items.Add(ammo);    
            }

            foreach (var item in CDItems)
            {
               if (UnityEngine.Random.Range(1, 2) == 1)
                    Items.Add(item);
            }

            return Items;
        }

        public IEnumerator<float> OnModeStarted()
        {
            StartupItems = Items();

            Server.ExecuteCommand($"/mp load {ModeName}");

            Player.List.ToList().CopyTo(pl);

            Player.List.ToList().ForEach(x => Spawned(x));

            yield return 0f;
        }

        public void OnDying(Exiled.Events.EventArgs.Player.DyingEventArgs ev)
        {
            if (pl.Contains(ev.Player))
            {
                pl.Remove(ev.Player);

                if (pl.Count < 2)
                    Round.IsLocked = false;
            }
        }

        public void OnSpawned(Exiled.Events.EventArgs.Player.SpawnedEventArgs ev)
        {
            Spawned(ev.Player);
        }

        public void Spawned(Player player)
        {
            Player.List.ToList().ForEach(x => x.DisableEffect(Exiled.API.Enums.EffectType.FogControl));
            Timing.CallDelayed(0.1f, () => Player.List.ToList().ForEach(x => x.EnableEffect(Exiled.API.Enums.EffectType.FogControl)));

            if (player.Role.Type != PlayerRoles.RoleTypeId.NtfSpecialist && pl.Contains(player))
            {
                player.Role.Set(PlayerRoles.RoleTypeId.NtfSpecialist);
                player.Position = Plugin.GetRandomValue(Plugin.Instance.Maps[ModeName]);

                player.ClearInventory();

                foreach (var item in StartupItems)
                    player.AddItem(item);
            }
        }
    }
}
