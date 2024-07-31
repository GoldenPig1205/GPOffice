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
using Exiled.API.Features.Items;
using Exiled.Loader.Models;
using InventorySystem;
using InventorySystem.Items.Coin;
using InventorySystem.Items.Usables.Scp330;
using MapEditorReborn.API.Features.Objects;
using MEC;
using PluginAPI.Roles;
using UnityEngine;

namespace GPOffice.Modes
{
    class ABattle
    {
        public static ABattle Instance;

        public Dictionary<string, List<Vector3>> PlayerWorkstation = new Dictionary<string, List<Vector3>>();
        public Dictionary<string, List<string>> PlayerAbilities = new Dictionary<string, List<string>>();

        public List<string> BlackOutCooldown = new List<string>();
        public List<ushort> GrapCoinSerials = new List<ushort>();

        public List<Player> spirits = new List<Player>();
        public List<Player> repairs = new List<Player>();
        public List<Player> magicians = new List<Player>();
        public List<Player> posions = new List<Player>();

        public Dictionary<string, string> CommonAbilities = new Dictionary<string, string>()
        {
            {"[일반] 운동", "25HP만큼 최대 체력을 추가합니다."},
            {"[일반] 경공", "이동 속도가 25% 증가합니다."},
            {"[일반] 진화", "몸의 크기가 12% 작아집니다."},
            {"[일반] 단련", "공격력이 20% 추가됩니다."},
            {"[일반] 잠수", "스태미나가 줄어들지 않습니다."},
            {"[일반] 행운", "5% 확률로 잠긴 문을 열 수 있습니다."},
            {"[일반] 체력 보충", "75AHP를 받습니다."},
            {"[일반] 랜덤박스", "랜덤한 아이템을 지급받습니다."},
            {"[일반] 위치 추적", "10초 간 랜덤한 1인의 위치를 확인합니다."},
            {"[일반] 광고", "현재 진영 정보를 C.A.S.S.I.E 로 출력합니다."}
        };
        public Dictionary<string, string> RareAbilities = new Dictionary<string, string>()
        {
            {"[희귀] 육체 강화", "1초당 1HP를 받습니다."},
            {"[희귀] 블랙아웃", "점프하면 해당 장소를 3초동안 정전시킵니다. [쿨타임 20초]"},
            {"[희귀] 강철 껍질", "데미지 경감 효과를 1 받습니다."},
            {"[희귀] 투명 망토", "25초 간 투명 효과를 받습니다."},
            {"[희귀] 흡혈귀", "상대에게 입힌 피해량의 20%만큼 AHP를 받습니다."},
            {"[희귀] 순간이동", "랜덤한 유저의 위치로 순간이동합니다."},
            {"[희귀] 봄버맨", "랜덤한 유저의 위치에 고폭 수류탄을 투척합니다."},
            {"[희귀] 갈고리", "지급된 동전을 튕기면 랜덤한 1인을 끌어옵니다."}
        };
        public Dictionary<string, string> EpicAbilities = new Dictionary<string, string>()
        {
            {"[영웅] 테러리스트의 유품", "핑크 사탕을 지급받습니다."},
            {"[영웅] 도박꾼", "아이템을 버리면 새로운 아이템을 받지만, 5% 확률로 손이 잘립니다!"},
            {"[영웅] 랜덤상자", "랜덤하지만 좋은 아이템을 지급받습니다."},
            {"[영웅] 핵 리모컨", "핵 프로세스를 시작합니다."},
            {"[영웅] 수리 기사", "모든 엘레베이터를 15초 간 고장내고, 모든 잠겨진 문에 액세스할 수 있으며, 테슬라를 작동시키지 않습니다."},
            {"[영웅] 슈퍼 스타", "자신의 마이크가 모두에게 공유됩니다."},
            {"[영웅] 럭키비키", "이전에 방문했던 워크스테이션에서 다시 한번 더 능력을 획득할 수 있습니다."},
            {"[영웅] 극독", "누군가에게 죽으면 죽인 자에게 심장 마비 효과를 겁니다."}
        };
        public Dictionary<string, string> LegendAbilities = new Dictionary<string, string>()
        {
            {"[전설] 스피드왜건", "속도가 크게 증가합니다."},
            {"[전설] 뱀의 손 무전기", "즉시 뱀의 손 지원을 부르며, 자신도 뱀의 손 소속이 됩니다."},
            {"[전설] 모드 설치", "다른 모드를 하나 더 불러옵니다."},
            {"[전설] 랜덤택배", "서버 인원 수 만큼 랜덤한 아이템을 드롭합니다."},
            {"[전설] 마술사", "누군가에게 죽으면 죽인 자와 교체됩니다."}
        };
        public Dictionary<string, string> MythicAbilities = new Dictionary<string, string>()
        {
            {"[신화] 해킹", "시설 핵을 즉시 터트립니다."},
            {"[신화] 로켓 런처", "상대방을 한방에 보내버릴 수 있습니다."},
            {"[신화] 스피릿", "영혼 상태가 됩니다."}
        };

        public void OnEnabled()
        {
            Task.WhenAll(
                OnModeStarted(),
                UpgradeBody()
                );

            Timing.RunCoroutine(Spirit());

            Exiled.Events.Handlers.Player.Jumping += OnJumping;
            Exiled.Events.Handlers.Player.FlippingCoin += OnFlippingCoin;
            Exiled.Events.Handlers.Player.ChangedItem += OnChangedItem;
            Exiled.Events.Handlers.Player.Dying += OnDying;
            Exiled.Events.Handlers.Player.InteractingDoor += InteractingDoor;
            Exiled.Events.Handlers.Player.InteractingLocker += InteractingLocker;
            Exiled.Events.Handlers.Player.DroppedItem += DroppedItem;
            Exiled.Events.Handlers.Player.Hurting += Hurting;
            Exiled.Events.Handlers.Player.ChangingSpectatedPlayer += ChangingSpectatedPlayer;
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
                                abilitiesText = abilitiesText.Replace("[신화]", "<color=#DF0101>[신화]</color>").Replace("[전설]", "<color=#ffd700>[전설]</color>").Replace("[영웅]", "<color=#FF00FF>[영웅]</color>").Replace("[희귀]", "<color=#2ECCFA>[희귀]</color>").Replace("[일반]", "<color=#A4A4A4>[일반]</color>");

                                player.ShowHint($"<align=left><b><size=25>보유 업그레이드</size></b>\n<size=20>{abilitiesText}</size></align>", 1.2f);
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

        public IEnumerator<float> Spirit()
        {
            while (true)
            {
                foreach (var player in Player.List)
                {
                    if (spirits.Contains(player))
                        player.EnableEffect(Exiled.API.Enums.EffectType.Invisible);
                }

                yield return Timing.WaitForSeconds(1f);
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

                    int grade = UnityEngine.Random.Range(1, 1001);
                    string abilityGrade;

                    if (grade <= 600)
                        abilityGrade = "[일반]";
                    else if (grade <= 900)
                        abilityGrade = "[희귀]";
                    else if (grade <= 990)
                        abilityGrade = "[영웅]";
                    else if (grade <= 998)
                        abilityGrade = "[전설]";
                    else
                        abilityGrade = "[신화]";

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
                        else if (abilityGrade == "[전설]")
                        {
                            Cassie.Clear();
                            Server.ExecuteCommand($"/cassie_sl {ev.Player.DisplayNickname}(이)가 <color=#ffd700>[전설]</color> 업그레이드를 입수하였습니다.");
                            return LegendAbilities;
                        }
                        else
                        {
                            Cassie.Clear();
                            Server.ExecuteCommand($"/cassie_sl {ev.Player.DisplayNickname}(이)가 <color=#DF0101>[신화]</color> 업그레이드를 입수하였습니다.");
                            return MythicAbilities;
                        }
                    }

                    void ApplyGiveAbility(string abilityName)
                    {
                        PlayerAbilities[ev.Player.UserId].Add(abilityName);
                        string styleName = abilityName.Replace("[신화]", "<color=#DF0101>[신화]</color>").Replace("[전설]", "<color=#ffd700>[전설]</color>").Replace("[영웅]", "<color=#FF00FF>[영웅]</color>").Replace("[희귀]", "<color=#2ECCFA>[희귀]</color>").Replace("[일반]", "<color=#A4A4A4>[일반]</color>");
                        ev.Player.ClearBroadcasts();
                        ev.Player.Broadcast(8, $"<size=20><b>다음 능력이 추가되었습니다.</b></size>\n<size=30>{styleName}</size>\n<size=25>{AbilityList()[abilityName]}</size>");
                    }

                    string abilityName = GetRandomValue(AbilityList().Keys.ToList());

                    ApplyGiveAbility(abilityName);

                    string aT = abilityName.Replace("[일반] ", "").Replace("[희귀] ", "").Replace("[영웅] ", "").Replace("[전설] ", "").Replace("[신화] ", "");

                    switch (aT)
                    {
                        case "운동": ev.Player.MaxHealth += 25; ev.Player.Health += 25; break;
                        case "경공": ev.Player.GetEffect(Exiled.API.Enums.EffectType.MovementBoost).Intensity += 10; break;
                        case "진화": ev.Player.Scale = new Vector3(ev.Player.Scale.x - 0.12f, ev.Player.Scale.y - 0.12f, ev.Player.Scale.z - 0.12f); break;
                        case "잠수": ev.Player.IsUsingStamina = false; break;
                        case "체력 보충": ev.Player.ArtificialHealth += 75; break;
                        case "랜덤박스":
                            int rn = UnityEngine.Random.Range(0, 55);

                            Server.ExecuteCommand($"/give {ev.Player.Id} {rn}");
                            if (ev.Player.IsScp)
                            {
                                Server.ExecuteCommand($"/forceeq {ev.Player.Id} {rn}");
                            }
                            break;
                        case "위치 추적":
                            Player target1 = Plugin.GetRandomValue(Player.List.Where(x => x.IsAlive).ToList());

                            for (int i = 1; i < 11; i++)
                            {
                                ev.Player.ShowHint($"소속이 <color={target1.Role.Color.ToHex()}>{target1.Role.Name}</color>인 ???은(는) <b>{target1.CurrentRoom.Name}</b>에 있습니다.", 1.2f);
                                await Task.Delay(1000);
                            }
                            break;
                        case "광고": Server.ExecuteCommand($"/cassie_sl <b>[<color={ev.Player.Role.Color.ToHex()}>{ev.Player.DisplayNickname}</color>이(가) 출력한 진영 정보]</b>\n <color=red>SCP</color> : {Player.List.Where(x => x.IsScp).Count()} / <color=#088A29>혼돈의 반란</color> : {Player.List.Where(x => x.IsCHI).Count()} / <color=#0080FF>NTF</color> : {Player.List.Where(x => x.IsNTF).Count()}"); break;
                        case "강철 껍질": ev.Player.GetEffect(Exiled.API.Enums.EffectType.DamageReduction).Intensity += 1; break;
                        case "투명 망토": ev.Player.EnableEffect(Exiled.API.Enums.EffectType.Invisible, 1, 25); break;
                        case "순간이동":
                            Player target = Plugin.GetRandomValue(Player.List.Where(x => x != ev.Player && x.IsAlive).ToList());
                            ev.Player.Position = target.Position;
                            break;
                        case "봄버맨":
                            var g = (ExplosiveGrenade)Item.Create(ItemType.GrenadeHE, ev.Player);
                            g.FuseTime = 3f;
                            g.SpawnActive(Plugin.GetRandomValue(Player.List.ToList().Where(x => x.IsAlive && x.Role.Team != ev.Player.Role.Team && ev.Player != x).ToList()).Position, ev.Player);
                            break;
                        case "갈고리":
                            Item gc = ev.Player.AddItem(ItemType.Coin);
                            GrapCoinSerials.Add(gc.Serial);

                            if (ev.Player.IsScp)
                                ev.Player.CurrentItem = gc;
                            break;
                        case "테러리스트의 유품": ev.Player.TryAddCandy(CandyKindID.Pink); break;
                        case "랜덤상자":
                            int rn1 = Plugin.GetRandomValue(new List<int> { 11, 16, 18, 24, 31, 32, 44, 45, 47, 48, 49, 50, 51, 52, 53 });

                            Server.ExecuteCommand($"/give {ev.Player.Id} {rn1}");
                            if (ev.Player.IsScp)
                            {
                                Server.ExecuteCommand($"/forceeq {ev.Player.Id} {rn1}");
                            }
                            break;
                        case "럭키비키": PlayerWorkstation[ev.Player.UserId].Clear(); break;
                        case "핵 리모컨": Warhead.Start(); Server.ExecuteCommand($"/cassie_sl {ev.Player.DisplayNickname}(이)가 핵을 <b>원격으로 활성화했습니다.</b>"); break;
                        case "수리 기사": Server.ExecuteCommand("/el l all"); ev.Player.IsBypassModeEnabled = true; await Task.Delay(15000); Server.ExecuteCommand("/el u all"); break;
                        case "슈퍼 스타": Server.ExecuteCommand($"/speak {ev.Player.Id} enable"); break;
                        case "극독": posions.Add(ev.Player); break;
                        case "스피드왜건": ev.Player.GetEffect(Exiled.API.Enums.EffectType.MovementBoost).Intensity += 100; break;
                        case "모드 설치":
                            object Mode1 = Plugin.GetRandomValue(Plugin.Mods.Keys.ToList());
                            string mod1 = Mode1.ToString();

                            var modeType = Type.GetType($"GPOffice.Modes.{Plugin.Mods[Mode1].ToString().Split('/')[2].Replace(" ", "")}");
                            if (modeType != null)
                            {
                                var modeInstance = Activator.CreateInstance(modeType);
                                var onEnabledMethod = modeType.GetMethod("OnEnabled");
                                onEnabledMethod?.Invoke(modeInstance, null);
                            }

                            Server.ExecuteCommand($"/cassie_sl {ev.Player.DisplayNickname}(이)가 [{mod1}] 모드를 설치했습니다.");
                            break;
                        case "뱀의 손 무전기":
                            Server.ExecuteCommand($"/fc {ev.Player.Id} Tutorial 0");

                            List<Player> SnakeHands = Player.List.Where(x => x.IsDead).ToList();

                            foreach (var player in SnakeHands)
                            {
                                player.Role.Set(PlayerRoles.RoleTypeId.Tutorial);
                                player.Position = new Vector3(-0.08203125f, 1000.96f, 6.828125f);

                                foreach (ItemType Item in new List<ItemType> { ItemType.KeycardFacilityManager, ItemType.GunFSP9, ItemType.GunRevolver, ItemType.Adrenaline, ItemType.AntiSCP207, ItemType.Ammo9x19, ItemType.Ammo44cal })
                                    player.AddItem(Item);
                            }

                            ev.Player.ShowHint($"<i>{SnakeHands.Count()}명의 <color=#FE2EF7>동료</color>들이 당신과 함께합니다..</i>", 5);
                            break;
                        case "랜덤택배":
                            for (int i = 1; i < Player.List.Count() + 1; i++)
                                Server.ExecuteCommand($"/drop {ev.Player.Id} {UnityEngine.Random.Range(1, 55)} 1");
                            break;
                        case "마술사": magicians.Add(ev.Player); break;
                        case "해킹": Warhead.Start(); Warhead.Detonate(); Server.ExecuteCommand($"/cassie_sl {ev.Player.DisplayNickname}(이)가 핵을 <b>원격으로 터트렸습니다.</b>"); break;
                        case "스피릿": spirits.Add(ev.Player); break;
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

        public T GetRandomValue<T>(List<T> list)
        {
            int index = UnityEngine.Random.Range(0, list.Count);
            return list[index];
        }

        public async void OnChangedItem(Exiled.Events.EventArgs.Player.ChangedItemEventArgs ev)
        {
            if (GrapCoinSerials.Contains(ev.Item.Serial))
            {
                while (true)
                {
                    if (ev.Player.CurrentItem == null || !GrapCoinSerials.Contains(ev.Player.CurrentItem.Serial))
                        break;

                    ev.Player.ShowHint("이 동전을 튕기면 <b>갈고리</b> 능력을 사용할 수 있습니다.", 1.2f);

                    await Task.Delay(1000);
                }
                
            }
        }
        public async void OnFlippingCoin(Exiled.Events.EventArgs.Player.FlippingCoinEventArgs ev)
        {
            if (GrapCoinSerials.Contains(ev.Item.Serial))
            {
                ev.Item.Destroy();

                Player target1 = Plugin.GetRandomValue(Player.List.Where(x => x.IsAlive && x != ev.Player).ToList());
                target1.Position = ev.Player.Position;

                for (int i=1; i<6; i++)
                {
                    target1.ShowHint($"<b>갈고리</b> 아이템을 사용한 {ev.Player.DisplayNickname}에 의해 끌려왔습니다.", 1.2f);

                    await Task.Delay(1000);
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
                ev.Player.IsUsingStamina = true;
                ev.Player.IsBypassModeEnabled = false;

                if (BlackOutCooldown.Contains(ev.Player.UserId))
                    BlackOutCooldown.Remove(ev.Player.UserId);

                if (spirits.Contains(ev.Player))
                    spirits.Remove(ev.Player);

                if (repairs.Contains(ev.Player))
                    repairs.Remove(ev.Player);

                if (magicians.Contains(ev.Player))
                {
                    magicians.Remove(ev.Player);

                    ev.Attacker.Kill($"몸이 교체되는 마술에 당했네요!");
                    ev.IsAllowed = false;
                    Server.ExecuteCommand($"/fc {ev.Player.Id} {ev.Attacker.Role.Name} 0");
                }

                if (posions.Contains(ev.Player))
                {
                    posions.Remove(ev.Player);

                    ev.Attacker.EnableEffect(Exiled.API.Enums.EffectType.CardiacArrest);
                }
            }
        }

        public void InteractingDoor(Exiled.Events.EventArgs.Player.InteractingDoorEventArgs ev)
        {
            if (ev.Player != null && PlayerAbilities[ev.Player.UserId].Contains("[일반] 행운") && UnityEngine.Random.Range(0, 100) <= 5)
            {
                if (ev.Door.IsOpen)
                    ev.Door.IsOpen = false;

                else
                    ev.Door.IsOpen = true;
            }
        }

        public void InteractingLocker(Exiled.Events.EventArgs.Player.InteractingLockerEventArgs ev)
        {
            if (ev.Player != null && PlayerAbilities[ev.Player.UserId].Contains("[일반] 행운") && UnityEngine.Random.Range(0, 100) <= 5)
            {
                if (ev.Chamber.IsOpen)
                    ev.Chamber.IsOpen = false;

                else
                    ev.Chamber.IsOpen = true;
            }
        }

        public void OnTriggeringTesla(Exiled.Events.EventArgs.Player.TriggeringTeslaEventArgs ev)
        {
            if (repairs.Contains(ev.Player))
                ev.IsAllowed = false;
        }

        public void DroppedItem(Exiled.Events.EventArgs.Player.DroppedItemEventArgs ev)
        {
            if (PlayerAbilities[ev.Player.UserId].Contains("[영웅] 도박꾼"))
            {
                if (UnityEngine.Random.Range(0, 100) <= 5)
                    ev.Player.EnableEffect(Exiled.API.Enums.EffectType.SeveredHands);

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

            if (PlayerAbilities[ev.Attacker.UserId].Contains("[신화] 로켓 런처"))
                Server.ExecuteCommand($"/rocket {ev.Player.Id} 1");
        }

        public void ChangingSpectatedPlayer(Exiled.Events.EventArgs.Player.ChangingSpectatedPlayerEventArgs ev)
        {
            if (ev.NewTarget != null)
            {
                if (PlayerAbilities[ev.NewTarget.UserId].Count <= 0)
                    ev.Player.ShowHint($"<align=left><b><size=22>워크스테이션 위에서 점프하면 능력을 획득할 수 있습니다.</size></b></align>", 250f);
                else
                {
                    string abilitiesText = string.Join(", ", PlayerAbilities[ev.NewTarget.UserId]);
                    abilitiesText = abilitiesText.Replace("[신화]", "<color=#DF0101>[신화]</color>").Replace("[전설]", "<color=#ffd700>[전설]</color>").Replace("[영웅]", "<color=#FF00FF>[영웅]</color>").Replace("[희귀]", "<color=#2ECCFA>[희귀]</color>").Replace("[일반]", "<color=#A4A4A4>[일반]</color>");

                    ev.Player.ShowHint($"<align=left><b><size=25>보유 업그레이드</size></b>\n<size=20>{abilitiesText}</size></align>", 250f);
                }
            }
        }

    }
}
