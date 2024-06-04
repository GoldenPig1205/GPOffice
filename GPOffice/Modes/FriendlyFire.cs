using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Exiled.API.Features;

namespace GPOffice.Modes
{
    class FriendlyFire
    {
        public static FriendlyFire Instance;

        public void OnEnabled()
        {
            Server.FriendlyFire = true;
        }

        public void OnDisabled()
        {
            Server.FriendlyFire = false;
        }
    }
}
