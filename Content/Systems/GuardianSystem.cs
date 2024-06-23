using System.Collections.Generic;
using System.Linq;
using GuardianClass.Content.DamageClasses;
using Terraria;
using Terraria.ModLoader;

namespace GuardianClass.Content.Systems;

internal class GuardianSystem : GlobalItem
{
    public override void ModifyTooltips(Item item, List<TooltipLine> tooltips) {
        if (item.DamageType == ModContent.GetInstance<GuardianDamage>()) {
            var tt = tooltips.FirstOrDefault(x => x.Name == "Damage" && x.Mod == "Terraria");
            if (tt != null) {
                var splitText = tt.Text.Split(' ');
                var damageValue = splitText.First();
                var damageWord = splitText.Last();
                tt.Text = damageValue + " guardian " + "damage";
            }
        }
    }
}
