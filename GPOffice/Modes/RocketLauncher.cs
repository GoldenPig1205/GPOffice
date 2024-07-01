using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Exiled.API.Features;
using MEC;

namespace GPOffice.Modes
{
    class RocketLauncher
    {
        public static RocketLauncher Instance;

        public void OnEnabled()
        {
            Timing.RunCoroutine(OnModeStarted());

            Exiled.Events.Handlers.Player.Hurt += OnHurt;
        }

        public void OnHurt(Exiled.Events.EventArgs.Player.HurtEventArgs ev)
        {
            if (!ev.DamageHandler.IsFriendlyFire)
                Server.ExecuteCommand($"/rocket {ev.Player.Id} 0.3");
        }

        public IEnumerator<float> OnModeStarted()
        {
            yield return 0f;
        }
    }
}
