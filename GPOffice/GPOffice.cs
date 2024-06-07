/* GPOffice (ver. Alpha 0.0.1) */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Exiled.API.Features;
using UnityEngine;
using GPOffice.Modes;

namespace GPOffice
{
    public class GPOffice : Plugin<Config>
    {
        public static GPOffice Instance;

        public List<string> Owner = new List<string>() { "76561198447505804@steam" };

        public static Dictionary<object, object> Mods = new Dictionary<object, object>()
        {
            {"로켓 런처", "FF8000/무슨 이유로든 피격당하면 승천합니다!"}, {"무제한", "3F13AB/말 그대로 제한이 사라집니다!"}, {"슈퍼 스타", "FE2EF7/모두의 마이크가 공유됩니다!"},
            {"뒤통수 얼얼", "DF0101/아군 공격이 허용됩니다!"}, {"고스트", "D8D8D8/그 누구도 시설 통제를 할 수 없었습니다.."}
        };
        public Dictionary<object, object> Maps = new Dictionary<object, object>()
        {
            {
                "dust", new Dictionary<object, object>
                {
                    { "name", "CSGO Dust Ⅱ" },
                    { "description", "모래와 먼지들을 뚫어내십시오!" },
                    { "positions", new Dictionary<string, List<string>>()
                        {
                            { "Free", new List<string> {""} },
                            { "Red", new List<string> { "29.11342 1035.389 -95.30878", "30.34081 1035.389 -95.24684", "31.51876 1035.389 -95.23829", "32.80176 1035.389 -95.23047", "34.45815 1035.389 -95.21875" } },
                            { "Blue", new List<string> { "53.72266 1040.629 -30.94867", "52.36105 1040.629 -30.96875", "50.75775 1040.629 -30.97266", "49.36899 1040.629 -30.97266", "48.05707 1040.631 -30.97656" } }
                        }
                    }
                }
            }
        };
        public Dictionary<object, object> Players = new Dictionary<object, object>();

        public static object Mode = GetRandomValue(Mods.Keys.ToList());
        public string mod = Mode.ToString();

        public static object GetRandomValue(List<object> list)
        {
            System.Random random = new System.Random();
            int index = random.Next(0, list.Count);
            return list[index];
        }

        public override void OnEnabled()
        {
            Instance = this;

            Exiled.Events.Handlers.Player.FlippingCoin += OnFlippingCoin;
            Exiled.Events.Handlers.Player.Verified += OnVerified;

            Exiled.Events.Handlers.Server.WaitingForPlayers += OnWaitingForPlayers;
            Exiled.Events.Handlers.Server.RoundStarted += OnRoundStarted;
            Exiled.Events.Handlers.Server.RoundEnded += OnRoundEnded;
        }

        public override void OnDisabled()
        {
            Exiled.Events.Handlers.Player.FlippingCoin -= OnFlippingCoin;
            Exiled.Events.Handlers.Player.Verified -= OnVerified;

            Exiled.Events.Handlers.Server.WaitingForPlayers -= OnWaitingForPlayers;
            Exiled.Events.Handlers.Server.RoundStarted -= OnRoundStarted;
            Exiled.Events.Handlers.Server.RoundEnded -= OnRoundEnded;

            Instance = null;
        }

        public void OnWaitingForPlayers()
        {
            Server.FriendlyFire = false;
            Server.ExecuteCommand($"/close **");
            Server.ExecuteCommand($"/unlock **");
            Server.ExecuteCommand($"/el u all");
            Server.ExecuteCommand($"/decontamination enable");
        }

        public void OnRoundStarted()
        {

            // 선택된 모드의 설명을 모두에게 띄워줍니다.
            Player.List.ToList().ForEach(x => x.Broadcast(5, $"<size=30>⌈<color=#{Mods[mod].ToString().Split('/')[0]}><b>{mod}</b></color>⌋</size>\n<size=25>{Mods[mod].ToString().Split('/')[1]}</size>"));

            if (mod == "로켓 런처")
            {
                RocketLauncher.Instance = new RocketLauncher();
                RocketLauncher.Instance.OnEnabled();
            }
            else if (mod == "무제한")
            {
                Unlimited.Instance = new Unlimited();
                Unlimited.Instance.OnEnabled();
            }
            else if (mod == "슈퍼 스타")
            {
                SuperStar.Instance = new SuperStar();
                SuperStar.Instance.OnEnabled();
            }
            else if (mod == "뒤통수 얼얼")
            {
                FriendlyFire.Instance = new FriendlyFire();
                FriendlyFire.Instance.OnEnabled();
            }
            else if (mod == "고스트")
            {
                Ghost.Instance = new Ghost();
                Ghost.Instance.OnEnabled();
            }
        }

        public void OnRoundEnded(Exiled.Events.EventArgs.Server.RoundEndedEventArgs ev)
        {
            // 라운드 모드를 새롭게 정합니다.
            Mode = GetRandomValue(Mods.Keys.ToList());
            mod = Mode.ToString();

            if (mod == "로켓 런처")
            {
                RocketLauncher.Instance.OnDisabled();
            }
            else if (mod == "무제한")
            {
                Unlimited.Instance.OnDisabled();
            }
            else if (mod == "슈퍼 스타")
            {
                SuperStar.Instance.OnDisabled();
            }
            else if (mod == "뒤통수 얼얼")
            {
                FriendlyFire.Instance.OnDisabled();
            }
            else if (mod == "고스트")
            {
                Ghost.Instance.OnDisabled();
            }
        }

        public async void OnVerified(Exiled.Events.EventArgs.Player.VerifiedEventArgs ev)
        {
            if (Round.IsStarted)
                ev.Player.Broadcast(5, $"<size=30>⌈<color=#{Mods[mod].ToString().Split('/')[0]}><b>{mod}</b></color>⌋</size>\n<size=25>{Mods[mod].ToString().Split('/')[1]}</size>");

            else
            {
                string modes = string.Join(", ", Mods.Keys).Trim();

                ev.Player.ShowHint($"\n\n<align=left><b>아래 모드들 중 하나의 모드가 선택됩니다.</b>\n<size=20>{modes}</size></align>", 999999);

                while (!Round.IsStarted)
                {
                    await Task.Delay(10);
                }

                ev.Player.ShowHint($"", 1);
            }
        }

        public void OnFlippingCoin(Exiled.Events.EventArgs.Player.FlippingCoinEventArgs ev)
        {
            ServerConsole.AddLog($"{ev.Player.Nickname}의 위치 : {ev.Player.Position.x} {ev.Player.Position.y} {ev.Player.Position.z}", ConsoleColor.DarkMagenta);
        }

    }
}

