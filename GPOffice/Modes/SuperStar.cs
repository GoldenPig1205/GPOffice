using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Exiled.API.Features;

namespace GPOffice.Modes
{
    class SuperStar
    {
        public static SuperStar Instance;

        public void OnEnabled()
        {
            Exiled.Events.Handlers.Player.Jumping += OnJumping;
        }

        public void OnDisabled()
        {
            Exiled.Events.Handlers.Player.Jumping -= OnJumping;
        }
        
        public void OnJumping(Exiled.Events.EventArgs.Player.JumpingEventArgs ev)
        {
            Server.ExecuteCommand($"/speak {ev.Player.Id} enable");
        }
    }
}
