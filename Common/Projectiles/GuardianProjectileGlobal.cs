/*using GuardianClass.Content.DamageClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria;
using Microsoft.Xna.Framework;
using GuardianClass.Content.Bases;
using GuardianClass.ModPlayers;

namespace GuardianClass.Common.Projectiles
{
    public class GuardianProjectileGlobal : GlobalProjectile
    {
        public override bool AppliesToEntity(Projectile entity, bool lateInstantiation)
        {
            bool apply = false;

            if(GuardianSystem.ShieldProjectiles.Contains(entity.type))
            {
                apply = true;
            }

            return apply;
        }

        public override void PostDraw(Projectile projectile, Color lightColor)
        {
            base.PostDraw(projectile, lightColor);

            if (Main.rand.Next(60) <= 5)
            {
                Vector2 pos = projectile.position + CoolMath.PointInRect(projectile.getRect());
                pos.RotatedBy(projectile.rotation);
                int d = Dust.NewDust(pos, 1, 1, DustID.TreasureSparkle);
                Main.dust[d].noGravity = true;
                Main.dust[d].color = Color.White;
            }

        }

       
    }
}
*/