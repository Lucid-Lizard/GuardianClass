using GuardianClass.Content.Bases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace GuardianClass.ModPlayers
{
    public class GuardianModPlayer : ModPlayer
    {
        public int LastDurability = -1;
        public int LastShieldType;

        public GuardianShield currentShield;

        public override void PreUpdate()
        {
            if(GuardianSystem.GuardianShieldItems.Contains(Player.HeldItem.type))
            {
                currentShield = (GuardianShield)Player.HeldItem.ModItem;
            }
            if(currentShield != null)
            {
                //Main.NewText(currentShield.Name);
            }
        }

        
        public override void PostUpdate()
        {
            /*if (GuardianSystem.GuardianShieldItems.Contains(Player.HeldItem.type))
            {
                if (currentShield != null && currentShield.ShieldProjectile != LastShieldType)
                {
                    LastShieldType = currentShield.ShieldProjectile;
                }
            }*/
        }
    }

    public class GuardianSystem : ModSystem
    {
        public static List<int> GuardianShieldItems = new List<int>();
    }
}
