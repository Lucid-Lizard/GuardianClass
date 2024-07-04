using GuardianClass.Content.DamageClasses;
using GuardianClass.ModPlayers;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace GuardianClass.Content.Bases;

public abstract class GuardianShield : ModItem
{
    public float AttackDistance;

    public int DurabilityStages = 3;
    public float IdleDistance = 25;

    public int MaxDurability = 100;
    public float Resistance = 0.25f;
    public int ShieldProjectile;

    public int SpawnTime;
    public int ThrustTime;

    public virtual void SetGuardianDefaults() { }

    public override void SetStaticDefaults() {
        GuardianSystem.GuardianShieldItems.Add(Item.type);
    }

    public override void SetDefaults() {
        Item.DamageType = ModContent.GetInstance<GuardianDamage>();
        
        // Should stats really be fixed to all shields?
        Item.damage = 24;
        Item.knockBack = 6f;

        Item.noUseGraphic = true;
        Item.autoReuse = false;
        Item.channel = true;
        
        // This should also match the dimensions of each shield individually.
        Item.width = 40;
        Item.height = 40;
        
        // I'm not quite sure why the absurd values are required. Surely there's an alternative method.
        Item.useTime = 120;
        Item.useAnimation = 1;
        Item.useStyle = ItemUseStyleID.Shoot;
        Item.UseSound = SoundID.Item1;
        
        Item.value = Item.buyPrice(silver: 1);
        Item.rare = ItemRarityID.Blue;

        SetGuardianDefaults();
    }

    // TODO: This should probably be handled by ModItem::Shoot().
    public override bool? UseItem(Player player) {
        if (player.ownedProjectileCounts[ShieldProjectile] == 0 && player.GetModPlayer<GuardianModPlayer>().ShieldUsable) {
            Projectile.NewProjectile(player.GetSource_FromThis(), player.position, Vector2.Zero, ShieldProjectile, Item.damage, 0);
        }

        return base.UseItem(player);
    }
}
