﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Exiled.API.Features;
using MEC;

namespace GPOffice.Modes
{
    class TripleUp
    {
        public static TripleUp Instance;

        public static Dictionary<object, object> Mods = Plugin.Mods.Concat(Plugin.SubMods).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

        public static object Mode1 = Plugin.GetRandomValue(Mods.Keys.ToList());
        public static string mod1 = Mode1.ToString();

        public static object Mode2 = Plugin.GetRandomValue(Mods.Keys.ToList());
        public static string mod2 = Mode2.ToString();

        public static object Mode3 = Plugin.GetRandomValue(Mods.Keys.ToList());
        public static string mod3 = Mode3.ToString();

        public List<string> Modes = new List<string>() { mod1, mod2, mod3 };

        public List<string> pl = new List<string>();

        public void OnEnabled()
        {
            Timing.RunCoroutine(OnModeStarted());
        }

        public IEnumerator<float> OnModeStarted()
        {
            for (int i = 0; i < 3; i++)
            {
                while (new List<string>() { "더블업", "트리플업" }.Contains(Modes[i]))
                    Modes[i] = Plugin.GetRandomValue(Mods.Keys.ToList()).ToString();

                var modeType = Type.GetType($"GPOffice.Modes.{Mods[Modes[i]].ToString().Split('/')[2].Replace(" ", "")}");
                if (modeType != null)
                {
                    var modeInstance = Activator.CreateInstance(modeType);
                    var onEnabledMethod = modeType.GetMethod("OnEnabled");
                    onEnabledMethod?.Invoke(modeInstance, null);
                }
            }

            Player.List.ToList().ForEach(x => x.Broadcast(10, $"<size=25><b>[<color=#{Mods[mod1].ToString().Split('/')[0]}>{mod1}</color> + <color=#{Mods[mod2].ToString().Split('/')[0]}>{mod2}</color> + <color=#{Plugin.Mods[mod3].ToString().Split('/')[0]}>{mod3}</color>]</b></size>"));

            yield return 1f;
        }
    }
}
