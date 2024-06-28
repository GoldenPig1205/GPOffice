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

namespace GPOffice.Modes
{
    class DoorLock
    {
        public static DoorLock Instance;

        public void OnEnabled()
        {
            Exiled.Events.Handlers.Player.InteractingDoor += OnInteractingDoor;
        }

        public void OnInteractingDoor(Exiled.Events.EventArgs.Player.InteractingDoorEventArgs ev)
        {
            if (UnityEngine.Random.Range(1, 20) == 1)
                ev.Player.Position = Exiled.API.Features.Doors.Door.Random().Position;

            else
            {
                ev.Player.ShowHint("<i><color=orange><size=25>\"잘못된 문을 열어버렸어..\"</size></color></i>", 3);
                ev.Player.Kill("5% 확률로 안타까운 죽음을 맞았습니다.");
            }
        }
    }
}
