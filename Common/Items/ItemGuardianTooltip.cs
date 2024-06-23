using System.Collections.Generic;
using System.Linq;
using GuardianClass.Content.DamageClasses;
using Terraria;
using Terraria.ModLoader;

namespace GuardianClass.Common.Items;

public sealed class ItemGuardianTooltip : GlobalItem
{
    public override bool AppliesToEntity(Item entity, bool lateInstantiation) {
        return entity.DamageType == ModContent.GetInstance<GuardianDamage>();
    }

    public override void ModifyTooltips(Item item, List<TooltipLine> tooltips) {
        var tooltip = tooltips.FirstOrDefault(line => line.Name == "Damage" && line.Mod == "Terraria");
        
        if (tooltip == null) {
            return;
        }
        
        var split = tooltip.Text.Split(' ');
        var damage = split.First();
        
        tooltip.Text = $"{damage} guardian damage";
    }
}
