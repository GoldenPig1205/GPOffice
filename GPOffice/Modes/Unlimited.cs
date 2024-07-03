using System.Collections.Generic;
using Exiled.API.Features;
using Exiled.Events.EventArgs.Scp173;
using MEC;

namespace GPOffice.Modes
{
    class Unlimited
    {
        public static Unlimited Instance;

        public int Tantrum = 0;

        public void OnEnabled()
        {
            Exiled.Events.Handlers.Player.Spawned += OnSpawned;
            Exiled.Events.Handlers.Player.DroppingItem += OnDroppingItem;

            Exiled.Events.Handlers.Scp106.Teleporting += OnTeleporting;
            Exiled.Events.Handlers.Scp106.Stalking += OnStalking;
            Exiled.Events.Handlers.Scp106.Attacking += OnScp106Attacking;

            Exiled.Events.Handlers.Scp939.PlayingSound += OnPlayingSound;

            Exiled.Events.Handlers.Scp079.ChangingCamera += OnChangingCamera;

            Exiled.Events.Handlers.Scp049.StartingRecall += OnStartingRecall;
            Exiled.Events.Handlers.Scp049.Attacking += OnScp049Attacking;

            Exiled.Events.Handlers.Scp096.Enraging += OnEnraging;

            Exiled.Events.Handlers.Scp173.PlacingTantrum += OnPlacingTantrum;
            Exiled.Events.Handlers.Scp173.UsingBreakneckSpeeds += OnUsingBreakneckSpeeds;

            Exiled.Events.Handlers.Player.SearchingPickup += OnSearchingPickup;
            Exiled.Events.Handlers.Player.Shooting += OnShooting;
            Exiled.Events.Handlers.Player.ChangingMicroHIDState += OnChangingMicroHIDState;
            Exiled.Events.Handlers.Player.UsingMicroHIDEnergy += OnUsingMicroHIDEnergy;

            Exiled.Events.Handlers.Item.ChargingJailbird += OnChargingJailbird;
        }

        public void OnSpawned(Exiled.Events.EventArgs.Player.SpawnedEventArgs ev)
        {
            ev.Player.MaxHealth = 30000;
            ev.Player.IsUsingStamina = false;
        }

        public void OnDroppingItem(Exiled.Events.EventArgs.Player.DroppingItemEventArgs ev)
        {
            if (ev.Item.Type == ItemType.GrenadeHE)
            {
                if (UnityEngine.Random.Range(1, 50) == 1)
                    Server.ExecuteCommand($"/rocket {ev.Player.Id} 0.1");
                else
                    ev.Player.ShowHint($"<color=red><i><size=20>\"불길한 느낌이 들어..\"</size></i></color>", 2);
            }
        }

        public async void OnTeleporting(Exiled.Events.EventArgs.Scp106.TeleportingEventArgs ev)
        {
            Timing.CallDelayed(0.1f, () =>
            {
                ev.Scp106.RemainingSinkholeCooldown = 0;
            });
        }

        public async void OnStalking(Exiled.Events.EventArgs.Scp106.StalkingEventArgs ev)
        {
            Timing.CallDelayed(0.1f, () =>
            {
                ev.Scp106.RemainingSinkholeCooldown = 0;
            });
        }

        public async void OnScp106Attacking(Exiled.Events.EventArgs.Scp106.AttackingEventArgs ev)
        {
            Timing.CallDelayed(0.1f, () =>
            {
                ev.Scp106.CaptureCooldown = 0;
            });
        }

        public async void OnPlayingSound(Exiled.Events.EventArgs.Scp939.PlayingSoundEventArgs ev)
        {
            Timing.CallDelayed(0.1f, () =>
            {
                ev.Scp939.MimicryCooldown = 0;
            });
        }

        public void OnChangingCamera(Exiled.Events.EventArgs.Scp079.ChangingCameraEventArgs ev)
        {
            ev.Scp079.Energy = 100000;
        }

        public void OnStartingRecall(Exiled.Events.EventArgs.Scp049.StartingRecallEventArgs ev)
        {
            Timing.CallDelayed(0.1f, () =>
            {
                ev.Scp049.CallCooldown = 0;
            });
        }

        public void OnScp049Attacking(Exiled.Events.EventArgs.Scp049.AttackingEventArgs ev)
        {
            Timing.CallDelayed(0.1f, () =>
            {
                ev.Scp049.RemainingAttackCooldown = 0;
                ev.Scp049.GoodSenseCooldown = 0;
            });
        }

        public void OnEnraging(Exiled.Events.EventArgs.Scp096.EnragingEventArgs ev)
        {
            Timing.CallDelayed(0.1f, () =>
            {
                ev.Scp096.EnrageCooldown = 0;
                ev.Scp096.EnragedTimeLeft = 99999;
                ev.Scp096.SprintingSpeed = 500;
            });
        }

        public void OnPlacingTantrum(Exiled.Events.EventArgs.Scp173.PlacingTantrumEventArgs ev)
        {
            Timing.RunCoroutine(TantrumCooldown(ev));
        }

        private IEnumerator<float> TantrumCooldown(PlacingTantrumEventArgs ev)
        {
            if (Tantrum >= 10)
            {
                ev.Player.ShowHint($"렉 방지를 위해 10개로 제한됩니다. (하나 당 180초)", 1);
                ev.IsAllowed = false;
            }
            else
            {
                Tantrum += 1;
                yield return Timing.WaitForSeconds(0.1f);
                ev.Cooldown.Remaining = 0;
                yield return Timing.WaitForSeconds(180f);
                Tantrum -= 1;
            }
        }

        public async void OnUsingBreakneckSpeeds(Exiled.Events.EventArgs.Scp173.UsingBreakneckSpeedsEventArgs ev)
        {
            Timing.CallDelayed(0.1f, () =>
            {
                ev.Scp173.RemainingBreakneckCooldown = 0;
            });
        }

        public void OnSearchingPickup(Exiled.Events.EventArgs.Player.SearchingPickupEventArgs ev)
        {
            ev.IsAllowed = false;
            ev.Player.AddItem(ev.Pickup);

            if (UnityEngine.Random.Range(1, 50) == 1)
                Server.ExecuteCommand($"/rocket {ev.Player.Id} 0.1");
            else
                ev.Player.ShowHint($"<color=red><i><size=20>\"불길한 느낌이 들어..\"</size></i></color>", 2);
        }

        public void OnShooting(Exiled.Events.EventArgs.Player.ShootingEventArgs ev)
        {
            ev.Player.CurrentItem.As<Exiled.API.Features.Items.Firearm>().Ammo += 250;
        }

        public void OnChangingMicroHIDState(Exiled.Events.EventArgs.Player.ChangingMicroHIDStateEventArgs ev)
        {
            ev.MicroHID.Energy += 100;
        }

        public void OnUsingMicroHIDEnergy(Exiled.Events.EventArgs.Player.UsingMicroHIDEnergyEventArgs ev)
        {
            ev.MicroHID.Energy += 100;
        }

        public void OnChargingJailbird(Exiled.Events.EventArgs.Item.ChargingJailbirdEventArgs ev)
        {
            ev.Item.As<Exiled.API.Features.Items.Jailbird>().TotalCharges = 0;
        }
    }
}
