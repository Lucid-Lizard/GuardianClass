using GuardianClass.Content.DamageClasses;
using GuardianClass.ModPlayers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace GuardianClass.Content.Items.Accessories
{
    public class ShieldPolish : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 24;
            Item.height = 20;

            Item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            // These 2 stat changes are equal to the Lightning Boots
            player.GetDamage(ModContent.GetInstance<GuardianDamage>()) += 0.08f;

            var g = player.GetModPlayer<GuardianModPlayer>();
            g.ShieldPolish = true;
        }
    }
}
