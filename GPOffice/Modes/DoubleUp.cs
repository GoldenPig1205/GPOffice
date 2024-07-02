using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Exiled.API.Features;

namespace GPOffice.Modes
{
    class DoubleUp
    {
        public static DoubleUp Instance;

        public static object Mode1 = GPOffice.GetRandomValue(GPOffice.Mods.Keys.ToList());
        public static string mod1 = Mode1.ToString();

        public static object Mode2 = GPOffice.GetRandomValue(GPOffice.Mods.Keys.ToList());
        public static string mod2 = Mode2.ToString();

        public List<string> Modes = new List<string>() { mod1, mod2 };

        public List<string> pl = new List<string>();

        public void OnEnabled()
        {
            Task.WhenAll(
                OnModeStarted()
                );
        }

        public async Task OnModeStarted()
        {
            for (int i=0; i<2; i++)
            {
                while (new List<string>() { "더블업", "트리플업" }.Contains(Modes[i])) 
                    Modes[i] = GPOffice.GetRandomValue(GPOffice.Mods.Keys.ToList()).ToString();

                var modeType = Type.GetType($"GPOffice.Modes.{GPOffice.Mods[Modes[i]].ToString().Split('/')[2].Replace(" ", "")}");
                if (modeType != null)
                {
                    var modeInstance = Activator.CreateInstance(modeType);
                    var onEnabledMethod = modeType.GetMethod("OnEnabled");
                    onEnabledMethod?.Invoke(modeInstance, null);
                }
            }

            Player.List.ToList().ForEach(x => x.Broadcast(10, $"<size=25><b>[<color=#{GPOffice.Mods[mod1].ToString().Split('/')[0]}>{mod1}</color> + <b><color=#{GPOffice.Mods[mod2].ToString().Split('/')[0]}>{mod2}</color>]</b></size>"));
        }
    }
}
