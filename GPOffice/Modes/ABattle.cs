using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Runtime.Remoting;
using System.Text;
using System.Threading.Tasks;
using CommandSystem.Commands.RemoteAdmin;
using Exiled.API.Extensions;
using Exiled.API.Features;
using Exiled.Loader.Models;
using InventorySystem;
using InventorySystem.Items.Usables.Scp330;
using MapEditorReborn.API.Features.Objects;
using UnityEngine;

namespace GPOffice.Modes
{
    class ABattle
    {
        public static ABattle Instance;

        public Dictionary<string, List<Vector3>> PlayerWorkstation = new Dictionary<string, List<Vector3>>();
        public Dictionary<string, List<string>> PlayerAbilities = new Dictionary<string, List<string>>();

        public List<string> BlackOutCooldown = new List<string>();

        public Dictionary<string, string> CommonAbilities = new Dictionary<string, string>()
                    {
                        {"[일반] 운동", "25HP만큼 최대 체력을 추가합니다."},
                        {"[일반] 경공", "이동 속도가 25% 증가합니다."},
                        {"[일반] 진화", "몸의 크기가 12% 작아집니다."},
                        {"[일반] 단련", "공격력이 20% 추가됩니다."},
                        {"[일반] 잠수", "스태미나가 줄어들지 않습니다."},
                        {"[일반] 행운", "5% 확률로 잠긴 문을 엽니다."},
                        {"[일반] 체력 보충", "75AHP를 받습니다."},
                        {"[일반] 랜덤박스", "랜덤한 아이템을 지급받습니다."}
                    };
        public Dictionary<string, string> RareAbilities = new Dictionary<string, string>()
                    {
                        {"[희귀] 육체 강화", "1초당 1HP를 받습니다."},
                        {"[희귀] 블랙아웃", "점프하면 해당 장소를 3초동안 정전시킵니다. [쿨타임 20초]"},
                        {"[희귀] 강철 껍질", "데미지 경감 효과를 1 받습니다."},
                        {"[희귀] 투명 망토", "25초 간 투명 효과를 받습니다."},
                        {"[희귀] 흡혈귀", "상대에게 입힌 피해량의 20%만큼 AHP를 받습니다."},
                        {"[희귀] 순간이동", "랜덤한 유저의 위치로 순간이동합니다."}
                    };
        public Dictionary<string, string> EpicAbilities = new Dictionary<string, string>()
                    {
                        {"[영웅] 테러리스트의 유품", "핑크 사탕을 지급받습니다."},
                        {"[영웅] 도박꾼", "아이템을 버리면 새로운 아이템을 받지만, 5% 확률로 손이 잘립니다!"},
                        {"[영웅] 랜덤상자", "랜덤하지만 좋은 아이템을 지급받습니다."},
                        {"[영웅] 핵 리모컨", "핵 프로세스를 시작합니다."},
                        {"[영웅] 수리 기사", "모든 엘레베이터를 15초 간 고장냅니다."},
                        {"[영웅] 슈퍼 스타", "자신의 마이크가 모두에게 공유됩니다."}
                    };
        public Dictionary<string, string> LegendAbilities = new Dictionary<string, string>()
                    {
                        {"[전설] 해킹", "시설 핵을 즉시 터트립니다."},
                        {"[전설] 스피드왜건", "속도가 크게 증가합니다."}
                    };

        public void OnEnabled()
        {
            Task.WhenAll(
                OnModeStarted(),
                UpgradeBody(),
                UpgradeStamina()
                );

            Exiled.Events.Handlers.Player.Jumping += OnJumping;
            Exiled.Events.Handlers.Player.Dying += OnDying;
            Exiled.Events.Handlers.Player.InteractingDoor += InteractingDoor;
            Exiled.Events.Handlers.Player.DroppedItem += DroppedItem;
            Exiled.Events.Handlers.Player.Hurting += Hurting;
        }

        public async Task OnModeStarted()
        {
            Server.ExecuteCommand($"/mp load ABattle");

            while (true)
            {
                foreach (var player in Player.List)
                {
                    if (!PlayerWorkstation.ContainsKey(player.UserId))
                    {
                        PlayerWorkstation.Add(player.UserId, new List<Vector3>());
                        PlayerAbilities.Add(player.UserId, new List<string>());
                    }
                    else
                    {
                        if (!player.IsDead)
                        {
                            if (PlayerAbilities[player.UserId].Count <= 0)
                                player.ShowHint($"<align=left><b><size=22>워크스테이션 위에서 점프하면 능력을 획득할 수 있습니다.</size></b></align>", 1.2f);
                            else
                            {
                                string abilitiesText = string.Join(", ", PlayerAbilities[player.UserId]);
                                abilitiesText = abilitiesText.Replace("[전설]", "<color=#ffd700>[전설]</color>");
                                abilitiesText = abilitiesText.Replace("[영웅]", "<color=#FF00FF>[영웅]</color>");
                                abilitiesText = abilitiesText.Replace("[희귀]", "<color=#2ECCFA>[희귀]</color>");
                                abilitiesText = abilitiesText.Replace("[일반]", "<color=#A4A4A4>[일반]</color>");

                                player.ShowHint($"<align=left><b><size=25>보유 능력</size></b>\n<size=20>{abilitiesText}</size></align>", 1.2f);
                            }
                        }
                    }

                }

                await Task.Delay(1000);
            }
        }

        public async Task UpgradeBody()
        {
            while (true)
            {
                foreach (var player in Player.List)
                {
                    if (PlayerAbilities[player.UserId].Contains("[희귀] 육체 강화"))
                        if (player.MaxHealth > player.Health)
                            player.Health += 1;
                }
                await Task.Delay(1000);
            }
        }

        public async Task UpgradeStamina()
        {
            while (true)
            {
                foreach (var player in Player.List)
                {
                    if (PlayerAbilities[player.UserId].Contains("[일반] 잠수"))
                        if (player.IsUsingStamina)
                            player.Stamina += 0.1f;
                }
                await Task.Delay(1000);
            }
        }

        public async void OnJumping(Exiled.Events.EventArgs.Player.JumpingEventArgs ev)
        {
            if (Physics.Raycast(ev.Player.Position, Vector3.down, out RaycastHit hit, 5, (LayerMask)1))
            {
                Transform WorkStation = hit.transform.parent.parent;

                if (WorkStation.name.Contains("Work Station") && !PlayerWorkstation[ev.Player.UserId].Contains(WorkStation.position))
                {
                    PlayerWorkstation[ev.Player.UserId].Add(WorkStation.position);

                    int grade = UnityEngine.Random.Range(1, 101);
                    string abilityGrade;

                    if (grade <= 60)
                        abilityGrade = "[일반]";
                    else if (grade <= 90)
                        abilityGrade = "[희귀]";
                    else if (grade <= 99)
                        abilityGrade = "[영웅]";
                    else
                        abilityGrade = "[전설]";

                    Dictionary<string, string> AbilityList()
                    {
                        if (abilityGrade == "[일반]")
                            return CommonAbilities;
                        else if (abilityGrade == "[희귀]")
                            return RareAbilities;
                        else if (abilityGrade == "[영웅]")
                        {
                            Cassie.Clear();
                            Server.ExecuteCommand($"/cassie_sl {ev.Player.DisplayNickname}(이)가 <color=#FF00FF>[영웅]</color> 업그레이드를 입수하였습니다.");
                            return EpicAbilities;
                        }
                        else
                        {
                            Cassie.Clear();
                            Server.ExecuteCommand($"/cassie_sl {ev.Player.DisplayNickname}(이)가 <color=#ffd700>[전설]</color> 업그레이드를 입수하였습니다.");
                            return LegendAbilities;
                        }
                    }
                     
                    void ApplyGiveAbility(string abilityName)
                    {
                        PlayerAbilities[ev.Player.UserId].Add(abilityName);
                        string styleName = abilityName.Replace("[전설]", "<color=#ffd700>[전설]</color>").Replace("[영웅]", "<color=#FF00FF>[영웅]</color>").Replace("[희귀]", "<color=#2ECCFA>[희귀]</color>").Replace("[일반]", "<color=#A4A4A4>[일반]</color>");
                        ev.Player.ClearBroadcasts();
                        ev.Player.Broadcast(8, $"<size=20><b>다음 능력이 추가되었습니다.</b></size>\n<size=30>{styleName}</size>\n<size=25>{AbilityList()[abilityName]}</size>");
                    }

                    string abilityName = GetRandomValue(AbilityList().Keys.ToList());

                    ApplyGiveAbility(abilityName);

                    string aT = abilityName.Replace("[일반] ", "").Replace("[희귀] ", "").Replace("[영웅] ", "").Replace("[전설] ", "");

                    switch (aT)
                    {
                        case "운동": ev.Player.MaxHealth += 25; ev.Player.Health += 25; break;
                        case "경공": ev.Player.GetEffect(Exiled.API.Enums.EffectType.MovementBoost).Intensity += 10; break;
                        case "진화": ev.Player.Scale = new Vector3(ev.Player.Scale.x - 0.12f, ev.Player.Scale.y - 0.12f, ev.Player.Scale.z - 0.12f); break;
                        case "잠수": ev.Player.StaminaStat.CurValue += 20; break;
                        case "체력 보충": ev.Player.ArtificialHealth += 75; break;
                        case "강철 껍질": ev.Player.EnableEffect(Exiled.API.Enums.EffectType.DamageReduction, 1); break;
                        case "투명 망토": ev.Player.EnableEffect(Exiled.API.Enums.EffectType.Invisible, 25); break;
                        case "순간이동":
                            Player target = GPOffice.GetRandomValue(Player.List.ToList());
                            ev.Player.Position = target.Position;
                            break;
                        case "랜덤박스":
                            int rn = UnityEngine.Random.Range(0, 55);

                            Server.ExecuteCommand($"/give {ev.Player.Id} {rn}"); 
                            if (ev.Player.IsScp)
                            {
                                Server.ExecuteCommand($"/forceeq {ev.Player.Id} {rn}");
                            }
                            break;
                        case "테러리스트의 유품": ev.Player.TryAddCandy(CandyKindID.Pink); break;
                        case "랜덤상자":
                            int rn1 = GPOffice.GetRandomValue(new List<int> { 11, 16, 18, 24, 31, 32, 44, 45, 47, 48, 49, 50, 51, 52, 53 });

                            Server.ExecuteCommand($"/give {ev.Player.Id} {rn1}");
                            if (ev.Player.IsScp)
                            {
                                Server.ExecuteCommand($"/forceeq {ev.Player.Id} {rn1}");
                            }
                            break;
                        case "핵 리모컨": Warhead.Start(); Cassie.Clear(); Server.ExecuteCommand($"/cassie_sl {ev.Player.DisplayNickname}(이)가 핵을 원격으로 활성화했습니다!"); break;
                        case "수리 기사": Server.ExecuteCommand("/el l all"); await Task.Delay(15000); Server.ExecuteCommand("/el u all"); break;
                        case "슈퍼 스타": Server.ExecuteCommand($"/speak {ev.Player.Id} enable"); break;
                        case "해킹": Warhead.Detonate(); Cassie.Clear(); Server.ExecuteCommand($"/cassie_sl {ev.Player.DisplayNickname}(이)가 핵을 원격으로 터트렸습니다!"); break;
                        case "스피드왜건": ev.Player.GetEffect(Exiled.API.Enums.EffectType.MovementBoost).Intensity += 100; break;
                    }
                }
            }

            if (PlayerAbilities[ev.Player.UserId].Contains("[희귀] 블랙아웃"))
            {
                if (!BlackOutCooldown.Contains(ev.Player.UserId))
                {
                    BlackOutCooldown.Add(ev.Player.UserId);
                    ev.Player.CurrentRoom.TurnOffLights(3);
                    await Task.Delay(20000);
                    BlackOutCooldown.Remove(ev.Player.UserId);
                }
            }
        }

        public void OnDying(Exiled.Events.EventArgs.Player.DyingEventArgs ev)
        {
            if (PlayerWorkstation.ContainsKey(ev.Player.UserId))
            {
                PlayerAbilities[ev.Player.UserId].Clear();
                PlayerWorkstation[ev.Player.UserId].Clear();
                ev.Player.Scale = new Vector3(1, 1, 1);
                Server.ExecuteCommand($"/speak {ev.Player.Id} disable");

                if (BlackOutCooldown.Contains(ev.Player.UserId))
                    BlackOutCooldown.Remove(ev.Player.UserId);
            }
        }

        public T GetRandomValue<T>(List<T> list)
        {
            int index = UnityEngine.Random.Range(0, list.Count);
            return list[index];
        }

        public void InteractingDoor(Exiled.Events.EventArgs.Player.InteractingDoorEventArgs ev)
        {
            if (ev.Player != null && PlayerAbilities[ev.Player.UserId].Contains("[일반] 행운") && !ev.Door.IsOpen && UnityEngine.Random.Range(0, 100) <= 5)
                ev.Door.IsOpen = true;
        }

        public void DroppedItem(Exiled.Events.EventArgs.Player.DroppedItemEventArgs ev)
        {
            if (PlayerAbilities[ev.Player.UserId].Contains("[영웅] 도박꾼"))
            {
                if (UnityEngine.Random.Range(0, 100) <= 5)
                    ev.Player.Kill("과도한 욕심은 화를 불러일으킨다구요!");

                else
                {
                    ev.Pickup.Destroy();
                    Server.ExecuteCommand($"/drop {ev.Player.Id} {UnityEngine.Random.Range(0, 55)} 1");
                }
            }
        }

        public void Hurting(Exiled.Events.EventArgs.Player.HurtingEventArgs ev)
        {
            if (PlayerAbilities[ev.Player.UserId].Contains("[일반] 단련"))
            {
                int count = PlayerAbilities.Values.Count(list => list.Contains("[일반] 단련"));

                ev.DamageHandler.Damage = (int)(ev.DamageHandler.Damage * (1 + (0.2 * count)));
            }

            if (PlayerAbilities[ev.Attacker.UserId].Contains("[희귀] 흡혈귀"))
                ev.Attacker.AddAhp(20 * (ev.DamageHandler.Damage / 100));
        }
    }
}
