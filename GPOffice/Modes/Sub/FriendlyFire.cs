using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Exiled.API.Features;
using UnityEngine;
using PlayerRoles.FirstPersonControl;
using MEC;
using HarmonyLib;
using Mirror;
using PlayerRoles.PlayableScps.Scp049;
using Utils.Networking;

namespace GPOffice.SubModes
{
    class FriendlyFire
    {
        public static FriendlyFire Instance;

        public void OnEnabled()
        {
            Server.FriendlyFire = true;

            Timing.RunCoroutine(OnModeStarted());

            Harmony harmony = new Harmony("GPOffice.FriendlyFire");
            harmony.PatchAll();
        }

        public IEnumerator<float> OnModeStarted()
        {
            yield return Timing.WaitForSeconds(1f);
        }

        [HarmonyPatch(typeof(HitboxIdentity), nameof(HitboxIdentity.IsEnemy))]
        public class HitboxIdentityPatch
        {
            public static bool Prefix(HitboxIdentity __instance, NetworkReader reader)
            {
                return true;
            }
        }
    }
}
