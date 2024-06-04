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
            Exiled.Events.Handlers.Player.VoiceChatting += OnVoiceChatting;
        }

        public void OnDisabled()
        {
            Exiled.Events.Handlers.Player.VoiceChatting -= OnVoiceChatting;
        }

        public void OnVoiceChatting(Exiled.Events.EventArgs.Player.VoiceChattingEventArgs ev)
        {
            Server.ExecuteCommand($"/speak {ev.Player.Id} enable");
        }
    }
}
