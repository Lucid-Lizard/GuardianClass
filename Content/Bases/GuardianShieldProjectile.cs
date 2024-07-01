using System;
using System.Collections.Generic;
using GuardianClass.Content.DamageClasses;
using GuardianClass.ModPlayers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ReLogic.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace GuardianClass.Content.Bases;

public abstract class GuardianShieldProjectile : ModProjectile
{
    public enum ShieldState
    {
        Spawn,
        Idle,
        ThrustStart,
        ThrustEnd,
        Despawn,
        Break
    }

    public float AttackOffset;
    public int AttackTimer;

    public bool BlockFXFlag;
    public int BlockFXTimer;

    public bool CanAttack = true;

    public bool ChangedStage;
    public int currentStage;

    protected int Durability;

    public bool Override;

    public int previousStage;

    public bool putAway = false;
    public int putAwayAnimTimer;
    public bool putOut = true;
    public int putOutAnimTimer;
    public float Scale = 1;

    public GuardianShield shieldItem;

    public ShieldState state;
    public string TextureName;
    public virtual void BlockProjectileEffect(Projectile proj) { }
    public virtual void StrikeNPCEffect(NPC npc) { }
    public virtual void BlockNPCEffect(NPC npc) { }
    public virtual void StageChangeEffect() { }
    public virtual void ShieldBreakEffect() { }

    public virtual void DespawnEffect() { }

    public virtual void OverrideItemStats() { }

    public virtual void ConstantEffect() { }

    public override void SetStaticDefaults() {
        ProjectileID.Sets.TrailCacheLength[Projectile.type] = 12; // The length of old position to be recorded
        ProjectileID.Sets.TrailingMode[Projectile.type] = 0; // The recording mode
        GuardianSystem.ShieldProjectiles.Add(Type);
    }

    public override void SetDefaults() {
        Projectile.tileCollide = false;
        Projectile.penetrate = -1;
        Projectile.DamageType = ModContent.GetInstance<GuardianDamage>();
    }

    public override bool OnTileCollide(Vector2 oldVelocity) {
        return false;
    }

    public override void OnSpawn(IEntitySource source) {
        state = ShieldState.Spawn;

        var player = Main.player[Projectile.owner];
        var g = player.GetModPlayer<GuardianModPlayer>();

        shieldItem = g.CurrentShield;
        if (Override) {
            OverrideItemStats();
        }

        Main.NewText(Projectile.type + " " + g.LastShieldType);
        if (Projectile.type == g.LastShieldType && g.LastDurability > 0) {
            Durability = g.LastDurability;
        }
        else {
            Durability = shieldItem.MaxDurability;
        }

        GuardianSystem.shieldData.Add(Projectile, new ShieldData(1,1));
        
    }

    public override bool PreAI() {
        previousStage = currentStage;
        var player = Main.player[Projectile.owner];
        var g = player.GetModPlayer<GuardianModPlayer>();


        if (Main.player[Projectile.owner].channel) {
            Projectile.timeLeft = 60;
            if (Durability > shieldItem.MaxDurability) {
                Durability = shieldItem.MaxDurability;
            }
        }
        else {
            state = ShieldState.Despawn;
            DespawnEffect();
            Projectile.timeLeft = 15;
        }

        if (Durability < 0) {
            g.LastDurability = Durability;

            Projectile.timeLeft = 0;
        }

        return true;
    }

    public override void AI() {

        GuardianSystem.shieldData[Projectile].durability = Durability;
        GuardianSystem.shieldData[Projectile].maxDurability = shieldItem.MaxDurability;
        GuardianSystem.shieldData[Projectile].Scale = Scale;

        if (GuardianSystem.shieldData[Projectile].Flash)
        {
            GuardianSystem.shieldData[Projectile].FlashTim++;
        }
        Projectile.netImportant = true;
        currentStage = CalculateDurabilityStage(shieldItem.DurabilityStages, Durability, shieldItem.MaxDurability);

        ChangedStage = StageChange();

        var player = Main.player[Projectile.owner];

        var g = player.GetModPlayer<GuardianModPlayer>();

        Projectile.direction = Projectile.velocity.X < 0 ? -1 : 1;

        //Main.NewText(Projectile.position + " " + player.position);

        switch (state) {
            case ShieldState.Spawn: {
                if (putOutAnimTimer++ < shieldItem.SpawnTime) {
                    Scale = MathHelper.Lerp(0f, 1f, CoolMath.EaseInOutBack(putOutAnimTimer / 15f));
                    AttackOffset = MathHelper.Lerp(-shieldItem.IdleDistance, 0f, CoolMath.EaseInOutBack(putOutAnimTimer / 15f));
                }
                else {
                    state = ShieldState.Idle;
                    putOutAnimTimer = 0;
                }

                break;
            }
            case ShieldState.Idle: {
                Array.Clear(Projectile.oldPos);
                if (Mouse.GetState().RightButton == ButtonState.Pressed && CanAttack) {
                    AttackTimer = 0;
                    state = ShieldState.ThrustStart;
                    CanAttack = false;
                    SoundEngine.PlaySound(new SoundStyle("GuardianClass/Assets/Sounds/GuardianSounds_WoodenShield3"), Projectile.Center);
                }

                break;
            }
            case ShieldState.ThrustStart: {
                if (AttackTimer++ < shieldItem.ThrustTime) {
                    AttackOffset = MathHelper.Lerp(
                        0f,
                        shieldItem.AttackDistance,
                        CoolMath.EaseInOutBack(AttackTimer / (float)(shieldItem.ThrustTime - 1))
                    );
                }
                else {
                    AttackTimer = 0;
                    state = ShieldState.ThrustEnd;
                }

                break;
            }
            case ShieldState.ThrustEnd: {
                if (AttackTimer++ < shieldItem.ThrustTime) {
                    AttackOffset = MathHelper.Lerp(
                        shieldItem.AttackDistance,
                        0f,
                        CoolMath.EaseOutBack(AttackTimer / (float)(shieldItem.ThrustTime - 1))
                    );
                }
                else {
                    CanAttack = true;
                    AttackTimer = 0;
                    state = ShieldState.Idle;
                }

                break;
            }
            case ShieldState.Despawn: {
                if (putAwayAnimTimer++ < 16) {
                    Scale = MathHelper.Lerp(1f, 0f, CoolMath.EaseInOutBack(putAwayAnimTimer / 15f));
                    AttackOffset = MathHelper.Lerp(0f, -shieldItem.IdleDistance, CoolMath.EaseInOutBack(putAwayAnimTimer / 15f));
                }
                else {
                    putOut = false;
                    putAwayAnimTimer = 0;
                    Projectile.Kill();
                }

                break;
            }
            case ShieldState.Break: {
                g.LastDurability = -1;
                Projectile.Kill();
                break;
            }
        }

        var playerCenter = player.Center - new Vector2(Projectile.width / 2, 0) - new Vector2(0, Projectile.height / 2);

        Projectile.position = playerCenter;

        var angleVec = Vector2.Normalize(Main.MouseWorld - playerCenter);
        Projectile.velocity = (shieldItem.IdleDistance + AttackOffset) * angleVec;
        Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;

        var Projlist = CheckForIntersectionsProjectiles(Projectile);
        foreach (var projectile in Projlist) {
            Durability -= projectile.damage;
            BlockProjectileEffect(projectile);


        }

        var NPClist = CheckForIntersectionsNPCs(Projectile);
        foreach (var npc in NPClist) {
            if (npc.immune[Projectile.owner] == 0) {
                Durability -= (int)(npc.damage * shieldItem.Resistance);
                g.LastDurability = Durability;
                if (state == ShieldState.ThrustStart && npc.immune[Projectile.owner] == 0) {
                    npc.StrikeNPC(npc.CalculateHitInfo(Projectile.damage, Projectile.direction, false, Projectile.knockBack));

                    StrikeNPCEffect(npc);
                }
                else {
                    if (npc.immune[Projectile.owner] == 0) {
                        if (npc.immune[Projectile.owner] == 0) {
                            BlockFXFlag = true;
                            BlockFXTimer = 45;
                            BlockNPCEffect(npc);
                        }
                    }
                }

                npc.immune[Projectile.owner] = 20;
            }

            Vector2 vel = Projectile.velocity;
            Vector2 npcVel = npc.velocity;
            vel.Normalize();
            vel *= npcVel.Length() + player.velocity.Length() * 2f;
            // Calculate the direction vector from the NPC to the projectile's center
            Vector2 dir = Projectile.Center - npc.Center;

            // Normalize the direction vector to get a unit vector
            dir.Normalize();

            // Calculate the dot product of the direction vector and the NPC's velocity
            float dotProduct = Vector2.Dot(dir, npcVel);

            // Define a threshold for considering the velocities aligned
            float threshold = 0.9f; // Adjust this value based on your needs

            if (dotProduct > threshold)
            {
                // Modify the NPC's velocity to match the projectile's velocity
                npc.velocity = vel;
            }

        }

        if (Projlist.Count + NPClist.Count > 0 && ChangedStage) {
            StageChangeEffect();
            //SoundEngine.PlaySound(new SoundStyle("GuardianClass/Assets/Sounds/GuardianSounds_WoodenShield1"), Projectile.Center);
        }

        g.LastDurability = Durability;

        if (ChangedStage) {
            StageChangeEffect();
        }

        ConstantEffect();
    }

    /*private void BlockProjectileAccessoryFX(Projectile projectile)
    {
        var player = Main.player[Projectile.owner];
        if(player.TryGetModPlayer<GuardianModPlayer>(out var g))
        {
            if (g.ShieldPolish)
            {
                projectile.velocity *= -1;
                projectile.friendly = true;
                projectile.hostile = false;
            }
        }


        

        
    }*/

    public Vector3 CalculateRotPos(float dist) {
        var player = Main.player[Projectile.owner];

        var playerCenter = player.Center - new Vector2(Projectile.width / 2, 0) - new Vector2(0, Projectile.height / 2);

        var position = playerCenter;

        var angleVec = Vector2.Normalize(Main.MouseWorld - playerCenter);
        var velocity = (shieldItem.IdleDistance + AttackOffset + dist) * angleVec;
        var rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;

        return new Vector3(position.X + velocity.X, position.Y + velocity.Y, rotation);
    }

    public virtual void DrawExtraOverShield() { }


    public override void PostAI() {
        ChangedStage = false;
        Override = false;

        if (Durability <= 0) {
            state = ShieldState.Break;
        }

        if (BlockFXFlag && BlockFXTimer-- == 0) {
            BlockFXTimer = 0;
            BlockFXFlag = false;
        }
    }

    public override void OnKill(int timeLeft) {
        //Main.NewText(Durability);
        var player = Main.player[Projectile.owner];
        var g = player.GetModPlayer<GuardianModPlayer>();
        g.LastShieldType = Projectile.type;
        switch (state) {
            case ShieldState.Despawn: {
                SoundEngine.PlaySound(SoundID.Item1, Projectile.Center);
                g.LastDurability = Durability;
                break;
            }

            case ShieldState.Break: {
                ShieldBreakEffect();
                //SoundEngine.PlaySound(new SoundStyle("GuardianClass/Assets/Sounds/GuardianSounds_WoodenShield2"), Projectile.Center);
                g.LastDurability = -1;
                break;
            }
        }

        GuardianSystem.shieldData.Remove(Projectile);
        
    }

    public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) {
        return false;
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) {
        Main.NewText(target.immune[Projectile.owner]);
    }

    public bool ShieldCol(Rectangle projHitbox, Rectangle targetHitbox) {
        var rot = Projectile.velocity;
        rot.Normalize();
        var start = Projectile.Center + Projectile.velocity - new Vector2(Projectile.height / 2, 0) * rot;
        var end = Projectile.Center + Projectile.velocity + new Vector2(Projectile.height / 2, 0) * rot;
        var collisionPoint = 0f; // Don't need that variable, but required as parameter
        return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), start, end, Projectile.width, ref collisionPoint);
    }

    public List<Projectile> CheckForIntersectionsProjectiles(Projectile Proj) {
        var projectiles = new List<Projectile>();
        foreach (var p in Main.ActiveProjectiles) {
            if (ShieldCol(Proj.Hitbox, p.Hitbox) && p.hostile && p != Proj) {
                projectiles.Add(p);
            }
        }

        return projectiles;
    }

    public List<NPC> CheckForIntersectionsNPCs(Projectile Proj) {
        var NPCs = new List<NPC>();
        foreach (var n in Main.ActiveNPCs) {
            if (ShieldCol(Proj.Hitbox, n.Hitbox) && n.friendly == false) {
                NPCs.Add(n);
            }
        }

        return NPCs;
    }

    public bool StageChange() {
        return currentStage != previousStage && state != ShieldState.Spawn;
    }

    public int CalculateDurabilityStage(int Stages, int Durability, int MaxDurability) {
        var progress = Durability / (float)MaxDurability;
        progress = 1 - progress;
        var num = Stages * progress;

        for (var i = 0; i < Stages; i++) {
            if (i == num) {
                //Main.NewText(num + " NUM" + i);
                return i;
            }

            if (num > i && num < i + 1) {
                //Main.NewText(num + " NUM" + i);
                return i;
            }
        }

        return -1;
    }

    public override bool PreDraw(ref Color lightColor) {
        var texture = (Texture2D)ModContent.Request<Texture2D>("GuardianClass/Content/Items/" + TextureName); // Get the projectile's texture


        var position = Projectile.Center;


        var shouldflip = Projectile.velocity.X < 0;
        Projectile.direction = shouldflip ? -1 : 1;

        if (state == ShieldState.ThrustStart) {
            for (var i = 0; i < 12; i++) {
                Main.EntitySpriteDraw(
                    texture,
                    Projectile.oldPos[i] + new Vector2(Projectile.width / 2, Projectile.height / 2) - Main.screenPosition,
                    new Rectangle(0, 0 + (Projectile.height + 2) * currentStage, Projectile.width, Projectile.height),
                    lightColor * MathHelper.Lerp(0.5f, 0f, i / 12f),
                    Projectile.rotation,
                    new Vector2(Projectile.width, Projectile.height) / 2f,
                    MathHelper.Lerp(Scale + 0.1f, 0, i / 12f),
                    shouldflip ? SpriteEffects.FlipHorizontally : SpriteEffects.None
                );
            }
        }

        Main.EntitySpriteDraw(
            texture,
            position - Main.screenPosition,
            new Rectangle(0, 0 + (Projectile.height + 2) * currentStage, Projectile.width, Projectile.height),
            lightColor,
            Projectile.rotation,
            new Vector2(Projectile.width, Projectile.height) / 2f,
            Scale,
            shouldflip ? SpriteEffects.FlipHorizontally : SpriteEffects.None
        );


        DrawExtraOverShield();


        return false;
    }
    private float FlashTim = 0;
    public override void PostDraw(Color lightColor) {
        if (BlockFXFlag) {
            var texture = (Texture2D)ModContent.Request<Texture2D>("GuardianClass/Content/Items/" + TextureName); // Get the projectile's texture


            var position = Projectile.Center;


            var shouldflip = Projectile.velocity.X < 0;
            Projectile.direction = shouldflip ? -1 : 1;


            Main.EntitySpriteDraw(
                texture,
                position - Main.screenPosition,
                new Rectangle(0, 0 + (Projectile.height + 2) * currentStage, Projectile.width, Projectile.height),
                Microsoft.Xna.Framework.Color.White * MathHelper.Lerp(1f, 0f * 1.5f, 1 - CoolMath.EaseInCirc(BlockFXTimer / (float)45)),
                Projectile.rotation,
                new Vector2(Projectile.width, Projectile.height) / 2f,
                MathHelper.Lerp(Scale, Scale * 1.75f, 1 - CoolMath.EaseInCirc(BlockFXTimer / (float)45)),
                shouldflip ? SpriteEffects.FlipHorizontally : SpriteEffects.None
            );
        }

        DrawExtraOverShield();


        //Main.spriteBatch.Begin();
       
        // Main.spriteBatch.End();
    }

    

    public override void DrawBehind(
        int index,
        List<int> behindNPCsAndTiles,
        List<int> behindNPCs,
        List<int> behindProjectiles,
        List<int> overPlayers,
        List<int> overWiresUI
    ) {
        overPlayers.Add(index);
    }

    
}
