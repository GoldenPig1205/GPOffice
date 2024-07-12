using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CustomRendering;
using Exiled.API.Features;
using MEC;
using Mirror;
using UnityEngine;
using YamlDotNet.Serialization.NodeDeserializers;

namespace GPOffice.Modes
{
    class MiniGames
    {
        public static MiniGames Instance;

        public int RoundCount = 0;
        public List<string> Games = new List<string>() { "airstrike", "dm", "escape", "battle", "versus2", "versus", "cs", "glass", "line", "dodge", "fall", 
            "football", "gungame", "knives", "chair", "puzzle", "race", "light", "jail", "spleef", "tag", "tdm", "lava", "zombie3", "zombie2", "zombie", "vote", "lobby" };

        public void OnEnabled()
        {
            Round.IsLocked = true;

            Timing.RunCoroutine(OnModeStarted());
        }

        public IEnumerator<float> OnModeStarted()
        {
            foreach (var player in Player.List)
            {
                player.Role.Set(PlayerRoles.RoleTypeId.ClassD);
                player.Position = new Vector3(40.03516f, 1014.11f, -32.02734f);
            }

            yield return Timing.WaitForSeconds(10f);

            while (RoundCount < 3)
            {
                bool end = true;

                Server.ExecuteCommand($"/ev run {GPOffice.GetRandomValue(Games)}");

                while (end)
                {
                    foreach (var player in Player.List)
                    {
                        if (Physics.Raycast(player.Position, Vector3.down, out RaycastHit hit, 10, (LayerMask)1))
                        {
                            ServerConsole.AddLog(hit.transform.name);
                            if (hit.transform.name == "classname=brush.003")
                            {
                                end = false;
                                break;
                            }
                        }
                    }

                    yield return Timing.WaitForSeconds(1f);
                }

                RoundCount += 1;

                if (RoundCount != 3)
                {
                    for (int i = 1; i < 10; i++)
                    {
                        foreach (var player in Player.List)
                        {
                            player.ClearBroadcasts();
                            player.Broadcast(2, $"<b><color=red>{10 - i}</color>초 후 <i><color=yellow>{RoundCount + 1}번째 라운드</color></i>가 시작됩니다.</b>");
                        }
                        yield return Timing.WaitForSeconds(1f);
                    }
                }
            }

            Round.IsLocked = false;
        }
    }
}
