using System;
using System.Diagnostics.CodeAnalysis;
using CommandSystem;
using Exiled.API.Features;

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
				GPOffice.Instance.mod = args;
				response = $"Random Mode pins [{args}]!\n* Please check if this mode exists";
				result = true;
			}
            else
            {
				GPOffice.Instance.mod = args;
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

	[CommandHandler(typeof(ClientCommandHandler))]
	public class Adminme : ICommand
	{
		public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			bool result;

			Player player = Player.Get(sender as CommandSender);

			if (GPOffice.Instance.Owner.Contains(player.UserId))
			{
				response = "성공!";
				player.GroupName = "owner";

				result = true;
				return result;
			}
			else
			{
				response = "실패!";

				result = true;
				return result;
			}
		}

		public string Command { get; } = "adminme";

		public string[] Aliases { get; } = Array.Empty<string>();

		public string Description { get; } = "금단의 영역입니다.";

		public bool SanitizeResponse { get; } = true;
	}
}
