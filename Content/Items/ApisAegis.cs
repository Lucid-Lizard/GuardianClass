﻿using GuardianClass.Content.Bases;
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
    public class ApisAegis : GuardianShield
    {
        public override void SetGuardianDefaults()
        {
            ShieldProjectile = ModContent.ProjectileType<ApisAegisProjectile>();
            MaxDurability = 200;

            spawnTime = 15;
            thrustTime = 5;

            DurabilityStages = 3;
            Resistance = 0.23f;

            attackDistance = 21;
            idleDistance = 27;
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.damage = 53;
        }
    }

    public class ApisAegisProjectile : GuardianShieldProjectile
    {
        public override void SetDefaults()
        {
            TextureName = "ApisAegisProjectile";
            Projectile.knockBack = 6;
            
            Projectile.width = 40;
            Projectile.height = 28;

        }

        public override void BlockNPCEffect(NPC npc)
        {
            npc.velocity.X = (MathF.Abs(npc.velocity.X)) * Projectile.direction;
            npc.velocity.Y = -3;
            npc.AddBuff(BuffID.Honey, 120);
            //SoundEngine.PlaySound(new SoundStyle("GuardianClass/Sounds/GuardianSounds_WoodenShieldNPCBlock"));

            for (int i = 0; i < Main.rand.Next(2, 4); i++)
            {
                int npc2 = NPC.NewNPC(Projectile.GetSource_FromAI(), (int)Projectile.position.X, (int)Projectile.position.Y, NPCID.BeeSmall);
                Main.npc[npc2].friendly = true;
            }
        }

        public override void StrikeNPCEffect(NPC npc)
        {
            SoundEngine.PlaySound(new SoundStyle("GuardianClass/Sounds/GuardianSounds_WoodenShieldNPCBlock"));
            //npc.AddBuff(BuffID.Confused, 120);
        }

        public override void BlockProjectileEffect(Projectile proj)
        {
            SoundEngine.PlaySound(new SoundStyle("GuardianClass/Sounds/GuardianSounds_ArrowWoodImpact"));
           

            for (int i = 0; i < Main.rand.Next(2, 4); i++)
            {
                int npc = NPC.NewNPC(Projectile.GetSource_FromAI(), (int)Projectile.position.X, (int)Projectile.position.Y, NPCID.BeeSmall);
                Main.npc[npc].friendly = true;
                

            }
        }

        public override void StageChangeEffect()
        {
            SoundEngine.PlaySound(new SoundStyle("GuardianClass/Sounds/GuardianSounds_WoodenShield2"), Projectile.Center);
            for(int i = 0; i < 3; i++)
            {
                Dust.NewDust(Projectile.Center, 4, 4, DustID.Hive, 0, 0);
            }
        }

        public override void ShieldBreakEffect()
        {
            SoundEngine.PlaySound(new SoundStyle("GuardianClass/Sounds/GuardianSounds_WoodenShield1"), Projectile.Center);
            for (int i = 0; i < 7; i++)
            {
                Dust.NewDust(Projectile.Center, 4, 4, DustID.Hive, 0, 0);
            }
        }

    }
}