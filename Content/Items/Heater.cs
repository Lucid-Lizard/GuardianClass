using GuardianClass.Content.Bases;
using GuardianClass.ModPlayers;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.Chat;
using Terraria.GameContent.UI.Chat;
using Terraria.ID;
using Terraria.ModLoader;

namespace GuardianClass.Content.Items
{
    public class Heater : GuardianShield
    {
        public override void SetGuardianDefaults()
        {
            ShieldProjectile = ModContent.ProjectileType<HeaterProjectile>();
            MaxDurability = 200;

            SpawnTime = 15;
            ThrustTime = 17;

            DurabilityStages = 1;
            Resistance = 0.23f;

            AttackDistance = 26;
            IdleDistance = 27;
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.damage = 53;
        }


        
    }

    public class HeaterProjectile : GuardianShieldProjectile
    {
        public override void SetDefaults()
        {
            TextureName = "HeaterProjectile";
            Projectile.knockBack = 6;

            Projectile.width = 40;
            Projectile.height = 30;

        }

        public override void BlockNPCEffect(NPC npc)
        {

            npc.AddBuff(BuffID.OnFire, 120);
            //SoundEngine.PlaySound(new SoundStyle("GuardianClass/Sounds/GuardianSounds_WoodenShieldNPCBlock"));

            
        }

        public override void StrikeNPCEffect(NPC npc)
        {
            SoundEngine.PlaySound(new SoundStyle("GuardianClass/Assets/Sounds/GuardianSounds_WoodenShieldNPCBlock"));
            //npc.AddBuff(BuffID.Confused, 120);
            npc.AddBuff(BuffID.OnFire, 120);
        }

        public override void BlockProjectileEffect(Projectile proj)
        {
            SoundEngine.PlaySound(new SoundStyle("GuardianClass/Assets/Sounds/GuardianSounds_ArrowWoodImpact"));

            Vector2 vel = Projectile.velocity;
            vel.Normalize();
            vel *= 6;
            
                int pro = Projectile.NewProjectile(Projectile.GetSource_FromAI(), proj.position, vel, ProjectileID.HellfireArrow, 35, 4, Projectile.owner);
                Main.projectile[pro].friendly = true;
                Main.projectile[pro].hostile = false;

            proj.netImportant = true;
            proj.Kill();
            

            for(int i = 0; i < Main.maxPlayers; i++)
            {
                Main.player[i].GetModPlayer<GuardianModPlayer>().AddWard(2);
            }

          
        }

        public override void StageChangeEffect()
        {
            SoundEngine.PlaySound(new SoundStyle("GuardianClass/Assets/Sounds/GuardianSounds_WoodenShield2"), Projectile.Center);
            for (int i = 0; i < 3; i++)
            {
                Dust.NewDust(Projectile.Center, 4, 4, DustID.Lava, 0, 0);
            }
        }

        public override void ShieldBreakEffect()
        {
            SoundEngine.PlaySound(new SoundStyle("GuardianClass/Assets/Sounds/GuardianSounds_WoodenShield1"), Projectile.Center);
            for (int i = 0; i < 7; i++)
            {
                Dust.NewDust(Projectile.Center, 4, 4, DustID.Lava, 0, 0);
            }
        }

        public override void PostDraw(Color lightColor)
        {

            Texture2D texture = (Texture2D)ModContent.Request<Texture2D>("GuardianClass/Content/Items/" + TextureName); // Get the projectile's texture



            Vector2 position = Projectile.Center;
            bool shouldflip = Projectile.velocity.X < 0;

            Projectile.ai[0]++;


            Main.EntitySpriteDraw(texture, position - Main.screenPosition, new Rectangle(0, 0 + ((Projectile.height + 2) * 1), Projectile.width, Projectile.height), Color.White * 0.5f, Projectile.rotation, new Vector2(Projectile.width, Projectile.height) / 2f, Scale + (0.5f * MathF.Sin(Projectile.ai[0] / 100)), shouldflip ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0f);

            base.PostDraw(lightColor);
        }

    }
}
