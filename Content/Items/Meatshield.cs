using GuardianClass.Content.Bases;
using GuardianClass.ModPlayers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace GuardianClass.Content.Items;

public class Meatshield : GuardianShield
{
    public override void SetGuardianDefaults() {
        ShieldProjectile = ModContent.ProjectileType<MeatshieldProjectile>();
        MaxDurability = 125;

        spawnTime = 15;
        thrustTime = 25;

        DurabilityStages = 1;


        Resistance = 0.25f;

        attackDistance = 25;
        idleDistance = 25;

        Item.damage = 36;
    }
}

public class MeatshieldProjectile : GuardianShieldProjectile
{
    public bool canStick = true;
    public int NPCPreviousDamage;
    public int StickedNPC = -1;

    public Texture2D text = TextureAssets.Item[1].Value;

    public override void SetDefaults() {
        TextureName = "MeatshieldProjectile";
        Projectile.knockBack = 6;

        Projectile.width = 40;
        Projectile.height = 38;
    }


    public override void BlockNPCEffect(NPC npc) {
        if (StickedNPC >= 0) {
            if (Durability <= 0) {
                var vel = Projectile.velocity;
                vel.Normalize();
                vel *= 5;

                Main.npc[StickedNPC].velocity = vel;
                Main.npc[StickedNPC].StrikeInstantKill();
            }
        }
        //SoundEngine.PlaySound(new SoundStyle("GuardianClass/Sounds/GuardianSounds_WoodenShieldNPCBlock"));
    }

    public override void StrikeNPCEffect(NPC npc) {
        if (StickedNPC < 0
            && canStick
            && npc.type != NPCID.TargetDummy
            && !Main.npc[npc.whoAmI].boss
            && npc.type != NPCID.DungeonGuardian
            && Main.npc[npc.whoAmI].life > 0) {
            StickedNPC = npc.whoAmI;
            Durability = npc.life;
            canStick = false;
            NPCPreviousDamage = npc.damage;
            SoundEngine.PlaySound(new SoundStyle("GuardianClass/Sounds/GuardianSounds_MeatshieldImpale"));
        }
        else {
            SoundEngine.PlaySound(new SoundStyle("GuardianClass/Sounds/GuardianSounds_WoodenShieldNPCBlock"));
        }
        //npc.AddBuff(BuffID.Confused, 120);
    }

    public override void BlockProjectileEffect(Projectile proj) {
        SoundEngine.PlaySound(new SoundStyle("GuardianClass/Sounds/GuardianSounds_ArrowWoodImpact"));
        proj.velocity *= -1;
        proj.friendly = true;
        proj.hostile = false;
    }

    public override void StageChangeEffect() {
        SoundEngine.PlaySound(new SoundStyle("GuardianClass/Sounds/GuardianSounds_WoodenShield2"), Projectile.Center);
        for (var i = 0; i < 3; i++) {
            Dust.NewDust(Projectile.Center, 4, 4, DustID.WoodFurniture);
        }
    }

    public override void ShieldBreakEffect() {
        SoundEngine.PlaySound(new SoundStyle("GuardianClass/Sounds/GuardianSounds_WoodenShield1"), Projectile.Center);
        for (var i = 0; i < 7; i++) {
            Dust.NewDust(Projectile.Center, 4, 4, DustID.WoodFurniture);
        }

        if (StickedNPC >= 0) {
            Main.npc[StickedNPC].StrikeInstantKill();
        }
    }

    public override void DespawnEffect() {
        if (StickedNPC >= 0) {
            var vel = Projectile.velocity;
            vel.Normalize();
            vel *= 5;
            Main.npc[StickedNPC].velocity = vel;
            Main.npc[StickedNPC].damage = NPCPreviousDamage;
            Main.npc[StickedNPC].hide = false;
            StickedNPC = -1;
        }
    }

    public override void ConstantEffect() {
        var player = Main.player[Projectile.owner];
        var g = player.GetModPlayer<GuardianModPlayer>();
        if (StickedNPC >= 0) {
            //Main.NewText(StickedNPC);
            Override = true;
            var npc = Main.npc[StickedNPC];
            npc.Center = Projectile.Center + Projectile.velocity;

            npc.hide = true;

            npc.damage = 0;

            npc.life = Durability;

            npc.immune[Projectile.owner] = 20;

            // HERE


            if (Durability <= 0) {
                shieldItem = g.currentShield;
                Durability = shieldItem.MaxDurability;


                Override = false;

                StickedNPC = -1;
            }
        }
    }

    public override void PostDraw(Color lightColor) {
        base.PostDraw(lightColor);
        // Main.NewText(Override);
        if (StickedNPC >= 0) {
            var shouldflip = Projectile.velocity.X < 0;

            var npc = Main.npc[StickedNPC];
            var width = npc.width;
            var height = npc.height;
            var rect = npc.frame;
            Main.NewText(width + " " + height + " " + text);
            text = TextureAssets.Npc[npc.type].Value;

            Vector2 pos;
            Vector2 vel;

            var player = Main.player[Projectile.owner];

            var playerCenter = player.Center - new Vector2(Projectile.width / 2, 0) - new Vector2(0, Projectile.height / 2);

            pos = playerCenter;

            var angleVec = Vector2.Normalize(Main.MouseWorld - playerCenter);
            vel = (shieldItem.idleDistance + AttackOffset + 10) * angleVec;
            var rot = Projectile.velocity.ToRotation();

            pos += vel;

            Main.EntitySpriteDraw(
                text,
                Projectile.Center - Main.screenPosition,
                rect,
                lightColor,
                rot - (shouldflip ? MathHelper.Pi : 0),
                new Vector2(rect.Width / 2 + 7, rect.Height / 2),
                1f,
                shouldflip ? SpriteEffects.FlipVertically & SpriteEffects.FlipHorizontally : SpriteEffects.FlipHorizontally
            );

            var texture = (Texture2D)ModContent.Request<Texture2D>("GuardianClass/Content/Items/" + TextureName); // Get the projectile's texture

            var position = Projectile.Center;

            Main.EntitySpriteDraw(
                texture,
                position - Main.screenPosition,
                new Rectangle(0, 0 + (Projectile.height + 2) * 1, Projectile.width, Projectile.height),
                lightColor,
                Projectile.rotation,
                new Vector2(Projectile.width, Projectile.height) / 2f,
                Scale,
                shouldflip ? SpriteEffects.FlipHorizontally : SpriteEffects.None
            );
        }
    }

    public override void OverrideItemStats() {
        var npc = Main.npc[StickedNPC];
        shieldItem.Resistance = npc.defense / 100f;
        shieldItem.MaxDurability = npc.lifeMax;
    }
}
