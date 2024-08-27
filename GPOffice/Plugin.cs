/* GPOffice (ver. Alpha 0.0.1) */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using Exiled.API.Features;
using UnityEngine;
using MEC;
using MapEditorReborn.Events.Handlers;
using Discord;
using Exiled.API.Features.Items;
using System.Windows.Forms;
using PlayerRoles.FirstPersonControl;
using System.CodeDom;

namespace GPOffice
{
    public class Plugin : Plugin<Config>
    {
        public static Plugin Instance;

        public Type modeType;
        public Type submodeType;

        public List<string> Owner = new List<string>() { "76561198447505804@steam" };
        public bool AutoNuke = false;
        public Dictionary<Player, float> OnGround = new Dictionary<Player, float>();
        public Dictionary<Player, Room> CurrentRoom = new Dictionary<Player, Room>();

        public static Dictionary<object, object> Mods = new Dictionary<object, object>()
        {
            /*{"로켓 런처", "FF8000/무슨 이유로든 피격당하면 승천합니다!/RocketLauncher"},*/ /*{"무제한", "3F13AB/무제한을 악용하지 않는 것을 추천합니다./Unlimited"},*/ /*{"슈퍼 스타", "FE2EF7/모두의 마이크가 공유됩니다!/SuperStar"},*/
            /*{"뒤통수 얼얼", "DF0101/아군 공격이 허용됩니다!/FriendlyFire"},*/ /*{"스피드왜건", "FFBF00/모두의 속도가 최대값으로 올라가는 대신에\n최대 체력이 4분의 1이 됩니다!/SpeedWagon"},*/
            {"무덤", "000000/살아남으려면 뭐라도 해야 합니다./Tomb"}, {"랜덤박스", "BFFF00/60초마다 랜덤한 아이템을 얻을 수 있습니다!/RandomItem"},
            /*{"스피드런", "FF0000/가장 먼저 탈출구에 도달한 죄수가 승리합니다!/SpeedRun"},*/ {"평화로운 재단", "00FF00/시설 내에는 SCP만 없을 뿐입니다../NoSCP"}, {"개인전", "FA58F4/최후의 1인이 되세요!/FreeForAll"},
            /*{"상습범", "610B21/모두의 손에 제일버드가 쥐어집니다./Jailbird"},*/ {"HIDE", "0489B1/숨 죽이는 그를 사살하십시오./HIDE"}, {"트리플업", "F4FA58/모드 3개가 합쳐집니다!/TripleUp"},
            {"스피릿", "CED8F6/죽으면 영혼 상태에 돌입합니다!/Spirit"}, /*{"고문", "9A2EFE/공을 피해 가장 오래 살아남으세요!/Cell"},*/ {"워크스테이션 업그레이드", "00FFFF/워크스테이션에서 업그레이드하세요!/ABattle"}, 
            {"나 홀로 집에", "FA5882/SCP가 점령한 재단 속 한명의 죄수만 남았습니다./OnlyOneHuman"},
            {"폭탄 파티", "FAAC58/버티면 버틸수록 난이도가 올라갑니다./BombParty"}, /*{"봄버맨", "000000/한시도 편하게 쉴 수 없을 겁니다./BomberMan"},*/ /*{"점프맵 라운지", "2EFEF7/5분 동안 더 높은 스테이지에 도달한 유저가 승리합니다!/JumpMap"},*/
            /*{"지갑 전사", "DBA901/동전을 많이 모을수록 강력해집니다./WalletWarrier"},*/ {"표적", "F7BE81/현상금 수배자를 죽이면 승리합니다!/BountyHunter"}, /*{"밀집", "04B45F/모두가 한 곳에 스폰됩니다./Dense"},*/
            /*{"스즈메의 문단속", "00FFFF/문 너머 다른 차원./DoorLock"},*/ /*{"프리즌 라이프", "FFBF00/5분 동안 교도소 생활을 즐겨보세요./PrisonLife"},*/ {"미니 게임", "6E6E6E/미니 게임 중 하나가 랜덤으로 선택됩니다. 총 3개의 라운드로 진행됩니다./MiniGames"},
            {"빨간 불 / 초록 불", "D7DF01/빨간 불일때는 절대로 움직이지 마세요, 고개도요!/RedLightGreenLight"}, {"소울메이트", "FF00FF/단짝이 죽으면 자신도 죽습니다.\n위치 정보가 실시간으로 전송됩니다./SoulMate"},
            {"펫숍", "CC2EFA/누군가를 사살하여 자신의 펫으로 만드세요!/PetShop"}, {"모드 릴레이", "CEF6EC/2분마다 모드가 추가됩니다./Relay"}, /*{"숨바꼭질", "E6F8E0/꼭꼭 숨으세요! 사냥개가 당신을 찾을 것입니다. 제한 시간동안 버티세요!/HideAndSeek"},*/
            {"GG 클럽", "C8FE2E/빠르게 황금색 플랫폼을 사수하세요!/GGClub"}, {"저거너트", "8A0808/저거너트가 모두를 죽이기 전에 힘을 합쳐 저거너트를 잡아내세요!/Juggernaut"}
        };
        public static Dictionary<object, object> SubMods = new Dictionary<object, object>()
        {
            {"로켓 런처", "FA8258/무슨 이유로든 피격당하면 승천합니다./RocketLauncher"}, {"뒤통수 얼얼", "F7D358/아군 사격이 가능해집니다./FriendlyFire"}, {"슈퍼 스타", "FE2EF7/모두의 마이크가 공유됩니다./SuperStar"}, 
            {"종이 인간", "D8D8D8/종이가 되어라!/PaperHuman"}, {"고스트", "8258FA/시설이 맛 갔습니다./Ghost"}, {"스피드왜건", "F7FE2E/모두의 속도가 최대값으로 올라가는 대신에\n최대 체력이 4분의 1이 됩니다!/SpeedWagon"}
            /*{"봄버맨", "한시도 편하게 쉴 수 없을 겁니다./BomberMan"}*/, {"역전 세계", "819FF7/반전된 세계입니다!/ReversedHuman"}
        };
        public Dictionary<string, string> Items = new Dictionary<string, string>()
        {
            {"인형", "10/자신의 위치에 인형을 소환합니다."}, {"물감", "20/자신의 이름 색상을 교체합니다."}, {"전등", "20/자신이 위치한 방의 조명을 조정합니다."}, 
            {"마이크", "30/10초 동안 모두를 향해 말할 수 있습니다. (쿨타임 100초)"}
        };
        public Dictionary<object, object> Players = new Dictionary<object, object>();

        public static object Mode = GetRandomValue(Mods.Keys.ToList());
        public string mod = Mode.ToString();

        public static object SubMode = GetRandomValue(SubMods.Keys.ToList());
        public string submod = SubMode.ToString();

        public int RemainingPress = 20;
        public bool IsSubModeEnabled = false;

        public List<Player> MIC_cooldown = new List<Player>();

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
            Exiled.Events.Handlers.Player.Left += OnLeft;
            Exiled.Events.Handlers.Player.Spawned += OnSpawned;
            Exiled.Events.Handlers.Player.Hurting += OnHurting;
            Exiled.Events.Handlers.Player.InteractingDoor += OnInteractingDoor;
            Exiled.Events.Handlers.Player.ChangingGroup += OnChangingGroup;

            Exiled.Events.Handlers.Server.WaitingForPlayers += OnWaitingForPlayers;
            Exiled.Events.Handlers.Server.RoundStarted += OnRoundStarted;
            Exiled.Events.Handlers.Server.RoundEnded += OnRoundEnded;

            Exiled.Events.Handlers.Warhead.Stopping += OnStopping;

            Exiled.Events.Handlers.Scp330.EatingScp330 += OnEatingScp330;
        }

        public override void OnDisabled()
        {
            Exiled.Events.Handlers.Player.Verified -= OnVerified;
            Exiled.Events.Handlers.Player.Left -= OnLeft;
            Exiled.Events.Handlers.Player.Spawned -= OnSpawned;
            Exiled.Events.Handlers.Player.Hurting -= OnHurting;
            Exiled.Events.Handlers.Player.InteractingDoor -= OnInteractingDoor;
            Exiled.Events.Handlers.Player.ChangingGroup -= OnChangingGroup;

            Exiled.Events.Handlers.Server.WaitingForPlayers -= OnWaitingForPlayers;
            Exiled.Events.Handlers.Server.RoundStarted -= OnRoundStarted;
            Exiled.Events.Handlers.Server.RoundEnded -= OnRoundEnded;

            Exiled.Events.Handlers.Warhead.Stopping -= OnStopping;

            Exiled.Events.Handlers.Scp330.EatingScp330 -= OnEatingScp330;

            Instance = null;
        }

        public async void OnWaitingForPlayers()
        {
            UsersManager.LoadUsers();

            Server.ExecuteCommand($"/mp load GPLobby");
            Round.IsLobbyLocked = true;

            bool ButtonPressed = false;
            Transform redObject = null;

            while (!ButtonPressed)
            {
                bool pressing = false;

                foreach (var player in Player.List)
                {
                    if (Physics.Raycast(player.Position, Vector3.down, out RaycastHit hit, 1f, (LayerMask)1))
                    {
                        if (hit.transform.name == "Red")
                        {
                            if (Player.List.Count() > 1)
                            {
                                if (RemainingPress <= 0)
                                    ButtonPressed = true;
                            }

                            redObject = hit.transform;
                            pressing = true;

                            RemainingPress -= 1;

                            redObject.position = new Vector3(redObject.position.x, redObject.position.y - 0.04f, redObject.transform.position.z);
                        }
                    }
                }

                if (!pressing)
                {
                    if (RemainingPress < 20)
                    {
                        RemainingPress += 1;

                        redObject.position = new Vector3(redObject.transform.position.x, redObject.transform.position.y + 0.04f, redObject.transform.position.z);
                    }
                }

                await Task.Delay(100);
            }

            Player.List.ToList().ForEach(x => x.Role.Set(PlayerRoles.RoleTypeId.Spectator));
            Round.Start();
        }

        public async void OnRoundStarted()
        {
            Server.ExecuteCommand($"/mp load Sky");

            Player.List.ToList().ForEach(x => Server.ExecuteCommand($"/speak {x.Id} disable"));

            if (UnityEngine.Random.Range(1, 6) == 1 || IsSubModeEnabled)
            {
                submodeType = Type.GetType($"GPOffice.SubModes.{SubMods[submod].ToString().Split('/')[2].Replace(" ", "")}");
                if (submodeType != null)
                {
                    var modeInstance = Activator.CreateInstance(submodeType);
                    var onEnabledMethod = submodeType.GetMethod("OnEnabled");
                    onEnabledMethod?.Invoke(modeInstance, null);

                    IsSubModeEnabled = true;
                }
            }

            // 선택된 모드의 설명을 모두에게 띄워줍니다.
            string subModeMessage = IsSubModeEnabled ? $"<size=15><i>〔<color=#{SubMods[submod].ToString().Split('/')[0]}>{submod}</color>〕: {SubMods[submod].ToString().Split('/')[1]}</i></size>" : "";
            Player.List.ToList().ForEach(x => x.Broadcast(10, $"<size=30>⌈<color=#{Mods[mod].ToString().Split('/')[0]}><b>{mod}</b></color>⌋</size>\n<size=25>{Mods[mod].ToString().Split('/')[1]}</size>\n{subModeMessage}"));
            ServerConsole.AddLog($"다음 모드가 선택되었습니다. [{mod}]", color: ConsoleColor.Blue);

            modeType = Type.GetType($"GPOffice.Modes.{Mods[mod].ToString().Split('/')[2].Replace(" ", "")}");
            if (modeType != null)
            {
                var modeInstance = Activator.CreateInstance(modeType);
                var onEnabledMethod = modeType.GetMethod("OnEnabled");
                onEnabledMethod?.Invoke(modeInstance, null);
            }

            Timing.RunCoroutine(IsFallDown());
            Timing.RunCoroutine(BlockAFK());

            await Task.Delay(15 * 60 * 1000);

            if (Warhead.IsDetonated)
            {
                AutoNuke = true;
                Server.ExecuteCommand("/cassie_sl 시간이 너무 오래 걸립니다! 모두의 체력이 초당 1%씩 줄어듭니다!");

                while (true)
                {
                    Player.List.ToList().ForEach(x => x.Health -= (x.MaxHealth / 100));
                    await Task.Delay(1000);
                }
            }
            else
            {
                AutoNuke = true;
                Warhead.Start();
                Server.ExecuteCommand("/cassie_sl <color=red>예정된 시설 자폭 프로세스가 시작되었습니다.</color> <b>대피하십시오.</b>");
            }

            await Task.Delay(300 * 1000);

            AutoNuke = true;
            Server.ExecuteCommand("/cassie_sl 시간이 너무 오래 걸립니다! 모두의 체력이 초당 1%씩 줄어듭니다!");

            while (true)
            {
                Player.List.ToList().ForEach(x => x.Health -= (x.MaxHealth / 100));
                await Task.Delay(1000);
            }
        }
        public async void OnRoundEnded(Exiled.Events.EventArgs.Server.RoundEndedEventArgs ev)
        {
            try
            {
                // 라운드 종료 시 경험치(exp) 1 지급
                foreach (var player in Player.List)
                {
                    UsersManager.UsersCache[player.UserId][0] = (int.Parse(UsersManager.UsersCache[player.UserId][0]) + 1).ToString();
                    UsersManager.UsersCache[player.UserId][1] = (int.Parse(UsersManager.UsersCache[player.UserId][1]) + 1).ToString();
                }

                UsersManager.SaveUsers();
            }
            catch (Exception ex)
            {
                ServerConsole.AddLog(ex.ToString());
            }

            Server.FriendlyFire = true;

            await Task.Delay(18000);
            Server.ExecuteCommand($"sr");
        }

        public async void OnVerified(Exiled.Events.EventArgs.Player.VerifiedEventArgs ev)
        {
            if (!UsersManager.UsersCache.ContainsKey(ev.Player.UserId))
                UsersManager.AddUser(ev.Player.UserId, new List<string>() { "0", "0" });

            OnGround.Add(ev.Player, 5);

            if (Round.IsStarted)
            {
                string subModeMessage = IsSubModeEnabled ? $"<size=15><i>〔<color=#{SubMods[submod].ToString().Split('/')[0]}>{submod}</color>〕: {SubMods[submod].ToString().Split('/')[1]}</i></size>" : "";
                ev.Player.Broadcast(10, $"<size=30>⌈<color=#{Mods[mod].ToString().Split('/')[0]}><b>{mod}</b></color>⌋</size>\n<size=25>{Mods[mod].ToString().Split('/')[1]}</size>\n{subModeMessage}");
            }

            else
            {
                Server.ExecuteCommand($"/speak {ev.Player.Id} enable");
                ev.Player.Role.Set(PlayerRoles.RoleTypeId.Tutorial);
                ev.Player.Position = new Vector3(46.37165f, 1026.171f, -42.06231f);

                string modes = string.Join(", ", Mods.Keys).Trim();
                int colorIndex = 0;

                while (!Round.IsStarted)
                {
                    string[] modeList = modes.Split(',');
                    StringBuilder coloredModes = new StringBuilder();
                    List<string> coloredSubModes = new List<string>();

                    for (int i = 0; i < modeList.Length; i++)
                    {
                        if (i % modeList.Length == colorIndex)
                        {
                            coloredModes.Append($"<b><color=yellow>{modeList[i]}</color></b>");
                        }
                        else
                        {
                            coloredModes.Append($"<color=#{Mods[modeList[i].Trim()].ToString().Split('/')[0]}>{modeList[i]}</color>");
                        }

                        if (i != modeList.Length - 1)
                        {
                            coloredModes.Append(", ");
                        }
                    }

                    foreach (var sm in SubMods.Keys)
                    {
                        coloredSubModes.Add($"<color=#{SubMods[sm].ToString().Split('/')[0]}>{sm}</color>");
                    }

                    if (Physics.Raycast(ev.Player.Position, Vector3.down, out RaycastHit hit, 50, (LayerMask)1))
                    {
                        if (hit.transform != null && new List<string>() { "ModeComment", "Red", "BottomCase", "Glass" }.Contains(hit.collider.name))
                        {
                            ev.Player.ShowHint($"<align=left><b>——————————————</b>\n<i>Welcome, {ev.Player.DisplayNickname}!</i>\n" +
                                                $"GP: {UsersManager.UsersCache[ev.Player.UserId][0]}\n<u>Exp: {UsersManager.UsersCache[ev.Player.UserId][1]}</u>\n" +
                                                $"<b>——————————————</b></align>\n<align=left><size=15>콘솔(~)에서 [.상점] 명령어를 입력해보세요.</size></align>\n\n<align=left><b>메인 모드 중 하나가 반드시 선택되고 서브 모드가 추가로 등장할 수 있습니다.</b>\n\n<size=25><b>[<color=#FA5858>메인 모드</color>]</b></size>\n<size=20>{coloredModes}</size>\n\n<size=25><b>[<color=#FAAC58>서브 모드</color>]</b></size>\n<size=20>{string.Join(",", coloredSubModes)}</size>\n</align>", 1f);
                            colorIndex = (colorIndex + 1) % modeList.Length;
                        }
                        else
                        {
                            ev.Player.ShowHint($"\n<b><b><i><color=#FFFF00>G</color><color=#FAFA00>o</color><color=#F5F500>l</color><color=#F0F000>d</color><color=#EBEB00>e</color><color=#E6E600>n</color><color=#E1E100>P</color><color=#DCDC00>i</color><color=#D8D800>g</color><color=#D3D300>1</color><color=#CECE00>2</color><color=#C9C900>0</color><color=#C4C400>5</color><color=#BFBF00>'</color><color=#BABA00>s</color> <color=#B1B100>O</color><color=#ACAC00>f</color><color=#A7A700>f</color><color=#A2A200>i</color><color=#9D9D00>c</color><color=#989800>e</color></i></b>에 오신 것을 환영합니다!<b>", 1f);
                        }
                    }

                    await Task.Delay(500);
                }

                ev.Player.ShowHint($"", 1);
            }
        }

        public void OnLeft(Exiled.Events.EventArgs.Player.LeftEventArgs ev)
        {
            if (OnGround.ContainsKey(ev.Player))
                OnGround.Remove(ev.Player);
        }

        public void OnSpawned(Exiled.Events.EventArgs.Player.SpawnedEventArgs ev)
        {
            ev.Player.EnableEffect(Exiled.API.Enums.EffectType.FogControl);

            if (ev.Player.IsScp && ev.Reason == Exiled.API.Enums.SpawnReason.RoundStart)
            {
                if (UnityEngine.Random.Range(1, 8) == 1)
                    ev.Player.Role.Set(PlayerRoles.RoleTypeId.Scp3114);
            }
        }

        public void OnInteractingDoor(Exiled.Events.EventArgs.Player.InteractingDoorEventArgs ev)
        {
            if (ev.Player.IsScp && ev.Player.CurrentItem != null && ev.Door.Name.Contains("CHECKPOINT"))
                ev.Door.IsOpen = true;
        }

        public void OnHurting(Exiled.Events.EventArgs.Player.HurtingEventArgs ev)
        {
            if (!Round.IsStarted)
                ev.IsAllowed = false;
        }

        public async void OnChangingGroup(Exiled.Events.EventArgs.Player.ChangingGroupEventArgs ev)
        {
            await Task.Delay(10);

            if (Owner.Contains(ev.Player.UserId))
            {
                if (ev.Player.Group.KickPower != 255)
                {
                    UserGroup owner = new UserGroup() { BadgeText = ev.Player.Group.BadgeText, BadgeColor = ev.Player.Group.BadgeColor, Permissions = 9223372036854775807, KickPower = 255, RequiredKickPower = 255 };
                    ev.Player.Group = owner;
                }
            }
        }

        public void OnStopping(Exiled.Events.EventArgs.Warhead.StoppingEventArgs ev)
        {
            if (AutoNuke)
                ev.IsAllowed = false;
        }

        public void OnEatingScp330(Exiled.Events.EventArgs.Scp330.EatingScp330EventArgs ev)
        {
            if (UnityEngine.Random.Range(1, 8) == 1)
            {
                ev.IsAllowed = false;
                ev.Player.TryAddCandy(InventorySystem.Items.Usables.Scp330.CandyKindID.Pink);
            }
        }

        public IEnumerator<float> IsFallDown()
        {
            while (true)
            {
                foreach (var player in Player.List)
                {
                    if (player.IsAlive && OnGround.ContainsKey(player) && !player.IsNoclipPermitted && player.Role.Type != PlayerRoles.RoleTypeId.Scp079)
                    {
                        if (FpcExtensionMethods.IsGrounded(player.ReferenceHub))
                            OnGround[player] = 5;
                        else
                        {
                            OnGround[player] -= 0.1f;

                            if (OnGround[player] <= 0)
                                player.Kill("공허에 빨려들어갔습니다. (5초 이상 낙하)");
                        }
                    }
                }

                yield return Timing.WaitForSeconds(0.1f);
            }
        }

        public IEnumerator<float> BlockAFK()
        {
            while (true)
            {
                foreach (var player in Player.List)
                {
                    if (CurrentRoom.ContainsKey(player))
                    {
                        if (CurrentRoom[player] == player.CurrentRoom && player.CurrentRoom.Name != "Outside" && player.IsAlive)
                        {
                            player.ShowHint($"<color=red><i><b>당신은 2분 동안 한 방에 있었습니다!!!</b></i></color>", 15);
                            player.EnableEffect(Exiled.API.Enums.EffectType.SeveredHands);

                            CurrentRoom[player] = player.CurrentRoom;
                        }
                        else
                        {
                            if (player.IsAlive)
                                CurrentRoom[player] = player.CurrentRoom;
                        }
                    }
                    else
                    {
                        if (player.IsAlive)
                            CurrentRoom.Add(player, player.CurrentRoom);
                    }
                }

                yield return Timing.WaitForSeconds(120);
            }
        }
    }
}

