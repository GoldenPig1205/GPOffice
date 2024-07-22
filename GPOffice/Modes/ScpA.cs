using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using CustomRendering;
using Exiled.API.Features;
using MEC;
using Mirror;
using PlayerRoles.FirstPersonControl;
using UnityEngine;

namespace GPOffice.Modes
{
    class ScpA
    {
        public void OnEnabled()
        {
            Server.RestartRedirect(7780);
        }
    }
}
