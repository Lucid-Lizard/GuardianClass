using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using System;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using GuardianClass.ModPlayers;

public class DurabilityBar : ModSystem
{
    public override void Load()
    {
        Terraria.On_Main.DrawInterface_14_EntityHealthBars += DurabilityBarRenderer;
    }

    private void DurabilityBarRenderer(On_Main.orig_DrawInterface_14_EntityHealthBars orig, Main self)
    {
        orig.Invoke(self);

        var Bar = (Texture2D)ModContent.Request<Texture2D>("GuardianClass/Assets/Textures/UI/DurabilityBar");
        var Color = (Texture2D)ModContent.Request<Texture2D>("GuardianClass/Assets/Textures/UI/DurabilityBar2");

        foreach (KeyValuePair<Terraria.Projectile, ShieldData> kvp in GuardianSystem.shieldData)
        {
            Projectile Projectile = kvp.Key;
            int Durability = kvp.Value.durability;
            int MaxDurability = kvp.Value.maxDurability;
            float Scale = kvp.Value.Scale;



            float Opacity = MathHelper.Lerp(0f, 1f, Scale / 1f);

            Main.EntitySpriteDraw(
                Bar,
                Projectile.Center + new Vector2(0, Projectile.height + 5) - Main.screenPosition,
                Bar.Bounds,
                Microsoft.Xna.Framework.Color.White * Opacity,
                0f,
                new Vector2(Bar.Width / 2, Bar.Height / 2),
                0.6f,
                SpriteEffects.None

                );

            float durabilityPercent = (float)Durability / (float)MaxDurability;

            Microsoft.Xna.Framework.Color flash = Microsoft.Xna.Framework.Color.White;
            if (durabilityPercent < 0.5f)
            {
                kvp.Value.Flash = true;
                float sin = MathF.Sin(kvp.Value.FlashTim / 10);
                sin /= 2;
                sin += 0.5f;
                flash = Microsoft.Xna.Framework.Color.Lerp(Microsoft.Xna.Framework.Color.White, Microsoft.Xna.Framework.Color.Gray, sin);
            }
            else
            {

                kvp.Value.FlashTim = 0;
            }



            Main.EntitySpriteDraw(
                Color,
                Projectile.Center + new Vector2(0, Projectile.height + 5) - Main.screenPosition,
                new Rectangle(0, 0, (int)MathHelper.Lerp(14, Color.Width - 14, durabilityPercent), Color.Height),
                flash * Opacity,
                0f,
                new Vector2(Bar.Width / 2, Bar.Height / 2),
                0.6f,
                SpriteEffects.None

                );
        }
    }

    private void DurabilityBarRenderer(On_Main.orig_DrawHealthBar orig, Main self, float X, float Y, int Health, int MaxHealth, float alpha, float scale, bool noFlip)
    {
        

    }
}