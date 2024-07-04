using GuardianClass.Content.DamageClasses;
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
    public class Tectonic : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 48;
            Item.height = 48;
            Item.DamageType = ModContent.GetInstance<GuardianDamage>();
            Item.knockBack = 8;
            Item.damage = 36;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.shoot = ModContent.ProjectileType<TectonicProjectile>();            
            Item.shootSpeed = 75f;
            Item.useTime = 1;
            Item.noUseGraphic = true;
        }

        public override bool CanShoot(Player player)
        {
            

            return player.GetModPlayer<GuardianModPlayer>().UsingMace == false;
        }
    }

    public class TectonicProjectile : ModProjectile
    {
        public override void SetDefaults()
        {

            Projectile.width = 48;
            Projectile.height = 48;
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
                
                Projectile.scale = MathHelper.Lerp(0f, 1f, CoolMath.EaseOutBack(Projectile.ai[0] / Length));

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
            int ImpactWidth = 24;
            for(int i = 0; i < ImpactWidth; i++)
            {
                Vector2 dustPos = Projectile.Center / 16;
                dustPos += new Vector2(i - (ImpactWidth / 2), 0);
                if (!Main.tile[(int)dustPos.X, (int)dustPos.Y].HasTile)
                {
                    for(int j = 0; j < 100; j++)
                    {
                        dustPos.Y++;
                        if(Main.tile[(int)dustPos.X, (int)dustPos.Y].HasTile)
                        {
                            break;
                        }
                    }
                }
                int d = Dust.NewDust(dustPos * 16, 6, 6, DustID.InfernoFork, 0, -5);
               
                if(i % 4 == 0 && i > 0)
                {
                    Vector2 vel = new Vector2(i - (ImpactWidth / 2), -5);
                    vel *= Main.rand.NextFloat(1f, 2f);
                    Projectile.NewProjectile(Projectile.GetSource_FromAI(), dustPos * 16, Vector2.Zero, ModContent.ProjectileType<TectonicPlateProjectile>(), 6, 4f, ai0: i - (ImpactWidth / 2));
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
                Vector2.Zero + new Vector2(0, 48),
                Projectile.scale,
                Microsoft.Xna.Framework.Graphics.SpriteEffects.None);
            return false;


        }

        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            overPlayers.Add(index);
        }
    }

    public class TectonicPlateProjectile : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 48;
            Projectile.height = 148;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 300;
            Projectile.tileCollide = false;
            Projectile.alpha = 255;
            Projectile.hide = true;

        }

        public int timer = 0;
        public int countdown = 0;

        public override void OnSpawn(IEntitySource source)
        {
            Projectile.rotation = 0 + (MathHelper.Pi / 35) * Projectile.ai[0];
        }
        public override bool PreAI()
        {
            Projectile.scale = 0;
            if(countdown++ > 5 * MathF.Abs(Projectile.ai[0]))
            {
                
                return true;
            }
            Projectile.timeLeft = 300;
            return false;
        }
        public override void AI()
        {
            if(timer == 0)
            {
                SoundEngine.PlaySound(new SoundStyle("GuardianClass/Assets/Sounds/GuardianSounds_Tectonic"), Projectile.Center);
            }
            timer++;
            if(timer <= 265) 
                Projectile.scale = MathHelper.Lerp(0, 1f, CoolMath.InOutQuadBlend(MathF.Min(timer, 15) / 15));
            else
                Projectile.scale = MathHelper.Lerp(1f, 0, CoolMath.EaseInBack(MathF.Min(timer - 265, 35) / 35));

        }

        public override bool PreDraw(ref Color lightColor)
        {

            Main.EntitySpriteDraw(
               TextureAssets.Projectile[Type].Value,
               Projectile.position - Main.screenPosition + new Vector2(Projectile.width / 2, Projectile.height / 2),
               TextureAssets.Projectile[Type].Value.Bounds,
               lightColor,
               Projectile.rotation,
               Vector2.Zero + new Vector2(Projectile.width / 2, Projectile.height - 20),
               Projectile.scale,
               Microsoft.Xna.Framework.Graphics.SpriteEffects.None);
            return true;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            var rot = Projectile.rotation;
            var start = Projectile.Center + Projectile.velocity - new Vector2(Projectile.height / 2, 0) * rot;
            var end = Projectile.Center + Projectile.velocity + new Vector2(Projectile.height / 2, 0) * rot;
            var collisionPoint = 0f; // Don't need that variable, but required as parameter

            

            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), start, end, Projectile.width, ref collisionPoint);
        }

        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            behindNPCsAndTiles.Add(index);
        }
    }
}
