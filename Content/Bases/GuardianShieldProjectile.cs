using GuardianClass.ModPlayers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.DataStructures;
using Terraria;
using Terraria.ModLoader;
using System.Runtime.Serialization;
using GuardianClass.Content.Items;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using Terraria.Audio;
using Terraria.ID;
using Microsoft.Xna.Framework.Graphics;

namespace GuardianClass.Content.Bases
{
    public abstract class GuardianShieldProjectile : ModProjectile
    {
        public string TextureName;
        public virtual void BlockProjectileEffect(Projectile proj) { }
        public virtual void StrikeNPCEffect(NPC npc) { }
        public virtual void BlockNPCEffect(NPC npc) { }
        public virtual void StageChangeEffect() { }
        public virtual void ShieldBreakEffect() { }

        public virtual void DespawnEffect() { }

        public bool Override = false;

        public virtual void OverrideItemStats()
        {

        }
        public virtual void ConstantEffect() { }

        protected int Durability;

        public GuardianShield shieldItem;

        public int previousStage;
        public int currentStage;

        public bool ChangedStage = false;

        public enum ShieldState
        {
            Spawn,
            Idle,
            ThrustStart,
            ThrustEnd,
            Despawn,
            Break
        }

        public ShieldState state;

        public bool putAway = false;
        public bool putOut = true;
        public int putOutAnimTimer = 0;
        public int putAwayAnimTimer = 0;
        public float Scale = 1;

        public bool CanAttack = true;
        public int AttackTimer = 0;

        public float AttackOffset = 0;

        public override void SetDefaults()
        {
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            return false;
        }
        public override void OnSpawn(IEntitySource source)
        {
            

            state = ShieldState.Spawn;

            Player player = Main.player[Projectile.owner];
            GuardianModPlayer g = player.GetModPlayer<GuardianModPlayer>();

            shieldItem = g.currentShield;
            if(Override)
                OverrideItemStats();

            Main.NewText(Projectile.type + " " + g.LastShieldType);
            if ((Projectile.type == g.LastShieldType) && (g.LastDurability > 0))
            {
                Durability = g.LastDurability;
            }
            else
            {
                Durability = shieldItem.MaxDurability;
            }
        }

        public override bool PreAI()
        {
            previousStage = currentStage;
            Player player = Main.player[Projectile.owner];
            GuardianModPlayer g = player.GetModPlayer<GuardianModPlayer>();



            if (Main.player[Projectile.owner].channel)
            {
                Projectile.timeLeft = 60;
                if(Durability > shieldItem.MaxDurability)
                {
                    Durability = shieldItem.MaxDurability;
                }
            }
            else
            {
                state = ShieldState.Despawn;
                DespawnEffect();
                Projectile.timeLeft = 15;
            }
            if (Durability < 0)
            {
                g.LastDurability = Durability;

                Projectile.timeLeft = 0;
            }
            return true;
        }

        public override void AI()
        {
            currentStage = CalculateDurabilityStage(shieldItem.DurabilityStages, Durability, shieldItem.MaxDurability);

            ChangedStage = StageChange();

            Player player = Main.player[Projectile.owner];

            GuardianModPlayer g = player.GetModPlayer<GuardianModPlayer>();

            Projectile.direction = Projectile.velocity.X < 0 ? -1 : 1;

            //Main.NewText(Projectile.position + " " + player.position);

            switch (state)
            {
                case ShieldState.Spawn:
                    {
                        if (putOutAnimTimer++ < shieldItem.spawnTime)
                        {
                            Scale = MathHelper.Lerp(0f, 1f, CoolMath.EaseInOutBack((float)putOutAnimTimer / 15f));
                            AttackOffset = MathHelper.Lerp(-shieldItem.idleDistance, 0f, CoolMath.EaseInOutBack((float)putOutAnimTimer / 15f));
                        }
                        else
                        {
                            state = ShieldState.Idle;
                            putOutAnimTimer = 0;
                        }
                        break;
                    }
                case ShieldState.Idle:
                    {
                        if (Mouse.GetState().RightButton == ButtonState.Pressed && CanAttack)
                        {
                            AttackTimer = 0;
                            state = ShieldState.ThrustStart;
                            CanAttack = false;
                            SoundEngine.PlaySound(new SoundStyle("GuardianClass/Sounds/GuardianSounds_WoodenShield3"), Projectile.Center);
                        }
                        break;
                    }
                case ShieldState.ThrustStart:
                    {
                        if (AttackTimer++ < shieldItem.thrustTime)
                        {
                            AttackOffset = MathHelper.Lerp(0f, shieldItem.attackDistance, CoolMath.EaseInOutBack((float)AttackTimer / (float)(shieldItem.thrustTime - 1)));
                        } else
                        {
                            AttackTimer = 0;
                            state = ShieldState.ThrustEnd;
                        }
                        break;
                    }
                case ShieldState.ThrustEnd:
                    {
                        if (AttackTimer++ < shieldItem.thrustTime)
                        {
                            AttackOffset = MathHelper.Lerp(shieldItem.attackDistance, 0f, CoolMath.EaseOutBack(((float)AttackTimer) / (float)(shieldItem.thrustTime - 1)));
                        }
                        else
                        {
                            CanAttack = true;
                            AttackTimer = 0;
                            state = ShieldState.Idle;
                        }
                        break;
                    }
                case ShieldState.Despawn:
                    {
                        if (putAwayAnimTimer++ < 16)
                        {
                            Scale = MathHelper.Lerp(1f, 0f, CoolMath.EaseInOutBack((float)putAwayAnimTimer / 15f));
                            AttackOffset = MathHelper.Lerp(0f, -shieldItem.idleDistance, CoolMath.EaseInOutBack((float)putAwayAnimTimer / 15f));
                        }
                        else
                        {
                            putOut = false;
                            putAwayAnimTimer = 0;
                            Projectile.Kill();
                        }
                        break;
                    }
                case ShieldState.Break:
                    {
                        g.LastDurability = -1;
                        Projectile.Kill();
                        break;
                    }
            }

            Vector2 playerCenter = player.Center - new Vector2(Projectile.width / 2, 0) - new Vector2(0, Projectile.height / 2);

            Projectile.position = playerCenter;

            Vector2 angleVec = Vector2.Normalize(Main.MouseWorld - playerCenter);
            Projectile.velocity = (shieldItem.idleDistance + AttackOffset) * angleVec;
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;

            List<Projectile> Projlist = CheckForIntersectionsProjectiles(Projectile);            
            foreach (Projectile projectile in Projlist)
            {
                Durability -= projectile.damage;
                BlockProjectileEffect(projectile);
            }

            List<NPC> NPClist = CheckForIntersectionsNPCs(Projectile);
            foreach (NPC npc in NPClist)
            {
                if (npc.immune[Projectile.owner] == 0)
                {
                    Durability -= (int)(npc.damage * shieldItem.Resistance);
                    g.LastDurability = Durability;
                    if (state == ShieldState.ThrustStart && npc.immune[Projectile.owner] == 0)
                    {                      
                        npc.StrikeNPC(npc.CalculateHitInfo(Projectile.damage, Projectile.direction, false, Projectile.knockBack));
                        StrikeNPCEffect(npc);
                    }
                    else
                    {
                        if (npc.immune[Projectile.owner] == 0)
                        {
                            npc.SimpleStrikeNPC(0, Projectile.direction, false, knockBack: 2);
                            BlockNPCEffect(npc);
                        }
                    }
                    npc.immune[Projectile.owner] = 20;
                }
            }

            if ((Projlist.Count + NPClist.Count) > 0 && ChangedStage)
            {
                StageChangeEffect();
                //SoundEngine.PlaySound(new SoundStyle("GuardianClass/Sounds/GuardianSounds_WoodenShield1"), Projectile.Center);
            }

            g.LastDurability = Durability;

            if (ChangedStage)
            {
                StageChangeEffect();
            }

            ConstantEffect();
        }

        public Vector3 CalculateRotPos(float dist)
        {
            Player player = Main.player[Projectile.owner];

            Vector2 playerCenter = player.Center - new Vector2(Projectile.width / 2, 0) - new Vector2(0, Projectile.height / 2);

            Vector2 position = playerCenter;

            Vector2 angleVec = Vector2.Normalize(Main.MouseWorld - playerCenter);
            Vector2 velocity = (shieldItem.idleDistance + AttackOffset + dist) * angleVec;
            float rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;

            return new Vector3(position.X + velocity.X, position.Y + velocity.Y, rotation);
        }

        public virtual void DrawExtraOverShield()
        {

        }


        public override void PostAI()
        {
            ChangedStage = false;
            Override = false;

            if (Durability <= 0)
            {
               state = ShieldState.Break;
            }
        }
        public override void OnKill(int timeLeft)
        {
            //Main.NewText(Durability);
            Player player = Main.player[Projectile.owner];
            GuardianModPlayer g = player.GetModPlayer<GuardianModPlayer>();
            g.LastShieldType = Projectile.type;
            switch (state)
            {
                case ShieldState.Despawn:
                    {
                        SoundEngine.PlaySound(SoundID.Item1, Projectile.Center);
                        g.LastDurability = Durability;
                        break;
                    }

                case ShieldState.Break:
                    {
                        ShieldBreakEffect();
                        //SoundEngine.PlaySound(new SoundStyle("GuardianClass/Sounds/GuardianSounds_WoodenShield2"), Projectile.Center);
                        g.LastDurability = -1;
                        break;
                    }
            
            }
                
           
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {

            Main.NewText(target.immune[Projectile.owner]);
        }

        public bool ShieldCol(Rectangle projHitbox, Rectangle targetHitbox)
        {
            Vector2 rot = Projectile.velocity;
            rot.Normalize();
            Vector2 start = (Projectile.Center + Projectile.velocity) - (new Vector2(Projectile.height / 2, 0 ) * rot);
            Vector2 end = (Projectile.Center + Projectile.velocity) + (new Vector2(Projectile.height / 2, 0) * rot);
            float collisionPoint = 0f; // Don't need that variable, but required as parameter
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), start, end, Projectile.width, ref collisionPoint);
        }

        public List<Projectile> CheckForIntersectionsProjectiles(Projectile Proj)
        {
            List<Projectile> projectiles = new List<Projectile>();
            foreach (Projectile p in Main.ActiveProjectiles)
            {
                if (ShieldCol(Proj.Hitbox, p.Hitbox) && p.hostile == true && p != Proj)
                {
                    projectiles.Add(p);
                }
            }
            return projectiles;
        }

        public List<NPC> CheckForIntersectionsNPCs(Projectile Proj)
        {
            List<NPC> NPCs = new List<NPC>();
            foreach (NPC n in Main.ActiveNPCs)
            {
                if (ShieldCol(Proj.Hitbox, n.Hitbox) && n.friendly == false)
                {

                    NPCs.Add(n);
                }
            }
            return NPCs;
        }

        public bool StageChange()
        {

            return (currentStage != previousStage) && (state != ShieldState.Spawn);
        }

        public int CalculateDurabilityStage(int Stages, int Durability, int MaxDurability)
        {

            float progress = (float)Durability / (float)MaxDurability;
            progress = 1 - progress;
            float num = Stages * progress;

            for (int i = 0; i < Stages; i++)
            {
                if (i == num)
                {
                    //Main.NewText(num + " NUM" + i);
                    return i;
                }

                if (num > i && num < i + 1)
                {
                    //Main.NewText(num + " NUM" + i);
                    return i;
                }


            }

            return -1;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = (Texture2D)ModContent.Request<Texture2D>("GuardianClass/Content/Items/" + TextureName); // Get the projectile's texture

            
            
            Vector2 position = Projectile.Center; 


            bool shouldflip = Projectile.velocity.X < 0;
            Projectile.direction = shouldflip ? -1 : 1;
            
            Main.EntitySpriteDraw(texture, position - Main.screenPosition, new Rectangle(0, 0 + ((Projectile.height + 2) * currentStage), Projectile.width, Projectile.height), lightColor, Projectile.rotation, new Vector2(Projectile.width, Projectile.height) / 2f, Scale, shouldflip ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0f);

            DrawExtraOverShield();

            return false;
        }

        public override void PostDraw(Color lightColor)
        {
            DrawExtraOverShield();
        }
        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            overPlayers.Add(index);
        }
    }
}
