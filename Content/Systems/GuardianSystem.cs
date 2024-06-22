using GuardianClass.Content.DamageClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace GuardianClass.Content.Systems
{
    internal class GuardianSystem : GlobalItem
    {
        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            base.ModifyTooltips(item, tooltips);

            if(item.DamageType == ModContent.GetInstance<GuardianDamage>())
            {
                TooltipLine tt = tooltips.FirstOrDefault(x => x.Name == "Damage" && x.Mod == "Terraria");
                if (tt != null)
                {
                    string[] splitText = tt.Text.Split(' ');
                    string damageValue = splitText.First();
                    string damageWord = splitText.Last();
                    tt.Text = damageValue + " guardian " + "damage";
                }
            }
        }
    }
}
