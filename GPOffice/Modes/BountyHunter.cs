using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Exiled.API.Features;
using MEC;
using Mirror;
using UnityEngine;

namespace GPOffice.Modes
{
    class BountyHunter
    {
        public static BountyHunter Instance;

        public List<string> pl = new List<string>();
        Player target = null;

        public void OnEnabled()
        {
            Server.FriendlyFire = true;

            Timing.RunCoroutine(OnModeStarted());

            Exiled.Events.Handlers.Player.Died += OnDied;
        }

        public IEnumerator<float> OnModeStarted()
        {
            yield return Timing.WaitForSeconds(10f);

            target = GPOffice.GetRandomValue(Player.List.Where(x => !x.IsScp).ToList());
            target.Group = new UserGroup { BadgeColor="red", BadgeText="표적" };

            Player.List.ToList().ForEach(x => x.Broadcast(10, $"<size=30><b><color=#FF8000>{target.DisplayNickname}</color>(이)가 현상금 수배자입니다.</b></size>\n<size=25>- 위치 정보가 실시간으로 전송됩니다. -</size>"));
        
            while (true)
            {
                foreach (var player in Player.List)
                    player.ShowHint($"<color=red>표적</color>({target.DisplayNickname})은 현재 {target.CurrentRoom.Name}에 있습니다.");

                yield return Timing.WaitForSeconds(1f);
            }
        }

        public void OnDied(Exiled.Events.EventArgs.Player.DiedEventArgs ev)
        {
            if (ev.Player == target)
            {
                string AttackerName()
                {
                    if (ev.Attacker != null)
                        return ev.Attacker.DisplayNickname;

                    else
                        return "알 수 없음";
                }

                foreach (var player in Player.List)
                {
                    player.ClearBroadcasts();
                    player.Broadcast(15, $"<size=25><b><color=red>표적</color>({target.DisplayNickname})이(가) 잡혔습니다!\n{AttackerName()}의 승리입니다!</b></size>");

                    if (player.IsAlive)
                        Server.ExecuteCommand($"/fc {player.Id} Tutorial 1");
                }
            }
        }
    }
}
