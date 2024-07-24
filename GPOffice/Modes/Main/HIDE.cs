using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CustomPlayerEffects;
using CustomRendering;
using Exiled.API.Features;
using Exiled.API.Features.Roles;
using MEC;
using Mirror;
using PlayerRoles;
using UnityEngine;

namespace GPOffice.Modes
{
    class HIDE
    {
        public static HIDE Instance;

        public List<Player> pl = new List<Player>();
        public Player monster = null;
        public float invisible = 1f;


        public void OnEnabled()
        {
            Respawn.TimeUntilNextPhase = 10000;
            Server.ExecuteCommand($"/el l all");
            Server.ExecuteCommand($"/close **");
            Server.ExecuteCommand($"/lock **");

            Task.WhenAll(
                Timer()
                );

            Timing.RunCoroutine(OnModeStarted());

            Exiled.Events.Handlers.Player.Hurting += OnHurting;
        }

        public async Task Timer()
        {
            for (int i=1; i<180; i++)
            {
                Player.List.ToList().ForEach(x => x.Broadcast(2, $"<size=25><color=#2ECCFA>NTF 승리</color>까지</color> {180 - i}초</size>", shouldClearPrevious: true));
                await Task.Delay(1000);
            }

            foreach (var player in Player.List)
            {
                if (player.IsScp)
                    player.Kill("제한시간이 초과하였습니다.");
            }
        }

        public IEnumerator<float> OnModeStarted()
        {
            Server.ExecuteCommand($"/mp load container");

            Player.List.ToList().CopyTo(pl);
            monster = Plugin.GetRandomValue(Player.List.ToList());

            try
            {
                Timing.CallDelayed(1f, () =>
                {
                    monster.Role.Set(RoleTypeId.Scp3114);
                    monster.Group = new UserGroup { BadgeText="MONSTER", BadgeColor="red" };
                    monster.Position = new Vector3(-15.84375f, 1001.957f, 49.89063f);
                    Server.ExecuteCommand($"/open ESCAPE_PRIMARY");

                    float health = 150 * Player.List.Count + 50 * Player.List.Count;
                    monster.MaxHealth = health;
                    monster.Health = health;
                    monster.IsUsingStamina = false;

                    foreach (var player in Player.List)
                    {
                        if (player != monster)
                        {
                            player.Role.Set(RoleTypeId.NtfCaptain);
                            player.Position = new Vector3(18.08594f, 1001.957f, 15.34766f);
                            for (int i = 1; i < 10; i++)
                                player.AddItem(ItemType.Ammo556x45);
                        }
                    }
                });
            }
            catch (Exception e)
            {
                ServerConsole.AddLog(e.ToString());
            }

            while (true)
            {
                if (invisible > 0)
                {
                    invisible -= 0.1f;

                    monster.DisableEffect(Exiled.API.Enums.EffectType.Invisible);
                }

                else
                    monster.EnableEffect(Exiled.API.Enums.EffectType.Invisible);

                yield return Timing.WaitForSeconds(0.1f);
            }
        }

        public void OnHurting(Exiled.Events.EventArgs.Player.HurtingEventArgs ev)
        {
            if (ev.Player == monster || ev.Attacker == monster)
            {
                invisible = 1f;
                monster.HumeShield = 0;
            }
            if (ev.Attacker.IsScp && ev.DamageHandler.Type != Exiled.API.Enums.DamageType.Strangled)
                ev.DamageHandler.Damage += 15;
        }
    }
}
