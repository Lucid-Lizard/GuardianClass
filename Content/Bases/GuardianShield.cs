using GuardianClass.Content.DamageClasses;
using GuardianClass.ModPlayers;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace GuardianClass.Content.Bases;

public abstract class GuardianShield : ModItem
{
    public float attackDistance;

    public int DurabilityStages = 3;
    public float idleDistance = 25;

    public int MaxDurability = 100;
    public float Resistance = 0.25f;
    public int ShieldProjectile;

    public int spawnTime;
    public int thrustTime;


    public virtual void SetGuardianDefaults() { }

    public override void SetStaticDefaults() {
        GuardianSystem.GuardianShieldItems.Add(Item.type);
    }

    public override void SetDefaults() {
        Item.damage = 24;
        Item.DamageType = ModContent.GetInstance<GuardianDamage>();
        Item.width = 40;
        Item.height = 40;
        Item.useTime = 120;
        Item.useAnimation = 1;
        Item.useStyle = ItemUseStyleID.Shoot;
        Item.noUseGraphic = true;
        Item.knockBack = 6;
        Item.value = Item.buyPrice(silver: 1);
        Item.rare = ItemRarityID.Blue;
        Item.UseSound = SoundID.Item1;
        Item.autoReuse = false;
        Item.channel = true;
        SetGuardianDefaults();
    }

    public override bool? UseItem(Player player) {
        if (player.ownedProjectileCounts[ShieldProjectile] == 0) {
            Projectile.NewProjectile(player.GetSource_FromThis(), player.position, Vector2.Zero, ShieldProjectile, Item.damage, 0);
        }

        return base.UseItem(player);
    }
}
