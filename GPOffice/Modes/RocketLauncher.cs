using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Exiled.API.Features;

namespace GPOffice.Modes
{
    class RocketLauncher
    {
        public static RocketLauncher Instance;

        public void OnEnabled()
        {
            Exiled.Events.Handlers.Player.Hurt += OnHurt;
        }

        public void OnDisabled()
        {
            Exiled.Events.Handlers.Player.Hurt -= OnHurt;
        }

        public void OnHurt(Exiled.Events.EventArgs.Player.HurtEventArgs ev)
        {
            if (!ev.DamageHandler.IsFriendlyFire)
                Server.ExecuteCommand($"/rocket {ev.Player.Id} 0.3");
        }
    }
}
