﻿
using Terraria.ID;
using Terraria.ModLoader;
using Terraria;
using GuardianClass.Content.Bases;
using Terraria.Audio;
using System.Numerics;
using Microsoft.Xna.Framework;

namespace GuardianClass.Content.Items
{
    public class SapphireShield : GuardianShield
    {
        public override void SetGuardianDefaults()
        {
            ShieldProjectile = ModContent.ProjectileType<SapphireShieldProjectile>();
            MaxDurability = 85;

            SpawnTime = 15;
            ThrustTime = 29;

            DurabilityStages = 3;
            Resistance = 0.225f;

            AttackDistance = 22;
            IdleDistance = 20;
            Item.damage = 45;
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemID.Sapphire, 17);
            recipe.AddTile(TileID.Anvils);
            recipe.Register();
        }
    }

    public class SapphireShieldProjectile : GuardianShieldProjectile
    {
        public override void SetDefaults()
        {
            TextureName = "SapphireShieldProjectile";
            Projectile.knockBack = 6;

            Projectile.width = 34;
            Projectile.height = 30;
        }

        public override void AI()
        {
            base.AI();
            
        }

        public override void ConstantEffect()
        {
            
            Microsoft.Xna.Framework.Vector2 vel = Projectile.velocity;
            vel.Normalize();
            vel *= MathHelper.Max( (Projectile.velocity.Length() - 20f) / 5f, 0f);
                int d = Dust.NewDust(Projectile.Center + Projectile.velocity, 2, 2, DustID.GemSapphire, vel.X * -1, vel.Y * -1);
                Main.dust[d].noGravity = true;
            
        }
        public override void BlockNPCEffect(NPC npc)
        {
            //SoundEngine.PlaySound(new SoundStyle("GuardianClass/Assets/Sounds/GuardianSounds_WoodenShieldNPCBlock"));
        }

        public override void StrikeNPCEffect(NPC npc)
        {
            SoundEngine.PlaySound(new SoundStyle("GuardianClass/Assets/Sounds/GuardianSounds_WoodenShieldNPCBlock"));
            //npc.AddBuff(BuffID.Confused, 120);
        }

        public override void BlockProjectileEffect(Projectile proj)
        {
            SoundEngine.PlaySound(new SoundStyle("GuardianClass/Assets/Sounds/GuardianSounds_ArrowWoodImpact"));
            
        }

        public override void StageChangeEffect()
        {
            SoundEngine.PlaySound(new SoundStyle("GuardianClass/Assets/Sounds/GuardianSounds_WoodenShield2"), Projectile.Center);
            for (var i = 0; i < 3; i++)
            {
                Dust.NewDust(Projectile.Center, 4, 4, DustID.GemSapphire);
            }
        }

        public override void ShieldBreakEffect()
        {
            SoundEngine.PlaySound(new SoundStyle("GuardianClass/Assets/Sounds/GuardianSounds_WoodenShield1"), Projectile.Center);
            for (var i = 0; i < 7; i++)
            {
                Dust.NewDust(Projectile.Center, 4, 4, DustID.GemSapphire);
            }
        }
    }
}
