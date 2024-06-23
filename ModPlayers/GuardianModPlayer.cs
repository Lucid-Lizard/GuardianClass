using System.Collections.Generic;
using GuardianClass.Content.Bases;
using Terraria.ModLoader;

namespace GuardianClass.ModPlayers;

public class GuardianModPlayer : ModPlayer
{
    public GuardianShield CurrentShield;
    
    public int LastDurability = -1;
    public int LastShieldType;

    public override void PreUpdate() {
        if (GuardianSystem.GuardianShieldItems.Contains(Player.HeldItem.type)) {
            CurrentShield = (GuardianShield)Player.HeldItem.ModItem;
        }
    }
}

public class GuardianSystem : ModSystem
{
    public static List<int> GuardianShieldItems = new();
}
