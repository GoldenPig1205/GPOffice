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
using PlayerRoles;
using PlayerRoles.PlayableScps.Scp106;

namespace GPOffice.SubModes
{
    class FriendlyFire
    {
        public static FriendlyFire Instance;

        public void OnEnabled()
        {
            Server.FriendlyFire = true;

            Timing.RunCoroutine(OnModeStarted());

            Harmony harmony = new Harmony("HitboxIdentityPatch");
            harmony.PatchAll();
        }

        public IEnumerator<float> OnModeStarted()
        {
            yield return Timing.WaitForSeconds(1f);
        }

        [HarmonyPatch(typeof(HitboxIdentity), nameof(HitboxIdentity.IsEnemy), typeof(Team), typeof(Team))]
        public class HitboxPatchPostfix
        {
            public static void Postfix(ref bool __result)
            {
                __result = true;
            }
        }
    }
}
