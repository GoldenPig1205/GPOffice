using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CustomPlayerEffects;
using CustomRendering;
using Exiled.API.Features;
using Exiled.API.Features.Roles;
using Exiled.API.Features.Doors;
using MapEditorReborn.API.Features.Objects;
using MEC;
using Mirror;
using PlayerRoles;
using UnityEngine;
using UnityEngine.Rendering;

namespace GPOffice.Modes
{
    class TroubleinTerroristTown
    {
        public static TroubleinTerroristTown Instance;

        public Dictionary<string, string> Roles = new Dictionary<string, string>
        {
            { "시민", "lime/탐정을 도와 모든 위협을 제거하세요. 주변을 경계하세요./innocent" },
            { "탐정", "cyan/시민팀을 위협하는 것들을 제거하세요. 모두에게 당신의 직업이 공개됩니다./innocent" },
            { "경찰", "aqua/시민팀을 위협하는 것들을 제거하세요. 모두에게 당신의 직업이 공개됩니다./innocent" },
            { "형사", "blue/시민팀을 위협하는 것들을 제거하세요. 모두에게 당신의 직업이 공개됩니다./innocent" },
            { "배신자", "red/당신과 함께 배신에 가담한 동료들을 제외하고 모두를 제거하세요./traitor" },
            { "매드 사이언티스트", "red/당신과 함께 배신에 가담한 동료들을 제외하고 모두를 제거하세요./traitor" },
            { "암살자", "red/당신과 함께 배신에 가담한 동료들을 제외하고 모두를 제거하세요./traitor" },
            { "스파이", "red/당신과 함께 배신에 가담한 동료들을 제외하고 모두를 제거하세요./traitor" },
            { "짐승인간", "red/당신과 함께 배신에 가담한 동료들을 제외하고 모두를 제거하세요./traitor" },
            { "수감자", "red/당신과 함께 배신에 가담한 동료들을 제외하고 모두를 제거하세요./traitor" },
        };

        public static List<string> CoolInnocentRoles = new List<string>() 
        { 
            "탐정", "경찰", "형사" 
        };

        public static List<string> SpecialTraitorTeamRoles = new List<string>() 
        { 
            "매드 사이언티스트", "암살자", "스파이", "짐승인간", "수감자" 
        };

        public List<string> Queue = new List<string>() 
        {
            "시민", "배신자", "시민", Plugin.GetRandomValue(CoolInnocentRoles), "시민", Plugin.GetRandomValue(SpecialTraitorTeamRoles), "시민", "시민", "시민", "시민",
            "배신자", "시민", "시민", "시민", "시민", Plugin.GetRandomValue(SpecialTraitorTeamRoles), "시민", "시민", "시민", "시민",
            "배신자", "시민", "시민", "시민", "시민", Plugin.GetRandomValue(SpecialTraitorTeamRoles), "시민", "시민", "시민", "시민"
        };

        public List<Player> pl = new List<Player>();
        public Dictionary<Player, string> playerRoles = new Dictionary<Player, string>();
        public Dictionary<Player, string> playerTeams = new Dictionary<Player, string>();

        public static void Shuffle<T>(List<T> list)
        {
            System.Random rng = new System.Random();
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }

        public void OnEnabled()
        {
            Round.IsLocked = true;

            Player.List.ToList().CopyTo(pl);

            Timing.RunCoroutine(OnModeStarted());

            Exiled.Events.Handlers.Player.Died += OnDied;
        }

        public IEnumerator<float> OnModeStarted()
        {
            yield return 0f;

            foreach (var Elevator in ElevatorDoor.List)
                Elevator.Lock(1205, Exiled.API.Enums.DoorLockType.Lockdown079);

            foreach (var Checkpoint in CheckpointDoor.List)
                Checkpoint.Lock(1205, Exiled.API.Enums.DoorLockType.Lockdown079);

            foreach (var player in Player.List)
                player.Position = Plugin.GetRandomValue(Room.List.Where(x => x.Name.Contains("Hcz")).ToList()).Position;

            Shuffle(pl);

            foreach (var player in pl)
            {
                string Role = Queue[pl.IndexOf(player)];
                string RoleColor = Roles[Role].Split('/')[0];
                string RoleDescription = Roles[Role].Split('/')[1];
                string RoleTeam = Roles[Role].Split('/')[2];

                playerRoles.Add(player, Role);
                playerTeams.Add(player, RoleTeam);
                player.ShowHint($"<size=30>당신은 <color={RoleColor}>{Role}</color>입니다.</size>\n<i><size=25>{RoleDescription}</size></i>");

                if (CoolInnocentRoles.Contains(Role))
                    player.Group = new UserGroup { BadgeText = Role, BadgeColor = RoleColor };
            }
        }

        public void OnDied(Exiled.Events.EventArgs.Player.DiedEventArgs ev)
        {
            string Role = Queue[pl.IndexOf(ev.Player)];
            string RoleColor = Roles[Role].Split('/')[0];

            playerTeams.Remove(ev.Player);
            ev.Player.Group = new UserGroup { BadgeText=Role, BadgeColor=RoleColor };
        }

        public async void IsRoundEnd()
        {
            while (true)
            {
                if (playerTeams.Values.Distinct().Count() == 1)
                {
                    foreach (var player in pl)
                    {
                        string Role = Queue[pl.IndexOf(player)];
                        string RoleColor = Roles[Role].Split('/')[0];

                        player.Group = new UserGroup { BadgeText = Role, BadgeColor = RoleColor };
                    }

                    string Team = playerTeams.Values.First();

                    if (Team == "innocent")
                        EndGame("시민", "9AFE2E", "다른 진영의 적들이 더 이상 없습니다.");

                    if (Team == "traitor")
                        EndGame("배신자", "FF0000", "다른 진영의 적들이 더 이상 없습니다.");
                }

                await Task.Delay(1000);
            }
        }

        public void EndGame(string Team, string Color, string Reason)
        {
            Round.IsLocked = false;

            Player.List.ToList().ForEach(x => x.Broadcast(20, $"<size=20>{Reason}</size>\n<size=25><b><color=#{Color}>{Team}</color>의 승리입니다.</b></size>"));
        }
    }
}
