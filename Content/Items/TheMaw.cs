using GuardianClass.Content.Bases;
using System;

using Terraria.Audio;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using Terraria.GameContent;
using GuardianClass.ModPlayers;

using Microsoft.Xna.Framework;
using Terraria.GameContent.UI.ResourceSets;
using Microsoft.Xna.Framework.Graphics;
using System.Security.Cryptography.X509Certificates;
using System.Collections.Generic;

namespace GuardianClass.Content.Items
{
    public class TheMaw : GuardianShield
    {
        public override void SetGuardianDefaults()
        {
            ShieldProjectile = ModContent.ProjectileType<TheMawProjectile>();
            MaxDurability = 125;

            spawnTime = 15;
            thrustTime = 25;

            DurabilityStages = 1;


            Resistance = 0.25f;

            attackDistance = 25;
            idleDistance = 25;
        }
    }

    public class TheMawProjectile : GuardianShieldProjectile
    {
        public int StickedNPC = -1;
        public bool canStick = true;
        public int NPCPreviousDamage;
        public override void SetDefaults()
        {
            TextureName = "TheMawProjectile";
            Projectile.knockBack = 6;

            Projectile.width = 34;
            Projectile.height = 24;

        }



        public override void BlockNPCEffect(NPC npc)
        {
            npc.velocity.X = (MathF.Abs(npc.velocity.X)) * Projectile.direction;
            npc.velocity.Y = -3;
            //SoundEngine.PlaySound(new SoundStyle("GuardianClass/Sounds/GuardianSounds_WoodenShieldNPCBlock"));
        }

        public override void StrikeNPCEffect(NPC npc)
        {


            if (StickedNPC < 0 && canStick && npc.type != NPCID.TargetDummy && !Main.npc[npc.whoAmI].boss && npc.type != NPCID.DungeonGuardian && Main.npc[npc.whoAmI].life > 0)
            {
                StickedNPC = npc.whoAmI;
                Durability = npc.lifeMax;
                canStick = false;
                NPCPreviousDamage = npc.damage;
                MaxEat = npc.height;
                SoundEngine.PlaySound(new SoundStyle("GuardianClass/Sounds/GuardianSounds_MeatshieldImpale"));
            }
            else
            {
                SoundEngine.PlaySound(new SoundStyle("GuardianClass/Sounds/GuardianSounds_WoodenShieldNPCBlock"));
            }
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
            for (int i = 0; i < 3; i++)
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

            if (StickedNPC >= 0)
            {
                Main.npc[StickedNPC].StrikeInstantKill();
            }
        }

        public override void DespawnEffect()
        {
            if (StickedNPC >= 0)
            {
                Main.npc[StickedNPC].StrikeInstantKill();
            }
        }



        public int EatTimer = 0;
        public int MaxEat;
        public override void ConstantEffect()
        {
            Player player = Main.player[Projectile.owner];
            GuardianModPlayer g = player.GetModPlayer<GuardianModPlayer>();

            if (StickedNPC >= 0)
            {
                if (EatTimer++ < MaxEat)
                {
                    
                    //Main.NewText(StickedNPC);
                    //Override = true;
                    NPC npc = Main.npc[StickedNPC];
                    npc.Center = Projectile.Center + Projectile.velocity;

                    npc.hide = true;

                    npc.damage = 0;


                    npc.immune[Projectile.owner] = 20;

                    // HERE
                    if (EatTimer % 20 == 0)
                    {
                        SoundEngine.PlaySound(new SoundStyle("GuardianClass/Sounds/GuardianSounds_TheMawBite"), Projectile.Center);
                    }

            
                }
                else
                {
                    
                    Main.npc[StickedNPC].StrikeInstantKill();
                    SoundEngine.PlaySound(SoundID.NPCHit13, Projectile.position);
                    SoundEngine.PlaySound(SoundID.NPCDeath13, Projectile.position);
                    for (int i = 0; i < 10; i++)
                    {
                        Dust.NewDust(Projectile.Center + Projectile.velocity, 10, 10, DustID.Blood);
                    }

                    Vector2 projVel = Projectile.velocity;
                    projVel.Normalize();
                    projVel *= 10;

                    Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, projVel, ModContent.ProjectileType<TheMawSpitProjectile>(), 34, 7);

                    StickedNPC = -1;
                }
            }

        }


        
        public Texture2D text = TextureAssets.Item[1].Value;

        public override void PostDraw(Color lightColor)
        {
            base.PostDraw(lightColor);
            // Main.NewText(Override);
            if (StickedNPC >= 0)
            {
                bool shouldflip = Projectile.velocity.X < 0;

                NPC npc = Main.npc[StickedNPC];
                int width = npc.width;
                int height = npc.height;
                Rectangle rect = npc.frame;
                Main.NewText(width + " " + height + " " + text);
                text = TextureAssets.Npc[npc.type].Value;

                Vector2 pos;
                Vector2 vel;

                Player player = Main.player[Projectile.owner];

                

                pos = Projectile.Center;
                vel = Projectile.velocity;
                vel.Normalize();
                vel *= 17;
                pos += vel;


                rect.Height -= EatTimer;

                Main.EntitySpriteDraw(text, (pos) - Main.screenPosition, rect, Color.White, Projectile.rotation - MathHelper.Pi, new Vector2(rect.Width / 2, rect.Height / 2), 1f, SpriteEffects.FlipVertically, 0f);

                Texture2D texture = (Texture2D)ModContent.Request<Texture2D>("GuardianClass/Content/Items/" + TextureName); // Get the projectile's texture



                Vector2 position = Projectile.Center;





                Main.EntitySpriteDraw(texture, position - Main.screenPosition, new Rectangle(0, 0 + ((Projectile.height + 2) * 1), Projectile.width, Projectile.height), lightColor, Projectile.rotation, new Vector2(Projectile.width, Projectile.height) / 2f, Scale, shouldflip ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0f);

                
            }
        }

       
    }
    //
    public class TheMawSpitProjectile : ModProjectile
    {
        public override void SetDefaults()
        {
            
            Projectile.width = 12;
            Projectile.height = 12;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.aiStyle = ProjAIStyleID.Arrow;
            Projectile.timeLeft = 600;
        }

        public override void AI()
        {
            if(Main.rand.Next(10) < 2)
            {
                Dust.NewDust(Projectile.Center, 6, 6, DustID.Blood);
            }
        }
    }
}
