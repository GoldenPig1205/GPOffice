/* GPOffice (ver. Alpha 0.0.1) */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Exiled.API.Features;
using UnityEngine;
using GPOffice.Modes;
using MEC;
using Exiled.Events.EventArgs.Interfaces;

namespace GPOffice
{
    public class GPOffice : Plugin<Config>
    {
        public static GPOffice Instance;

        public List<string> Owner = new List<string>() { "76561198447505804@steam" };

        public static Dictionary<object, object> Mods = new Dictionary<object, object>()
        {
            {"로켓 런처", "FF8000/무슨 이유로든 피격당하면 승천합니다!"}, {"무제한", "3F13AB/말 그대로 제한이 사라집니다!"}, {"슈퍼 스타", "FE2EF7/모두의 마이크가 공유됩니다!"},
            {"뒤통수 얼얼", "DF0101/아군 공격이 허용됩니다!"}, {"스피드왜건", "FFBF00/모두의 속도가 최대값으로 올라가는 대신에\n최대 체력이 반으로 줄어듭니다!"},
            {"무덤", "000000/살아남으려면 뭐든지 해야 합니다."}, {"랜덤박스", "BFFF00/60초마다 랜덤한 아이템을 얻을 수 있습니다!"}, {"종이 인간", "FFFFFF/종이가 되어라!"},
            {"스피드런", "FF0000/가장 먼저 탈출구에 도달한 죄수가 승리합니다!"}, {"평화로운 재단", "00FF00/시설 내에는 SCP만 없을 뿐입니다.."}, {"개인전", "FA58F4/최후의 1인이 되세요!"},
            {"스즈메의 문단속", "2ECCFA/문 너머 다른 세상을 마주하세요."}, {"HIDE", "D8D8D8/숨 죽이는 그를 잡으십시오."}, {"밀집", "FF00FF/모두가 같은 장소에서 시작합니다!"}
        };
        public Dictionary<string, List<Vector3>> Maps = new Dictionary<string, List<Vector3>>()
        {
            {"dust", new List<Vector3>() { new Vector3(53.26172f, 1040.629f, -31.19531f), new Vector3(23.61328f, 1037.999f, -43.21484f), new Vector3(22.21875f, 1037.996f, -58.96875f),
                                           new Vector3(15.63281f, 1037.996f, -58.85547f), new Vector3(2.316406f, 1039.301f, -60.15234f), new Vector3(1.617188f, 1039.141f, -86.67188f),
                                           new Vector3(13.26172f, 1040.617f, -108.5156f), new Vector3(29.76563f, 1039.965f, -102.4492f), new Vector3(28f, 1037.996f, -83.48438f), new Vector3(51.28906f, 1037.996f, -56.47656f),
                                           new Vector3(31.28125f, 1035.387f, -97.19922f), new Vector3(47.57813f, 1037.999f, -42.97266f), new Vector3(57.39844f, 1035.696f, -75.83594f), new Vector3(72.95975f, 1038.644f, -76.43791f),
                                           new Vector3(68.43631f, 1040.629f, -43.37151f), new Vector3(67.52615f, 1038.644f, -61.91447f), new Vector3(71.90897f, 1037.996f, -82.12541f), new Vector3(75.31912f, 1038.668f, -104.3949f),
                                           new Vector3(58.19333f, 1037.013f, -91.25609f)}}
        };
        public Dictionary<object, object> Players = new Dictionary<object, object>();

        public static object Mode = GetRandomValue(Mods.Keys.ToList());
        public string mod = Mode.ToString();

        public static T GetRandomValue<T>(List<T> list)
        {
            System.Random random = new System.Random();
            int index = random.Next(0, list.Count);
            return list[index];
        }

        public override void OnEnabled()
        {
            Instance = this;

            Exiled.Events.Handlers.Player.Verified += OnVerified;

            Exiled.Events.Handlers.Server.RoundStarted += OnRoundStarted;
            Exiled.Events.Handlers.Server.RoundEnded += OnRoundEnded;
        }

        public override void OnDisabled()
        {
            Exiled.Events.Handlers.Player.Verified -= OnVerified;

            Exiled.Events.Handlers.Server.RoundStarted -= OnRoundStarted;
            Exiled.Events.Handlers.Server.RoundEnded -= OnRoundEnded;

            Instance = null;
        }

        public void OnRoundStarted()
        {
            // 선택된 모드의 설명을 모두에게 띄워줍니다.
            Player.List.ToList().ForEach(x => x.Broadcast(5, $"<size=30>⌈<color=#{Mods[mod].ToString().Split('/')[0]}><b>{mod}</b></color>⌋</size>\n<size=25>{Mods[mod].ToString().Split('/')[1]}</size>"));
            ServerConsole.AddLog($"다음 모드가 선택되었습니다. [{mod}]", color:ConsoleColor.Blue);

            switch (mod)
            {
                case "로켓 런처":
                    RocketLauncher.Instance = new RocketLauncher(); RocketLauncher.Instance.OnEnabled(); break;
                case "무제한":
                    Unlimited.Instance = new Unlimited(); Unlimited.Instance.OnEnabled(); break;
                case "슈퍼 스타":
                    SuperStar.Instance = new SuperStar(); SuperStar.Instance.OnEnabled(); break;
                case "뒤통수 얼얼":
                    FriendlyFire.Instance = new FriendlyFire(); FriendlyFire.Instance.OnEnabled(); break;
                case "스피드왜건":
                    SpeedWagon.Instance = new SpeedWagon(); SpeedWagon.Instance.OnEnabled(); break;
                case "무덤":
                    Tomb.Instance = new Tomb(); Tomb.Instance.OnEnabled(); break;
                case "랜덤박스":
                    RandomItem.Instance = new RandomItem(); RandomItem.Instance.OnEnabled(); break;
                case "종이 인간":
                    PaperHuman.Instance = new PaperHuman(); PaperHuman.Instance.OnEnabled(); break;
                case "스피드런":
                    SpeedRun.Instance = new SpeedRun(); SpeedRun.Instance.OnEnabled(); break;
                case "평화로운 재단":
                    NoSCP.Instance = new NoSCP(); NoSCP.Instance.OnEnabled(); break;
                case "개인전":
                    FreeForAll.Instance = new FreeForAll(); FreeForAll.Instance.OnEnabled(); break;
                case "스즈메의 문단속":
                    DoorLock.Instance = new DoorLock(); DoorLock.Instance.OnEnabled(); break;
                case "HIDE":
                    HIDE.Instance = new HIDE(); HIDE.Instance.OnEnabled(); break;
                case "밀집":
                    Dense.Instance = new Dense(); Dense.Instance.OnEnabled(); break;
                default:
                    break;
            }

            // OnSpawned or OnChangingRole 이벤트 핸들
            foreach (var player in Player.List)
                player.Role.Set(player.Role);
        }

        public async void OnRoundEnded(Exiled.Events.EventArgs.Server.RoundEndedEventArgs ev)
        {
            await Task.Delay(8000);
            Server.ExecuteCommand($"sr");
        }

        public async void OnVerified(Exiled.Events.EventArgs.Player.VerifiedEventArgs ev)
        {
            if (Round.IsStarted)
                ev.Player.Broadcast(5, $"<size=30>⌈<color=#{Mods[mod].ToString().Split('/')[0]}><b>{mod}</b></color>⌋</size>\n<size=25>{Mods[mod].ToString().Split('/')[1]}</size>");

            else
            {
                string modes = string.Join(", ", Mods.Keys).Trim();
                int colorIndex = 0;

                while (!Round.IsStarted)
                {
                    string[] modeList = modes.Split(',');
                    StringBuilder coloredModes = new StringBuilder();

                    for (int i = 0; i < modeList.Length; i++)
                    {
                        if (i % modeList.Length == colorIndex)
                        {
                            coloredModes.Append($"<color=yellow>{modeList[i]}</color>");
                        }
                        else
                        {
                            coloredModes.Append(modeList[i]);
                        }

                        if (i != modeList.Length - 1)
                        {
                            coloredModes.Append(", ");
                        }
                    }

                    ev.Player.ShowHint($"\n\n<align=left><b>아래 모드들 중 하나의 모드가 선택됩니다.</b>\n<size=20>{coloredModes}</size></align>", 3);
                    await Task.Delay(500);
                    colorIndex = (colorIndex + 1) % modeList.Length;
                }

                ev.Player.ShowHint($"", 1);
            }
        }
    }
}

