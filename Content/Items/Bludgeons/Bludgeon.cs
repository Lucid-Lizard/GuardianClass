﻿using GuardianClass.Content.DamageClasses;
using GuardianClass.Content.Projectiles;
using GuardianClass.ModPlayers;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace GuardianClass.Content.Items.Bludgeons
{
    public class Bludgeon : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 36;
            Item.height = 36;
            Item.DamageType = ModContent.GetInstance<GuardianDamage>();
            Item.knockBack = 6;
            Item.damage = 18;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.shoot = ModContent.ProjectileType<BludgeonProjectile>();            
            Item.shootSpeed = 45f;
            Item.useTime = 1;
            Item.noUseGraphic = true;
        }

        public override bool CanShoot(Player player)
        {
            

            return player.GetModPlayer<GuardianModPlayer>().UsingMace == false;
        }
    }

    public class BludgeonProjectile : ModProjectile
    {
        public override void SetDefaults()
        {

            Projectile.width = 36;
            Projectile.height = 36;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 60;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.aiStyle = -1;
            Projectile.tileCollide = false;
            
        }

        float Length;
        int OGDMG = 0;
        public int dir;
        public override void OnSpawn(IEntitySource source)
        {
            OGDMG = Projectile.damage;
            Length = Projectile.velocity.Length();
            Projectile.ai[0] = 0;
            Projectile.velocity = Vector2.Zero;

            var player = Main.player[Projectile.owner];

            player.GetModPlayer<GuardianModPlayer>().UsingMace = true;

            dir = player.direction;
        }

        public bool ImpactBool = false;
        public bool JustImpacted = false;
        public bool ShockCol = false;
        public int timer = 0;
        public override bool PreAI()
        {

            if (timer == (Length - 10))
            {
               // Main.NewText("Playing sound at projectile center");
                SoundEngine.PlaySound(new SoundStyle("GuardianClass/Assets/Sounds/GuardianSounds_MaceBash"), Projectile.Center);
            }

            if (ShockCol)
            {
                ShockCol = false;
            }
            return true;
        }
        public override void AI()
        {
           // Main.NewText(Projectile.ai[0]);
            timer++;
            Projectile.hide = true;

            var player = Main.player[Projectile.owner];

            player.GetModPlayer<GuardianModPlayer>().UsingMace = true;

            var playerCenter = player.Center - new Vector2((Projectile.width / 2), 0) - new Vector2(0, Projectile.height / 2);

            if (dir == 1)
            {
                player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, Projectile.rotation);
            }
            else
            {
                player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, Projectile.rotation - MathHelper.Pi);
            }

            

            Projectile.position = playerCenter;
            if (Projectile.ai[0]++ < Length && !ImpactBool)
            {
                
                Projectile.scale = MathHelper.Lerp(0f, 1, CoolMath.EaseOutBack(Projectile.ai[0] / Length));

                Projectile.velocity = new Vector2(0).RotatedBy(Projectile.rotation);
                if (dir == 1)
                {
                    Projectile.rotation = MathHelper.Lerp(0f - MathHelper.PiOver2, 0f + (MathHelper.Pi / 8f), MathHelper.Min((float)CoolMath.EaseInBack6(Projectile.ai[0] / Length), 1));
                } else
                {
                    Projectile.rotation = MathHelper.Lerp(0f - MathHelper.PiOver2, 0f - MathHelper.Pi - (MathHelper.Pi / 8f), MathHelper.Min((float)CoolMath.EaseInBack6(Projectile.ai[0] / Length), 1));
                }
                Projectile.timeLeft = 30;

                
               /* Projectile.rotation += MathHelper.PiOver4;*/
            } else
            {
                if (!JustImpacted)
                {
                    Impact();
                    
                }
                ImpactBool = true;
                
            }
        }

        public void Impact()
        {
            int ImpactWidth = 16;
            for(int i = 0; i < ImpactWidth; i++)
            {
                Vector2 dustPos = Projectile.Center / 16;
                dustPos += new Vector2(i - (ImpactWidth / 2), 0);
                if (!Main.tile[(int)dustPos.X, (int)dustPos.Y].HasTile)
                {
                    for(int j = 0; j < 8; j++)
                    {
                        dustPos.Y++;
                        if(Main.tile[(int)dustPos.X, (int)dustPos.Y].HasTile)
                        {
                            break;
                        }
                    }
                }
                Dust.NewDust(dustPos * 16, 6, 6, DustID.Dirt, 0, -5);
                if(Main.rand.Next(0,10) <= 3)
                {
                    Vector2 vel = new Vector2(i - (ImpactWidth / 2), -5);
                    vel *= Main.rand.NextFloat(1f, 2f);
                    Projectile.NewProjectile(Projectile.GetSource_FromAI(), dustPos * 16, vel, ModContent.ProjectileType<MaceTileProjectile>(), 6, 4f, ai0: Main.tile[(int)dustPos.X, (int)dustPos.Y].TileType);
                }
            }

            Main.player[Projectile.owner].GetModPlayer<CameraPlayer>().AddCameraShake(20, 5);

            JustImpacted = true;
            ShockCol = true;
        }

        public override void OnKill(int timeLeft)
        {
            var player = Main.player[Projectile.owner];
            player.SetItemAnimation(0);
            player.GetModPlayer<GuardianModPlayer>().UsingMace = false;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            var rot = Projectile.velocity;
            rot.Normalize();
            var start = Projectile.Center + Projectile.velocity - new Vector2(Projectile.height / 2, 0) * rot;
            var end = Projectile.Center + Projectile.velocity + new Vector2(Projectile.height / 2, 0) * rot;
            var collisionPoint = 0f; // Don't need that variable, but required as parameter

            bool Shock = false;
            if (ShockCol)

            {
                Vector2 projCenter = Projectile.Center / 16;
                int projX = (int)projCenter.X;
                Vector2 npcHitbox = new Vector2(targetHitbox.Center.X, targetHitbox.Center.Y) / 16;
                int npcX = (int)npcHitbox.X;
                if(npcX <= projX + 8 && npcX >= projX - 8)
                {
                    Shock = true;
                }
            }
            
            return (Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), start, end, Projectile.width, ref collisionPoint) || Shock) && !projHitbox.Intersects(targetHitbox);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            
            base.OnHitNPC(target, hit, damageDone);

            if (Projectile.ai[0] < 0)
            {
                target.velocity.Y -= 6f;
                target.velocity.X -= 3f * target.direction;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            var player = Main.player[Projectile.owner];
            Vector2 armpos;
            if (dir == 1)
            {
                 armpos = (Vector2)player.GetFrontHandPosition(Player.CompositeArmStretchAmount.Full, Projectile.rotation);
            }
            else
            {
                 armpos = (Vector2)player.GetFrontHandPosition(Player.CompositeArmStretchAmount.Full, Projectile.rotation - MathHelper.Pi);
            }
            


            Main.EntitySpriteDraw(
                TextureAssets.Projectile[Type].Value,
                armpos - Main.screenPosition ,
                TextureAssets.Projectile[Type].Value.Bounds,
                lightColor,
                Projectile.rotation + MathHelper.PiOver4,
                Vector2.Zero + new Vector2(0, 36),
                Projectile.scale,
                Microsoft.Xna.Framework.Graphics.SpriteEffects.None);
            return false;


        }

        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            overPlayers.Add(index);
        }
    }
}
