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

            DurabilityStages = 3;
            Resistance = 0.23f;

            AttackDistance = 26;
            IdleDistance = 27;
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.damage = 53;
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemID.HellstoneBar, 18);
            recipe.AddIngredient(ItemID.Obsidian, 8);
            recipe.AddTile(TileID.Anvils);
            recipe.Register();
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            var h = $"[i:{ModContent.ItemType<ShieldHeart>()}]";

            var tooltip = tooltips.FirstOrDefault(line => line.Name == "Tooltip0" && line.Mod == "Terraria");
            var Tip = new TooltipLine(this.Mod, "GuardianTooltip", "");

            Tip.Text = $"Gives all players {h}{h} when you block a projectile\nBlocked projectiles turn into Hellfire Arrows and are reflected back at enemies";
            tooltips.Insert(tooltips.Count - 1, Tip);
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

            wooshfx = "GuardianClass/Assets/Sounds/GuardianSounds_HeaterWoosh";
        }

        public override void BlockNPCEffect(NPC npc)
        {

            npc.AddBuff(BuffID.OnFire, 120);
            SoundEngine.PlaySound(new SoundStyle("GuardianClass/Assets/Sounds/GuardianSounds_HeaterSizzle").WithVolumeScale(0.5f), Projectile.Center);

            Gore.NewGore(Projectile.GetSource_FromAI(), Projectile.Center + Projectile.velocity, new Vector2(0, -1), GoreID.Smoke1);
        }

        public override void StrikeNPCEffect(NPC npc)
        {
            SoundEngine.PlaySound(new SoundStyle("GuardianClass/Assets/Sounds/GuardianSounds_HeaterSizzle"));
            //npc.AddBuff(BuffID.Confused, 120);
            npc.AddBuff(BuffID.OnFire, 120);
        }

        public override void BlockProjectileEffect(Projectile proj)
        {
            SoundEngine.PlaySound(new SoundStyle("GuardianClass/Assets/Sounds/GuardianSounds_HeaterSizzle"));

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
                if (Main.player[i].TryGetModPlayer<GuardianModPlayer>(out var mod))
                    mod.AddWard(2);
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

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.PointClamp, null, null, null, Main.GameViewMatrix.ZoomMatrix);


            float opacity = (float)((CalculateDurabilityStage(3, Durability, shieldItem.MaxDurability)+ 1)/ (float)shieldItem.DurabilityStages);
            opacity = 1 - opacity;
           

            Main.EntitySpriteDraw(texture, position - Main.screenPosition, new Rectangle(0, 0 + ((Projectile.height + 2) * 3), Projectile.width, Projectile.height), Color.Yellow * opacity, Projectile.rotation, new Vector2(Projectile.width, Projectile.height) / 2f, Scale + (0.1f * MathF.Sin(Projectile.ai[0] / 100)), shouldflip ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0f);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp, null, null, null, Main.GameViewMatrix.ZoomMatrix);

            base.PostDraw(lightColor);
        }

    }
}
