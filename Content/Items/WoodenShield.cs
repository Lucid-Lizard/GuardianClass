using GuardianClass.Content.Bases;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace GuardianClass.Content.Items;

public class WoodenShield : GuardianShield
{
    public override void SetGuardianDefaults() {
        ShieldProjectile = ModContent.ProjectileType<WoodenShieldProjectile>();
        MaxDurability = 50;

        SpawnTime = 15;
        ThrustTime = 30;

        DurabilityStages = 3;
        Resistance = 0.25f;

        AttackDistance = 15;
        IdleDistance = 16;
    }

    public override void AddRecipes()
    {
        Recipe recipe = CreateRecipe();
        recipe.AddIngredient(ItemID.Wood, 12);
        recipe.AddTile(TileID.WorkBenches);
        recipe.Register();
    }
}

public class WoodenShieldProjectile : GuardianShieldProjectile
{
    public override void SetDefaults() {
        TextureName = "WoodenShieldProjectile";
        Projectile.knockBack = 6;

        Projectile.width = 34;
        Projectile.height = 24;
    }

    public override void BlockNPCEffect(NPC npc) {
        //SoundEngine.PlaySound(new SoundStyle("GuardianClass/Assets/Sounds/GuardianSounds_WoodenShieldNPCBlock"));
    }

    public override void StrikeNPCEffect(NPC npc) {
        SoundEngine.PlaySound(new SoundStyle("GuardianClass/Assets/Sounds/GuardianSounds_WoodenShieldNPCBlock"));
        //npc.AddBuff(BuffID.Confused, 120);
    }

    public override void BlockProjectileEffect(Projectile proj) {
        SoundEngine.PlaySound(new SoundStyle("GuardianClass/Assets/Sounds/GuardianSounds_ArrowWoodImpact"));
        proj.velocity *= -1;
        proj.friendly = true;
        proj.hostile = false;
    }

    public override void StageChangeEffect() {
        SoundEngine.PlaySound(new SoundStyle("GuardianClass/Assets/Sounds/GuardianSounds_WoodenShield2"), Projectile.Center);
        for (var i = 0; i < 3; i++) {
            Dust.NewDust(Projectile.Center, 4, 4, DustID.WoodFurniture);
        }
    }

    public override void ShieldBreakEffect() {
        
        SoundEngine.PlaySound(new SoundStyle("GuardianClass/Assets/Sounds/GuardianSounds_WoodenShield1"), Projectile.Center);
        for (var i = 0; i < 7; i++) {
            Dust.NewDust(Projectile.Center, 4, 4, DustID.WoodFurniture);
        }
    }
}
