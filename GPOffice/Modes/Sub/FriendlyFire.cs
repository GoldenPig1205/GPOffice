﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Exiled.API.Features;
using UnityEngine;
using PlayerRoles.FirstPersonControl;
using MEC;
using HarmonyLib;
using Mirror;
using PlayerRoles.PlayableScps.Scp049;
using Utils.Networking;
using PlayerRoles;
using PlayerRoles.PlayableScps.Scp106;
using PlayerRoles.PlayableScps.Scp939;
using PlayerRoles.PlayableScps.Scp173;
using Exiled.API.Features.DamageHandlers;
using PluginAPI.Events;

namespace GPOffice.SubModes
{
    class FriendlyFire
    {
        public static FriendlyFire Instance;

        public List<Player> HumanMeleeCooldown = new List<Player>();
        public int Scp106AttackTeamCoolDown = 0;
        public List<Player> Scp106Stacks = new List<Player>();

        public void OnEnabled()
        {
            Server.FriendlyFire = true;

            Timing.RunCoroutine(OnModeStarted());

            Exiled.Events.Handlers.Player.Spawned += OnSpawned;
            Exiled.Events.Handlers.Player.TogglingNoClip += OnTogglingNoClip;

            Exiled.Events.Handlers.Scp173.Blinking += OnBlinking;

            Exiled.Events.Handlers.Scp939.Lunging += OnLunging;

            Harmony harmony = new Harmony("HitboxIdentityPatch");
            harmony.PatchAll();
        }

        public IEnumerator<float> OnModeStarted()
        {
            yield return Timing.WaitForSeconds(1f);
        }

        public void OnSpawned(Exiled.Events.EventArgs.Player.SpawnedEventArgs ev)
        {
            if (new List<RoleTypeId>() { RoleTypeId.Scp173, RoleTypeId.Scp106 }.Contains(ev.Player.Role.Type) || ev.Player.IsHuman)
                ev.Player.ShowHint($"<size=20><b><i>tip.</i></b> [ALT] 키를 눌러 같은 진영에게 피해를 입힐 수 있습니다.</size>", 10);

            else if (ev.Player.Role.Type == RoleTypeId.Scp939)
                ev.Player.ShowHint($"<size=20><b><i>tip.</i></b> 런지를 사용하는 도중 근접한 SCP를 쳐다보면 해당 개체에 피해를 입힐 수 있습니다.</size>", 10);
        }

        public async void OnTogglingNoClip(Exiled.Events.EventArgs.Player.TogglingNoClipEventArgs ev)
        {
            if (ev.Player.IsHuman)
            {
                if (Physics.Raycast(ev.Player.ReferenceHub.PlayerCameraReference.position + ev.Player.ReferenceHub.PlayerCameraReference.forward * 0.2f, ev.Player.ReferenceHub.PlayerCameraReference.forward, out RaycastHit hit, 1.5f, InventorySystem.Items.Firearms.Modules.StandardHitregBase.HitregMask) &&
                    hit.collider.TryGetComponent<IDestructible>(out IDestructible destructible))
                {
                    var player = Player.Get(hit.collider.GetComponentInParent<ReferenceHub>());

                    if (ev.Player != player && !HumanMeleeCooldown.Contains(ev.Player))
                    {
                        Hitmarker.SendHitmarkerDirectly(ev.Player.ReferenceHub, 0.7f);
                        player.Hurt(12.05f, "무지성으로 구타당해 죽었습니다.");

                        HumanMeleeCooldown.Add(ev.Player);
                        await Task.Delay(1000);
                        HumanMeleeCooldown.Remove(ev.Player);
                    }
                }
            }

            else if (ev.Player.Role.Type == RoleTypeId.Scp173)
            {
                if (Physics.Raycast(ev.Player.ReferenceHub.PlayerCameraReference.position + ev.Player.ReferenceHub.PlayerCameraReference.forward * 0.2f, ev.Player.ReferenceHub.PlayerCameraReference.forward, out RaycastHit hit, 1.2f, InventorySystem.Items.Firearms.Modules.StandardHitregBase.HitregMask) &&
                    hit.collider.TryGetComponent<IDestructible>(out IDestructible destructible))
                {
                    if (ev.Player.Role is Exiled.API.Features.Roles.Scp173Role scp173)
                    {
                        if (scp173.BlinkCooldown == 0f)
                        {
                            var player = Player.Get(hit.collider.GetComponentInParent<ReferenceHub>());

                            if (ev.Player != player && player.IsScp)
                            {
                                Hitmarker.SendHitmarkerDirectly(ev.Player.ReferenceHub, 1.2f);
                                Server.ExecuteCommand($"/cassie {ev.Player.Role.Name}이(가) {player.Role.Name}의 뒷통수를 쳤습니다.");
                                player.Hurt(-1, Exiled.API.Enums.DamageType.Scp173);
                            }
                        }
                    }
                }
            }

            else if (ev.Player.Role.Type == RoleTypeId.Scp106)
            {
                if (Physics.Raycast(ev.Player.ReferenceHub.PlayerCameraReference.position + ev.Player.ReferenceHub.PlayerCameraReference.forward * 0.2f, ev.Player.ReferenceHub.PlayerCameraReference.forward, out RaycastHit hit, 1.5f, InventorySystem.Items.Firearms.Modules.StandardHitregBase.HitregMask) &&
                    hit.collider.TryGetComponent<IDestructible>(out IDestructible destructible))
                {
                    var player = Player.Get(hit.collider.GetComponentInParent<ReferenceHub>());

                    if (ev.Player != player && player.IsScp && Scp106AttackTeamCoolDown <= 0)
                    {
                        Hitmarker.SendHitmarkerDirectly(ev.Player.ReferenceHub, 0.9f);
                        player.Hurt(40, Exiled.API.Enums.DamageType.Scp106);
                        Scp106AttackTeamCoolDown = 1;

                        if (Scp106Stacks.Contains(player))
                        {
                            player.EnableEffect(Exiled.API.Enums.EffectType.PocketCorroding);
                            await Task.Delay(1000);
                            player.DisableEffect(Exiled.API.Enums.EffectType.PocketCorroding);

                            Scp106AttackTeamCoolDown = 0;
                        }
                        else
                        {
                            Scp106Stacks.Add(player);
                            await Task.Delay(1000);
                            Scp106AttackTeamCoolDown = 0;
                            await Task.Delay(9000);
                            Scp106Stacks.Remove(player);
                        }
                    }
                }
            }
        }

        public async void OnBlinking(Exiled.Events.EventArgs.Scp173.BlinkingEventArgs ev)
        {
            await Task.Delay(20);

            if (Physics.Raycast(ev.Player.ReferenceHub.PlayerCameraReference.position + ev.Player.ReferenceHub.PlayerCameraReference.forward * 0.2f, ev.Player.ReferenceHub.PlayerCameraReference.forward, out RaycastHit hit, 1.5f, InventorySystem.Items.Firearms.Modules.StandardHitregBase.HitregMask) &&
            hit.collider.TryGetComponent<IDestructible>(out IDestructible destructible))
            {
                var player = Player.Get(hit.collider.GetComponentInParent<ReferenceHub>());

                if (ev.Player != player && player.IsScp)
                {
                    Hitmarker.SendHitmarkerDirectly(ev.Player.ReferenceHub, 1.5f);
                    Server.ExecuteCommand($"/cassie {ev.Player.Role.Name}이(가) {player.Role.Name}의 뒷통수를 쳤습니다.");
                    player.Hurt(-1, Exiled.API.Enums.DamageType.Scp173);
                }
            }
        }

        public async void OnLunging(Exiled.Events.EventArgs.Scp939.LungingEventArgs ev)
        {
            for (float i=0; i<0.85f; i += 0.01f)
            {
                if (Physics.Raycast(ev.Player.ReferenceHub.PlayerCameraReference.position + ev.Player.ReferenceHub.PlayerCameraReference.forward * 0.2f, ev.Player.ReferenceHub.PlayerCameraReference.forward, out RaycastHit hit, 1.5f, InventorySystem.Items.Firearms.Modules.StandardHitregBase.HitregMask) &&
                hit.collider.TryGetComponent<IDestructible>(out IDestructible destructible))
                {
                    var player = Player.Get(hit.collider.GetComponentInParent<ReferenceHub>());

                    if (ev.Player != player && player.IsScp)
                    {
                        Hitmarker.SendHitmarkerDirectly(ev.Player.ReferenceHub, 1.5f);
                        Server.ExecuteCommand($"/cassie {ev.Player.Role.Name}이(가) {player.Role.Name}의 뒷통수를 쳤습니다.");
                        player.Hurt(-1, Exiled.API.Enums.DamageType.Scp939);
                    }
                }

                await Task.Delay(10);
            }
        }

        [HarmonyPatch(typeof(HitboxIdentity), nameof(HitboxIdentity.IsEnemy), typeof(Team), typeof(Team))]
        public class HitboxPatchPostfix
        {
            public static void Postfix(ref bool __result)
            {
                __result = true;
            }
        }
    }
}
