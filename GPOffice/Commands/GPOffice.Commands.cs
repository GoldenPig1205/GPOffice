using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Security.AccessControl;
using System.Threading.Tasks;
using CommandSystem;
using Discord;
using Exiled.API.Features;
using MEC;
using UnityEngine;

namespace GPOffice.Commands
{
	[CommandHandler(typeof(RemoteAdminCommandHandler))]
	public class ForceMode : ICommand
	{
		public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			bool result;

			string args = string.Join(" ", arguments).Trim();

			if (args != null)
            {
				Plugin.Instance.mod = args;
				response = $"Random Mode pins [{args}]!\n* Please check if this mode exists";
				result = true;
			}
            else
            {
				response = $"There is no name for Random Mode!\n";
				result = false;
			}
			return result;
		}

		public string Command { get; } = "forcemode";

		public string[] Aliases { get; } = { "fm" };

		public string Description { get; } = "이번 라운드의 모드를 강제합니다.";

		public bool SanitizeResponse { get; } = true;
	}

    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class ForceSubMode : ICommand
    {
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            bool result;

            string args = string.Join(" ", arguments).Trim();

            if (args != null)
            {
                Plugin.Instance.submod = args;
                response = $"Random Sub Mode pins [{args}]!\n* Please check if this sub mode exists";
                result = true;
                Plugin.Instance.IsSubModeEnabled = true;
            }
            else
            {
                response = $"There is no name for Random Sub Mode!\n";
                result = false;
            }
            return result;
        }

        public string Command { get; } = "forcesubmode";

        public string[] Aliases { get; } = { "fsm" };

        public string Description { get; } = "이번 라운드의 서브 모드를 강제합니다.";

        public bool SanitizeResponse { get; } = true;
    }

    [CommandHandler(typeof(ClientCommandHandler))]
    public class Store : ICommand
    {
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            bool result;
			Dictionary<string, string> Items = Plugin.Instance.Items;
            string Mark = "\n[ 밀무역자 출신 D계급이 운영하는 상점 ]\n";
            Player player = Player.Get(sender as CommandSender);

            try
			{
                if (arguments.At(0) == "조회")
                {
                    string itemsList = string.Join("\n", Items.Select(item => $"{item.Key}: {item.Value}"));
                    response = $"{Mark}(아이템 조회)\n" + itemsList;

                    result = true;
                    return result;
                }
                else if (arguments.At(0) == "구매")
                {
                    if (!player.IsAlive)
                    {
                        response = $"{Mark}(아이템 구매)\n죽은 상태에서는 상점에 방문할 수 없습니다.";

                        result = false;
                        return result;
                    }

                    if (Items.ContainsKey(arguments.At(1)))
                    {
                        string ItemName = arguments.At(1);

                        if (int.Parse(Items[arguments.At(1)].Split('/')[0]) <= int.Parse(UsersManager.CheckUser(player.UserId, 0)))
                        {
                            UsersManager.UsersCache[player.UserId][0] = (int.Parse(UsersManager.UsersCache[player.UserId][0]) - int.Parse(Items[arguments.At(1)].Split('/')[0])).ToString();

                            response = $"{Mark}(아이템 구매)\n구매에 성공하였습니다!";

                            if (ItemName == "인형")
                                Server.ExecuteCommand($"/ragdoll {player.Id} {UnityEngine.Random.Range(0, 20)} 1");

                            else if (ItemName == "물감")
                            {
                                string rc = Plugin.GetRandomValue(new List<string>() { "black", "white", "silver", "yellow", "red", "pink", "pumpkin", "green", "light_green", "lime", "police_blue", "carmine", "nickel", "mint", "army_green", "tomato", "deep_pink" });
                                player.Group = new UserGroup { BadgeColor = rc, BadgeText = rc };
                                player.ShowHint($"<size=25><i>당신의 이름 색상은 {rc}(으)로 배정되었습니다.</i></size>", 5);
                            }

                            else if (ItemName == "전등")
                                player.CurrentRoom.Color = new Color(UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f));

                            else if (ItemName == "마이크")
                            {
                                if (Plugin.Instance.MIC_cooldown.Contains(player))
                                    response = $"{Mark}(아이템 구매)\n[마이크] 아이템의 구매 쿨타임이 남았습니다.";

                                else
                                {
                                    Plugin.Instance.MIC_cooldown.Add(player);

                                    Server.ExecuteCommand($"/speak {player.Id} enable");

                                    player.ShowHint($"<b><color=red>[ON AIR]</color></b>", 10);
                                    Timing.CallDelayed(10f, () => Server.ExecuteCommand($"/speak {player.Id} disable"));

                                    Timing.CallDelayed(100f, () => { Plugin.Instance.MIC_cooldown.Remove(player); });
                                }
                            }

                            result = true;
                            return result;
                        }
                        else
                        {
                            response = $"{Mark}(아이템 구매)\n구매에 필요한 GP가 부족합니다!";

                            result = false;
                            return result;
                        }
                    }
                    else
                    {
                        response = $"{Mark}(아이템 구매)\n존재하지 않는 아이템 이름입니다.";

                        result = false;
                        return result;
                    }
                }
                else
                {
                    response = $"{Mark}(???)\n알 수 없는 서브 명령어입니다.";

                    result = false;
                    return result;
                }
            }
            catch (Exception e)
            {
                response = $"{Mark}(도움말)\n.상점 조회 - 아이템 목록을 불러옵니다.\n.상점 구매 {{아이템 이름}} - 아이템을 구매합니다.";

                result = false;
                return result;
            }
        }

        public string Command { get; } = "상점";

        public string[] Aliases { get; } = { "store" };

        public string Description { get; } = "상점에서 아이템을 구매할 수 있습니다.";

        public bool SanitizeResponse { get; } = true;
    }
}
