using Terraria;
using Terraria.ModLoader;

namespace GuardianClass.Content.DamageClasses;

public sealed class GuardianDamage : DamageClass
{
    public override bool UseStandardCritCalcs { get; } = true;

    public override StatInheritanceData GetModifierInheritance(DamageClass damageClass) {
        var empty = new StatInheritanceData(
            0f,
            0f,
            0f,
            0f,
            0f
        );

        return damageClass == Generic ? StatInheritanceData.Full : empty;
    }

    public override bool ShowStatTooltipLine(Player player, string lineName) {
        return lineName != "Speed";
    }
}
