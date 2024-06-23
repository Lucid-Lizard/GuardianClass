using System.Collections.Generic;
using GuardianClass.Content.Bases;
using Terraria.ModLoader;

namespace GuardianClass.ModPlayers;

public class GuardianModPlayer : ModPlayer
{
    public GuardianShield currentShield;
    public int LastDurability = -1;
    public int LastShieldType;

    public override void PreUpdate() {
        if (GuardianSystem.GuardianShieldItems.Contains(Player.HeldItem.type)) {
            currentShield = (GuardianShield)Player.HeldItem.ModItem;
        }

        if (currentShield != null) {
            //Main.NewText(currentShield.Name);
        }
    }


    public override void PostUpdate() {
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
    public static List<int> GuardianShieldItems = new();
}
