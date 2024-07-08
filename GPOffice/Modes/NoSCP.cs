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
    class NoSCP
    {
        public static NoSCP Instance;

        public void OnEnabled()
        {
            Exiled.Events.Handlers.Player.Spawned += OnSpawned;
        }

        public void OnSpawned(Exiled.Events.EventArgs.Player.SpawnedEventArgs ev)
        {
            if (ev.Player.IsScp || new List<PlayerRoles.RoleTypeId>() { PlayerRoles.RoleTypeId.ChaosRifleman, PlayerRoles.RoleTypeId.ChaosRepressor, PlayerRoles.RoleTypeId.ChaosMarauder,
            PlayerRoles.RoleTypeId.NtfSergeant, PlayerRoles.RoleTypeId.NtfCaptain, PlayerRoles.RoleTypeId.NtfPrivate, PlayerRoles.RoleTypeId.FacilityGuard}.Contains(ev.Player.Role.Type))
            {
                int rn = UnityEngine.Random.Range(1, 4);

                if (rn == 1)
                    ev.Player.Role.Set(PlayerRoles.RoleTypeId.ChaosConscript);
                else if (rn == 2)
                    ev.Player.Role.Set(PlayerRoles.RoleTypeId.NtfSpecialist);
                else if (rn == 3)
                {
                    ev.Player.Role.Set(PlayerRoles.RoleTypeId.Tutorial);
                    ev.Player.Position = new Vector3(-0.08203125f, 1000.96f, 6.828125f);
                    
                    foreach (ItemType Item in new List<ItemType>{ ItemType.KeycardFacilityManager, ItemType.GunFSP9, ItemType.GunRevolver, ItemType.Adrenaline, ItemType.AntiSCP207, ItemType.Ammo9x19, ItemType.Ammo44cal })
                        ev.Player.AddItem(Item);
                }
            }
        }
    }
}
