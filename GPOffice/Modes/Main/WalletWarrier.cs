using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Exiled.API.Features;
using MEC;
using Mirror;
using UnityEngine;

namespace GPOffice.Modes
{
    class WalletWarrier
    {
        public static WalletWarrier Instance;

        public float mh = 100;

        public void OnEnabled()
        {
            Timing.RunCoroutine(OnModeStarted());
        }
        
        public IEnumerator<float> OnModeStarted()
        {
            while (true)
            {
                foreach (var p in Player.List)
                {
                    if (!p.IsScp)
                    {
                        int coins = p.Items.Count(item => item.Type == ItemType.Coin);
                        switch (coins)
                        {
                            case 0:
                                mh = 100;

                                p.MaxHealth = mh;
                                if (p.Health > mh)
                                    p.Health = mh;
                                p.Scale = new Vector3(1f, 1f, 1f);
                                break;

                            case 1:
                                mh = 120;

                                p.MaxHealth = mh;
                                if (p.Health > mh)
                                    p.Health = mh;
                                p.EnableEffect(Exiled.API.Enums.EffectType.MovementBoost, 10, 2);
                                p.Scale = new Vector3(0.95f, 0.95f, 0.95f);
                                break;

                            case 2:
                                mh = 150;

                                p.MaxHealth = mh;
                                if (p.Health > mh)
                                    p.Health = mh;
                                p.EnableEffect(Exiled.API.Enums.EffectType.MovementBoost, 20, 2);
                                p.EnableEffect(Exiled.API.Enums.EffectType.DamageReduction, 1, 2);
                                p.Scale = new Vector3(0.9f, 0.9f, 0.9f);
                                break;

                            case 3:
                                mh = 200;

                                p.MaxHealth = mh;
                                if (p.Health > mh)
                                    p.Health = mh;
                                p.EnableEffect(Exiled.API.Enums.EffectType.MovementBoost, 30, 2);
                                p.EnableEffect(Exiled.API.Enums.EffectType.DamageReduction, 1, 2);
                                p.AddAhp(1);
                                p.Scale = new Vector3(0.85f, 0.85f, 0.85f);
                                break;

                            case 4:
                                mh = 300;

                                p.MaxHealth = mh;
                                if (p.Health > mh)
                                    p.Health = mh;
                                p.EnableEffect(Exiled.API.Enums.EffectType.MovementBoost, 40, 2);
                                p.EnableEffect(Exiled.API.Enums.EffectType.DamageReduction, 2, 2);
                                p.Scale = new Vector3(0.8f, 0.8f, 0.8f);
                                p.AddAhp(1);
                                p.Heal(1);
                                break;

                            case 5:
                                mh = 450;

                                p.MaxHealth = mh;
                                if (p.Health > mh)
                                    p.Health = mh;
                                p.EnableEffect(Exiled.API.Enums.EffectType.MovementBoost, 50, 2);
                                p.EnableEffect(Exiled.API.Enums.EffectType.DamageReduction, 2, 2);
                                p.Scale = new Vector3(0.75f, 0.75f, 0.75f);
                                p.AddAhp(2);
                                p.Heal(2);
                                break;

                            case 6:
                                mh = 600;

                                p.MaxHealth = mh;
                                if (p.Health > mh)
                                    p.Health = mh;
                                p.EnableEffect(Exiled.API.Enums.EffectType.MovementBoost, 60, 2);
                                p.EnableEffect(Exiled.API.Enums.EffectType.DamageReduction, 3, 2);
                                p.Scale = new Vector3(0.7f, 0.7f, 0.7f);
                                p.AddAhp(3);
                                p.Heal(3);
                                break;

                            case 7:
                                mh = 800;

                                p.MaxHealth = mh;
                                if (p.Health > mh)
                                    p.Health = mh;
                                p.EnableEffect(Exiled.API.Enums.EffectType.MovementBoost, 70, 2);
                                p.EnableEffect(Exiled.API.Enums.EffectType.DamageReduction, 4, 2);
                                p.Scale = new Vector3(0.65f, 0.65f, 0.65f);
                                p.AddAhp(4);
                                p.Heal(5);
                                break;

                            case 8:
                                mh = 1000;

                                p.MaxHealth = mh;
                                if (p.Health > mh)
                                    p.Health = mh;
                                p.EnableEffect(Exiled.API.Enums.EffectType.MovementBoost, 80, 2);
                                p.EnableEffect(Exiled.API.Enums.EffectType.DamageReduction, 5, 2);
                                p.Scale = new Vector3(0.6f, 0.6f, 0.6f);
                                p.AddAhp(5);
                                p.Heal(10);
                                break;
                        }
                    }
                }
                yield return Timing.WaitForSeconds(1f); 
            }
        }
    }
}
