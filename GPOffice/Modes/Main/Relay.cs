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

        public static Dictionary<object, object> Mods = Plugin.Mods.Concat(Plugin.SubMods).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
        public static object Mode = Plugin.GetRandomValue(Mods.Keys.ToList());
        public static string mod = Mode.ToString();

        public List<string> pl = new List<string>();

        public void OnEnabled()
        {
            Timing.RunCoroutine(OnModeStarted());
        }

        public IEnumerator<float> OnModeStarted()
        {
            yield return Timing.WaitForSeconds(10);

            while (true)
            {
                var modeType = Type.GetType($"GPOffice.Modes.{Mods[mod].ToString().Split('/')[2].Replace(" ", "")}");
                if (modeType != null)
                {
                    var modeInstance = Activator.CreateInstance(modeType);
                    var onEnabledMethod = modeType.GetMethod("OnEnabled");
                    onEnabledMethod?.Invoke(modeInstance, null);
                }

                Player.List.ToList().ForEach(x => x.Broadcast(10, $"<size=20>다음 모드 주자는..</size>\n<size=25><b>[<color=#{Mods[mod].ToString().Split('/')[0]}>{mod}</color>]</b></size>"));

                yield return Timing.WaitForSeconds(120f);

                Mode = Plugin.GetRandomValue(Mods.Keys.ToList());
                mod = Mode.ToString();
            }
        }

    }
}
