using GuardianClass.Content.DamageClasses;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace GuardianClass.Content.Projectiles
{
    public class MaceTileProjectile : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.damage = 6;
            Projectile.DamageType = ModContent.GetInstance<GuardianDamage>();
            Projectile.tileCollide = false;
            Projectile.penetrate = 2;
            Projectile.aiStyle = -1;
            Projectile.friendly = true;
            Projectile.hostile = false;
        }

        public override void AI()
        {
            Projectile.velocity.Y += 0.2f;
            Projectile.velocity.X *= 0.98f;
            Projectile.rotation += MathHelper.Pi / 16;

            /*if(Main.rand.Next(0,100) <= 10)
            {
                Main.NewText(Projectile.ai[0]);
                Main.NewText(TileLoader.GetTile((int)Projectile.ai[0]));
                Dust.NewDust(Projectile.Center, 
                    16, 
                    16,
                    TileLoader.GetTile((int)Projectile.ai[0]) != null ? TileLoader.GetTile((int)Projectile.ai[0]).DustType : 0);
            }*/

            

            if (Projectile.ai[1]++ >= 20)
            {
                Projectile.tileCollide = true;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D Texture = TextureAssets.Tile[(int)Projectile.ai[0]].Value;

            Vector2 texpos = new Vector2(9, 3);
            texpos *= 18;
            Rectangle texrect = new Rectangle((int)texpos.X, (int)texpos.Y, 16, 16);

            Main.EntitySpriteDraw(
                Texture,
                Projectile.position + new Vector2(Projectile.width / 2, Projectile.height / 2) - Main.screenPosition,
                texrect,
                lightColor,
                Projectile.rotation,
                 new Vector2(Projectile.width / 2, Projectile.height / 2),
                Projectile.scale,
                SpriteEffects.None);

            
            return false;
        }
    }
}
