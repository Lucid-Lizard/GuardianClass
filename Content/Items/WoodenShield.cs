using GuardianClass.Content.Bases;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace GuardianClass.Content.Items
{
    public class WoodenShield : GuardianShield
    {
        public override void SetGuardianDefaults()
        {
            ShieldProjectile = ModContent.ProjectileType<WoodenShieldProjectile>();
            MaxDurability = 50;

            spawnTime = 15;
            thrustTime = 30;

            DurabilityStages = 3;
            Resistance = 0.25f;

            attackDistance = 15;
            idleDistance = 16;
        }
    }

    public class WoodenShieldProjectile : GuardianShieldProjectile
    {
        public override void SetDefaults()
        {
            TextureName = "WoodenShieldProjectile";
            Projectile.knockBack = 6;

            Projectile.width = 34;
            Projectile.height = 24;

        }

        public override void BlockNPCEffect(NPC npc)
        {
            
            //SoundEngine.PlaySound(new SoundStyle("GuardianClass/Sounds/GuardianSounds_WoodenShieldNPCBlock"));
        }

        public override void StrikeNPCEffect(NPC npc)
        {
            SoundEngine.PlaySound(new SoundStyle("GuardianClass/Sounds/GuardianSounds_WoodenShieldNPCBlock"));
            //npc.AddBuff(BuffID.Confused, 120);
        }

        public override void BlockProjectileEffect(Projectile proj)
        {
            SoundEngine.PlaySound(new SoundStyle("GuardianClass/Sounds/GuardianSounds_ArrowWoodImpact"));
            proj.velocity *= -1;
            proj.friendly = true;
            proj.hostile = false;
        }

        public override void StageChangeEffect()
        {
            SoundEngine.PlaySound(new SoundStyle("GuardianClass/Sounds/GuardianSounds_WoodenShield2"), Projectile.Center);
            for(int i = 0; i < 3; i++)
            {
                Dust.NewDust(Projectile.Center, 4, 4, DustID.WoodFurniture, 0, 0);
            }
        }

        public override void ShieldBreakEffect()
        {
            SoundEngine.PlaySound(new SoundStyle("GuardianClass/Sounds/GuardianSounds_WoodenShield1"), Projectile.Center);
            for (int i = 0; i < 7; i++)
            {
                Dust.NewDust(Projectile.Center, 4, 4, DustID.WoodFurniture, 0, 0);
            }
        }

    }
}
