using System.Collections.Generic;
using System.Linq;
using Exiled.API.Features;
using MEC;
using UnityEngine;

namespace GPOffice.Modes
{
    class ABattle
    {
        public static ABattle Instance;

        public Dictionary<string, List<Vector3>> PlayerWorkstation = new Dictionary<string, List<Vector3>>();
        public Dictionary<string, List<string>> PlayerAbilities = new Dictionary<string, List<string>>();

        public Dictionary<string, string> CommonAbilities = new Dictionary<string, string>()
                    {
                        {"[일반] 운동", "25HP만큼 최대 체력을 추가합니다."},
                        {"[일반] 경공", "이동 속도가 25% 증가합니다."},
                        {"[일반] 진화", "몸의 크기가 12% 작아집니다."},
                        {"[일반] 단련", "공격력이 20% 추가됩니다."},
                        {"[일반] 잠수", "스태미나 소모량이 20% 줄어듭니다."},
                        {"[일반] 행운", "1% 확률로 잠긴 문을 엽니다."},
                        {"[일반] 체력 보충", "영구적인 75AHP를 받습니다."},
                        {"[일반] 랜덤박스", "랜덤한 아이템을 지급받습니다."}
                    };
        public Dictionary<string, string> RareAbilities = new Dictionary<string, string>()
                    {
                        {"[희귀] 육체 강화", "200HP를 받습니다."},
                        {"[희귀] 블랙아웃", "시설을 10초 동안 정전시킵니다."}
                    };
        public Dictionary<string, string> EpicAbilities = new Dictionary<string, string>()
                    {
                        {"[영웅] 테러리스트의 유품", "핑크 사탕을 지급받습니다."},
                        {"[영웅] 랜덤상자", "랜덤하지만 좋은 아이템을 지급받습니다."},
                        {"[영웅] 핵 리모컨", "핵 프로세스를 시작합니다."},
                        {"[영웅] 수리 기사", "모든 엘레베이터를 15초 간 고장냅니다."},
                        {"[영웅] 슈퍼 스타", "자신의 마이크가 모두에게 공유됩니다."}
                    };
        public Dictionary<string, string> LegendAbilities = new Dictionary<string, string>()
                    {
                        {"[전설] 해킹", "시설 핵을 즉시 터트립니다."},
                        {"[전설] 스피드왜건", "속도가 최대량이 됩니다."}
                    };

        public void OnEnabled()
        {
            Timing.RunCoroutine(
                OnModeStarted()
                );

            Exiled.Events.Handlers.Player.Jumping += OnJumping;
            Exiled.Events.Handlers.Player.Dying += OnDying;
        }

        public IEnumerator<float> OnModeStarted()
        {
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
                                player.ShowHint($"<align=left><b><size=22>워크스테이션에서 능력을 획득하십시오.</size></b></align>", 1.2f);
                            else
                            {
                                string abilitiesText = string.Join(", ", PlayerAbilities[player.UserId]);
                                abilitiesText = abilitiesText.Replace("[전설]", "<color=gold>[전설]</color>");
                                abilitiesText = abilitiesText.Replace("[영웅]", "<color=pink>[영웅]</color>");
                                abilitiesText = abilitiesText.Replace("[희귀]", "<color=skyblue>[희귀]</color>");
                                abilitiesText = abilitiesText.Replace("[일반]", "<color=gray>[일반]</color>");

                                player.ShowHint($"<align=left><b><size=25>보유 능력</size></b>\n<size=20>{abilitiesText}</size></align>", 1.2f);
                            }
                        }
                    }

                }

                yield return Timing.WaitForSeconds(1f);
            }
        }

        public void OnJumping(Exiled.Events.EventArgs.Player.JumpingEventArgs ev)
        {
            if (Physics.Raycast(ev.Player.ReferenceHub.PlayerCameraReference.position + ev.Player.ReferenceHub.PlayerCameraReference.forward * 0.2f, ev.Player.ReferenceHub.PlayerCameraReference.forward, out RaycastHit hit, 3, InventorySystem.Items.Firearms.Modules.StandardHitregBase.HitregMask))
            {
                Transform WorkStation = hit.transform.parent.parent;

                if (WorkStation.name.Contains("Work Station") && !PlayerWorkstation[ev.Player.UserId].Contains(WorkStation.position))
                {
                    PlayerWorkstation[ev.Player.UserId].Add(WorkStation.position);

                    int grade = UnityEngine.Random.Range(1, 101);
                    string abilityGrade;

                    if (grade <= 50)
                        abilityGrade = "[일반]";
                    else if (grade <= 80)
                        abilityGrade = "[희귀]";
                    else if (grade <= 95)
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
                            return EpicAbilities;
                        else
                            return LegendAbilities;
                    }

                    void ApplyGiveAbility(string abilityName)
                    {
                        PlayerAbilities[ev.Player.UserId].Add(abilityName);
                        ev.Player.Broadcast(8, $"<size=20><b>다음 능력이 추가되었습니다.</b></size>\n<size=30>{abilityName}</size>\n<size=25>{AbilityList()[abilityName]}</size>");
                    }

                    string abilityName = GetRandomValue(AbilityList().Keys.ToList());

                    ApplyGiveAbility(abilityName);
                }
            }
        }

        public void OnDying(Exiled.Events.EventArgs.Player.DyingEventArgs ev)
        {
            if (PlayerWorkstation.ContainsKey(ev.Player.UserId))
                PlayerAbilities[ev.Player.UserId].Clear();
        }

        private T GetRandomValue<T>(List<T> list)
        {
            int index = UnityEngine.Random.Range(0, list.Count);
            return list[index];
        }
    }
}
