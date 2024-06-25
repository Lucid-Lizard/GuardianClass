using GuardianClass.Content.DamageClasses;
using GuardianClass.ModPlayers;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace GuardianClass.Content.Items.Warbanners
{
    public class SkeletonWarbanner : ModItem
    {
        public override void SetDefaults()
        {
            Item.useTime = 120;
            Item.useAnimation = 120;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.shoot = ModContent.ProjectileType<SkeletonWarbannerMinion>();
            Item.damage = 25;
            Item.DamageType = ModContent.GetInstance<GuardianDamage>();
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            
            var P = player.GetModPlayer<WarbannerMinionPlayer>();

            P.Clear();

            P.CreateTroops(Item.shoot, 25, 5);

            

            // Since we spawned the projectile manually already, we do not need the game to spawn it for ourselves anymore, so return false
            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.Wood, 25)
                .AddIngredient(ItemID.Silk, 30)
                .AddIngredient(ItemID.Bone, 15)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }

    public class SkeletonWarbannerMinion : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projPet[Projectile.type] = true;
            ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true;
        }

        public sealed override void SetDefaults()
        {
            Projectile.width = 26;
            Projectile.height = 36;
            Projectile.tileCollide = true; // Makes the minion go through tiles freely

            // These below are needed for a minion weapon
            Projectile.friendly = true; // Only controls if it deals damage to enemies on contact (more on that later)
            Projectile.minion = true; // Declares this as a minion (has many effects)
            Projectile.DamageType = ModContent.GetInstance<GuardianDamage>(); ; // Declares the damage type (needed for it to deal damage)
            Projectile.minionSlots = 0f; // Amount of slots this minion occupies from the total minion slots available to the player (more on that later)
            Projectile.penetrate = -1; // Needed so the minion doesn't despawn on collision with enemies or tiles
        }

        public override bool MinionContactDamage()
        {
            return true;
        }

        public int Health = 30;
        public int SwitchTargetTimer = 0;

        public enum AIState
        {
            Idle,
            Attack
        }

        public AIState state = AIState.Idle;
        int WanderTimer = 0;
        int WanderDirection = -1;
        public override void AI()
        {
            Player owner = Main.player[Projectile.owner];

            var WBP = owner.GetModPlayer<WarbannerMinionPlayer>();

            
            
            if (!CheckActive(owner))
            {
                return;
            }

            if (!WBP.GetMinionTroop(Projectile, out Troop troop))
            {
                return;
            }

            if (state == AIState.Idle)
            {
                if (troop.Attacking)
                {
                    state = AIState.Attack;
                }
                if (troop.Captain == Projectile)
                {
                    Projectile.scale = 1.2f;

                    if (Vector2.Distance(Projectile.Center, owner.Center) > 250f && WanderTimer == 0)
                    {
                        Projectile.velocity.X += 0.5f * (owner.Center.X < Projectile.Center.X ? -1 : 1);
                        Projectile.velocity.X = Math.Clamp(Projectile.velocity.X, -10, 10);
                    }
                    else
                    {
                        if (WanderTimer == 0 && Main.rand.Next(0, 100) < 2)
                        {
                            WanderTimer = Main.rand.Next(120, 180);
                            WanderDirection = Main.rand.Next(0, 10) < 5 ? -1 : 1;
                        }

                        if (WanderTimer != 0)
                        {
                            WanderTimer--;
                            Projectile.direction = WanderDirection;
                            Projectile.velocity.X += 0.2f * WanderDirection;
                            Projectile.velocity.X = Math.Clamp(Projectile.velocity.X, -5, 5);
                        }
                    }

                    List<NPC> Temp1 = troop.Targets;

                    foreach (NPC npc in Temp1)
                    {
                        if (Vector2.Distance(npc.Center, Projectile.Center) > 500f)
                        {
                            Temp1.Remove(npc);
                        }
                    }

                    troop.Targets = Temp1;

                    

                    for(int i = 0; i < Main.npc.Length; i++) 
                    {
                        NPC npc = Main.npc[i];
                        if (troop.Targets.Contains(npc))
                        {
                            return;
                        }

                        if (Vector2.Distance(npc.Center, Projectile.Center) < 500f)
                        {
                            troop.Targets.Add(npc);
                        }
                    }

                    NPC nearest = null;

                    foreach (NPC npc in troop.Targets)
                    {
                        if (nearest == null)
                        {
                            nearest = npc;
                        }
                        else
                        {
                            if (Vector2.Distance(nearest.Center, Projectile.Center) > Vector2.Distance(npc.Center, Projectile.Center))
                            {
                                nearest = npc;
                            }
                        }
                    }

                    troop.Focus = nearest;

                    troop.Attacking = troop.Focus != null;

                    

                }
                else
                {
                    if (WBP.GetTroopPos(Projectile, out int MinionPos))
                    {
                        //Projectile.position.X = troop.Captain.Center.X + (Projectile.width * MinionPos * -troop.Captain.direction) - (Projectile.width / 2);
                        Vector2 target = Vector2.Zero;
                        target.X = troop.Captain.Center.X + (Projectile.width * MinionPos * -troop.Captain.direction) - (Projectile.width / 2);
                        Vector2 direction = Projectile.Center - target;
                        Projectile.velocity.X = (Projectile.velocity.X * (60f - 1) + -direction.X) / 60f;


                    }
                }

                Projectile.velocity.Y += 0.8f;
                Projectile.velocity.X *= 0.9f;
            }
            else
            {
                
                if (troop.Captain == Projectile)
                {

                    if (Vector2.Distance(troop.Focus.Center, Projectile.Center) < 175f && WanderTimer == 0)
                    {
                        Projectile.velocity.X += 0.5f * (troop.Focus.Center.X < Projectile.Center.X ? 1 : -1);
                        Projectile.velocity.X = Math.Clamp(Projectile.velocity.X, -10, 10);
                    } else
                    {
                        if (WanderTimer == 0 && Main.rand.Next(0, 100) < 2)
                        {
                            WanderTimer = Main.rand.Next(120, 180);
                            WanderDirection = Main.rand.Next(0, 10) < 5 ? -1 : 1;
                        }

                        if (WanderTimer != 0)
                        {
                            WanderTimer--;
                            Projectile.direction = WanderDirection;
                            Projectile.velocity.X += 0.2f * WanderDirection;
                            Projectile.velocity.X = Math.Clamp(Projectile.velocity.X, -5, 5);
                        }
                    }
                } else
                {
                    Main.NewText(Projectile.ai[0]);
                    if (Projectile.ai[0]++ >= 60)
                    {
                        Vector2 vel = Projectile.Center - troop.Focus.Center;
                        vel.Normalize();
                        vel *= -7;
                        Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.position, vel, ProjectileID.WoodenArrowFriendly, 1, 1f, Projectile.owner);
                        Projectile.ai[0] = 0;
                    }

                    if (WBP.GetTroopPos(Projectile, out int MinionPos))
                    {
                        //Projectile.position.X = troop.Captain.Center.X + (Projectile.width * MinionPos * -troop.Captain.direction) - (Projectile.width / 2);
                        Vector2 target = Vector2.Zero;
                        target.X = troop.Captain.Center.X + (Projectile.width * MinionPos * -troop.Captain.direction) - (Projectile.width / 2);
                        Vector2 direction = Projectile.Center - target;
                        Projectile.velocity.X = (Projectile.velocity.X * (60f - 1) + -direction.X) / 60f;


                    }
                }

                Projectile.velocity.Y += 0.8f;
                Projectile.velocity.X *= 0.9f;

                if (!troop.Focus.active)
                {
                    
                    if (troop.Targets.Count > 0)
                    {


                        

                        troop.Attacking = false;
                    }
                    else
                    {
                        foreach (NPC npc in troop.Targets)
                        {
                            
                            
                           
                                if (Vector2.Distance(troop.Focus.Center, Projectile.Center) > Vector2.Distance(npc.Center, Projectile.Center))
                                {
                                    troop.Focus = npc;
                                }
                            
                        }
                    }

                }

                if (!troop.Attacking)
                {
                    state = AIState.Idle;
                }
            }
        }

        
            
        


       

        private bool CheckActive(Player owner)
        {
            if (owner.dead || !owner.active)
            {


                return false;
            }

            if (Health > 0)
            {
                Projectile.timeLeft = 2;
            }

            return true;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Health -= target.damage;
        }
    }
}