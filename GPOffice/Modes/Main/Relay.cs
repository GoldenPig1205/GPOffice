using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Exiled.API.Features;
using MEC;

namespace GPOffice.Modes
{
    class Relay
    {
        public static Relay Instance;

        CoroutineHandle timing_OnModeStarted;

        public static object Mode = Plugin.GetRandomValue(Plugin.Mods.Keys.ToList());
        public static string mod = Mode.ToString();

        public List<string> pl = new List<string>();

        public void OnEnabled()
        {
            timing_OnModeStarted = Timing.RunCoroutine(OnModeStarted());
        }

        public void OnDisabled()
        {
            Timing.KillCoroutines(timing_OnModeStarted);
        }

        public IEnumerator<float> OnModeStarted()
        {
            yield return Timing.WaitForSeconds(10);

            while (true)
            {
                var modeType = Type.GetType($"GPOffice.Modes.{Plugin.Mods[mod].ToString().Split('/')[2].Replace(" ", "")}");
                if (modeType != null)
                {
                    var modeInstance = Activator.CreateInstance(modeType);
                    var onEnabledMethod = modeType.GetMethod("OnEnabled");
                    onEnabledMethod?.Invoke(modeInstance, null);
                }

                Player.List.ToList().ForEach(x => x.Broadcast(10, $"<size=20>다음 모드 주자는..</size>\n<size=25><b>[<color=#{Plugin.Mods[mod].ToString().Split('/')[0]}>{mod}</color>]</b></size>"));

                yield return Timing.WaitForSeconds(120f);

                var currentModeType = Type.GetType($"GPOffice.Modes.{Plugin.Mods[mod].ToString().Split('/')[2].Replace(" ", "")}");
                if (currentModeType != null)
                {
                    var currentModeInstance = Activator.CreateInstance(currentModeType);
                    var onDisabledMethod = currentModeType.GetMethod("OnDisabled");
                    onDisabledMethod?.Invoke(currentModeInstance, null);

                    currentModeInstance = null;
                }

                Mode = Plugin.GetRandomValue(Plugin.Mods.Keys.ToList());
                mod = Mode.ToString();
            }
        }

    }
}
