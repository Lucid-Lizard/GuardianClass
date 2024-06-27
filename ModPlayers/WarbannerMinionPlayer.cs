using Microsoft.Xna.Framework;

using System.Collections.Generic;

using Terraria;
using Terraria.ModLoader;

namespace GuardianClass.ModPlayers
{
    public class Troop
    {
        public List<Projectile> Units;
        public List<NPC> Targets;
        public NPC Focus;
        public Projectile Captain;
        public string Name;

        public bool Attacking = false;
        public Troop(string name)
        {
            Units = new List<Projectile>();
            Targets = new List<NPC>();
            Name = name;
        }

        public void ChooseCaptain()
        {
            Captain = Units[0];
        }
    }
    public class WarbannerMinionPlayer : ModPlayer
    {
        public List<NPC> Targets = new List<NPC>();
        public List<Projectile> Minions = new List<Projectile>();
        public List<Troop> Troops = new List<Troop>();
        
        
        
        public void Clear()
        {
            Targets.Clear();
            Minions.Clear();
            Troops.Clear();
        }
        public void CreateTroops(int Minion, int Num, int TroopSize)
        {
            Troops = new List<Troop>();

            for (int i = 0; i < TroopSize; i++)
            {
                Troops.Add(new Troop($"Troop{i + 1}")); // Adjusted to use i+1 for naming
                Troops[i].Units = new List<Projectile>();
                for (int j = 0; j < Num / TroopSize; j++)
                {
                    int p = Projectile.NewProjectile(Player.GetSource_FromThis(), Player.position, Vector2.Zero, Minion, 1, 0, Player.whoAmI);

                    Troops[i].Units.Add(Main.projectile[p]); // Adjusted to use i as the index
                }
                Troops[i].ChooseCaptain();
            }
        }


        public bool GetMinionTroop(Projectile Minion, out Troop Troop)
        {
            foreach(Troop t in Troops)
            {
                if (t.Units.Contains(Minion))
                {
                    Troop = t;
                    return true;
                }
            }
            Troop = null;
            return false;
        }

        public bool GetTroopPos(Projectile Minion, out int MinionPos)
        {
            if(GetMinionTroop(Minion, out Troop troop))
            {
                MinionPos = troop.Units.IndexOf(Minion);
                return true;
            }

            MinionPos = -1;
            return false;
        }

        public bool Refocus(Projectile Minion)
        {
            if (GetMinionTroop(Minion, out Troop troop))
            {
                for (int i = 0; i < Main.npc.Length; i++)
                {
                    
                    NPC npc = Main.npc[i];

                    if(npc.friendly || npc.CountsAsACritter)
                    {
                        break;
                    }

                    if (troop.Targets.Contains(npc))
                    {
                        if (Vector2.Distance(npc.Center, Minion.Center) > 500f)
                        {
                            troop.Targets.Remove(npc);
                        }

                        break;
                    }

                    if (Vector2.Distance(npc.Center, Minion.Center) < 500f)
                    {
                        troop.Targets.Add(npc);
                    }

                    NPC nearest = null;

                    foreach (NPC npc2 in troop.Targets)
                    {
                        if (nearest == null)
                        {
                            nearest = npc2;
                        }
                        else
                        {
                            if (Vector2.Distance(nearest.Center, Minion.Center) > Vector2.Distance(npc.Center, Minion.Center))
                            {
                                nearest = npc;
                            }
                        }
                    }

                    troop.Focus = nearest;

                    troop.Attacking = troop.Focus != null;
                }
                return true;
            }

            return false;
        }

        public void DestroyTroop(Troop troop)
        {
            foreach(Projectile p in troop.Units)
            {
                FindNewTroop(p);
            }
            Troops.Remove(troop);
        }

        public void FindNewTroop(Projectile Minion)
        {
            if(Troops.Count - 1 > 0) {
                Troop t = Troops[0];
                foreach (Troop troop in Troops)
                {
                    if (!troop.Captain.active)
                    {
                        return;
                    }
                    if (Vector2.Distance(Minion.Center, troop.Captain.Center) < Vector2.Distance(Minion.Center, t.Captain.Center))
                    {
                        t = troop;
                    }
                }

                if (t != null)
                {
                    t.Units.Add(Minion);
                }
            } else
            {
                Minion.Kill();
            }
            

            
        }
    }
}
