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
            Exiled.Events.Handlers.Player.InteractingElevator += OnInteractingElevator;
        }

        public void OnInteractingDoor(Exiled.Events.EventArgs.Player.InteractingDoorEventArgs ev)
        {
            if (UnityEngine.Random.Range(1, 20) != 1)
            {
                Vector3 pos = Exiled.API.Features.Doors.Door.Random().Position;
                pos.y += 1;
                ev.Player.Position = pos;
            }

            else
            {
                ev.Player.ShowHint("<i><color=orange><size=25>\"잘못된 문을 열어버렸어..\"</size></color></i>", 3);
                ev.Player.Kill("5% 확률로 안타까운 죽음을 맞았습니다.");
            }
        }

        public void OnInteractingElevator(Exiled.Events.EventArgs.Player.InteractingElevatorEventArgs ev)
        {
            if (UnityEngine.Random.Range(1, 20) != 1)
            {
                Vector3 pos = Exiled.API.Features.Doors.ElevatorDoor.Random().Position;
                pos.y += 1;
                ev.Player.Position = pos;
            }

            else
            {
                ev.Player.ShowHint("<i><color=orange><size=25>\"잘못된 엘레베이터를 열어버렸어..\"</size></color></i>", 3);
                ev.Player.Kill("5% 확률로 안타까운 죽음을 맞았습니다.");
            }
        }
    }
}
