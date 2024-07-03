﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Exiled.API.Features;
using UnityEngine;
using PlayerRoles.FirstPersonControl;

namespace GPOffice.Modes
{
    public class Hit
    {
        public Player player;
        public PlayerMovementState Movestate;
        bool CanHit = true;
        PlayerMovementState _Movestate;

        public void Update()
        {
            Movestate = (player.Role.Base as FpcStandardRoleBase).FpcModule.CurrentMovementState;

            if (Movestate != _Movestate)
            {
                _Movestate = Movestate;

                if (Movestate == PlayerMovementState.Sneaking && CanHit)
                {
                    FriendlyFire.Instance.OnMelee(player);
                    CanHit = false;
                    MEC.Timing.CallDelayed(1, () => { CanHit = true; });
                }
            }
        }
    }

    public class Gtool : MonoBehaviour
    {
        public List<Hit> hits = new List<Hit>();

        void Check(Player player)
        {
            if (player.IsDead || player.IsCuffed)
                return;

            FriendlyFire FF = FriendlyFire.Instance;

            if (Physics.Raycast(player.Position, Vector3.down, out RaycastHit hit, 1, (LayerMask)1))
            {
                string pos = hit.transform.parent.name;
            }
        }

        void Update()
        {
            hits.RemoveAll(x => x.player == null);

            foreach (var p in hits)
            {
                try
                {
                    p.Update();
                    Check(p.player);
                }
                catch (Exception ex)
                {

                }
            }
        }
    }

    class FriendlyFire
    {
        public static FriendlyFire Instance;
        public Gtool gtool;

        public void OnEnabled()
        {
            Server.FriendlyFire = true;

            Exiled.Events.Handlers.Server.RoundStarted += OnRoundStarted;

            Exiled.Events.Handlers.Player.Verified += OnVerified;
        }

        public void OnMelee(Player player)
        {
            if (player.IsDead || player.IsCuffed)
                return;

            if (Physics.Raycast(player.ReferenceHub.PlayerCameraReference.position + player.ReferenceHub.PlayerCameraReference.forward * 0.2f, player.ReferenceHub.PlayerCameraReference.forward, out RaycastHit hit, 1, InventorySystem.Items.Firearms.Modules.StandardHitregBase.HitregMask) &&
                hit.collider.TryGetComponent<IDestructible>(out IDestructible destructible))
            {
                Hitmarker.SendHitmarkerDirectly(player.ReferenceHub, 1f);
                destructible.Damage(1, new PlayerStatsSystem.CustomReasonDamageHandler("무지성으로 뚜드려 맞아 죽었습니다.", 12), hit.point);
            }
        }

        public void OnRoundStarted()
        {
            GameObject gameobject = GameObject.Instantiate(new GameObject());
            gtool = gameobject.AddComponent<Gtool>();

            while (true)
            {
                foreach (var player in Player.List)
                    player.AddItem(ItemType.GunCOM18);
            }
        }

        public void OnVerified(Exiled.Events.EventArgs.Player.VerifiedEventArgs ev)
        {
            gtool.hits.Add(new Hit { player = ev.Player });
        }
    }
}
