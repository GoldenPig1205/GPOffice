using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CustomRendering;
using Exiled.API.Features;
using Exiled.API.Features.Items;
using MEC;
using Mirror;
using UnityEngine;

namespace GPOffice.Modes
{
    class AmongUs
    {
        public static AmongUs Instance;

        List<Player> Imposters = new List<Player>();
        List<Player> Crews = new List<Player>();

        public void OnEnabled()
        {
            Timing.RunCoroutine(OnModeStarted());
        }

        public IEnumerator<float> OnModeStarted()
        {
            yield return Timing.WaitForSeconds(10f);

            int totalPlayers = Player.List.Count;
            int numImposters = totalPlayers / 10;

            List<Player> players = Player.List.ToList();
            for (int i = 0; i < numImposters; i++)
            {
                int randomIndex = UnityEngine.Random.Range(0, players.Count);
                Player imposter = players[randomIndex];
                Imposters.Add(imposter);
                players.RemoveAt(randomIndex);
            }

            Crews.AddRange(players);
        }
    }
}
