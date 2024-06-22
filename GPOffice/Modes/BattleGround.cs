using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Exiled.API.Features;
using UnityEngine;

namespace GPOffice.Modes
{
    class BattleGround
    {
        public static BattleGround Instance;

        Task TaskA = new Task(() => BattleGround.Instance.OnModeStarted());

        public void OnEnabled()
        {
            Server.FriendlyFire = true;
            Round.IsLocked = true;
            TaskA.Start();
        }

        public void OnDisabled()
        {
            Server.FriendlyFire = false;
            Round.IsLocked = false;
            TaskA.Dispose();
        }

        public void OnModeStarted()
        {
            Server.ExecuteCommand($"/mp load plane");

            Player p1 = Player.List.ToList()[0];
            p1.Role.Set(PlayerRoles.RoleTypeId.Tutorial);

            for (int i=1; i<Player.Dictionary.Count * 20; i++)
            {
                p1.Position = new Vector3(UnityEngine.Random.Range(-27.57813f, 45.28906f), 1042.989f, UnityEngine.Random.Range(-75.58594f, -2.535156f));
                Server.ExecuteCommand($"/drop {p1.Id} {UnityEngine.Random.Range(0, 54)} 1");
            }

            foreach (var player in Player.List)
            {
                player.Role.Set(PlayerRoles.RoleTypeId.ClassD);
                player.Position = new Vector3(UnityEngine.Random.Range(-27.57813f, 45.28906f), 1042.989f, UnityEngine.Random.Range(-75.58594f, -2.535156f));
            }
        }
    }
}
