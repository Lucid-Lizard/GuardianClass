﻿using GuardianClass.Content.Bases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.Audio;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;

namespace GuardianClass.Content.Items
{
    public class GoldShield : GuardianShield
    {
        public override void SetGuardianDefaults()
        {
            ShieldProjectile = ModContent.ProjectileType<GoldShieldProjectile>();
            MaxDurability = 65;

            SpawnTime = 15;
            ThrustTime = 30;

            DurabilityStages = 3;
            Resistance = 0.23f;

            AttackDistance = 18;
            IdleDistance = 18;
            Item.damage = 34;
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemID.GoldBar, 12);
            recipe.AddTile(TileID.Anvils);
            recipe.Register();
        }
    }

    public class GoldShieldProjectile : GuardianShieldProjectile
    {
        public override void SetDefaults()
        {
            TextureName = "GoldShieldProjectile";
            Projectile.knockBack = 6;

            Projectile.width = 36;
            Projectile.height = 22;
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
            proj.velocity *= -1;
            proj.friendly = true;
            proj.hostile = false;
        }

        public override void StageChangeEffect()
        {
            SoundEngine.PlaySound(new SoundStyle("GuardianClass/Assets/Sounds/GuardianSounds_WoodenShield2"), Projectile.Center);
            for (var i = 0; i < 3; i++)
            {
                Dust.NewDust(Projectile.Center, 4, 4, DustID.Gold);
            }
        }

        public override void ShieldBreakEffect()
        {
            SoundEngine.PlaySound(new SoundStyle("GuardianClass/Assets/Sounds/GuardianSounds_WoodenShield1"), Projectile.Center);
            for (var i = 0; i < 7; i++)
            {
                Dust.NewDust(Projectile.Center, 4, 4, DustID.Gold);
            }
        }
    }
}