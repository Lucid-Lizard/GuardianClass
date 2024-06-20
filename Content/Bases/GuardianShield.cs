using GuardianClass.Content.Items;
using GuardianClass.ModPlayers;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace GuardianClass.Content.Bases
{
    public abstract class GuardianShield : ModItem
    {
        public int ShieldProjectile;

        public int MaxDurability = 100;

        public int spawnTime;
        public int thrustTime;
        
        public int DurabilityStages = 3;
        public float Resistance = 0.25f;

        public float attackDistance;
        public float idleDistance = 25;

        

        public virtual void SetGuardianDefaults()
        {

        }

        public override void SetStaticDefaults()
        {
            GuardianSystem.GuardianShieldItems.Add(Item.type);

        }

        public override void SetDefaults()
        {
            Item.damage = 24;
            Item.DamageType = DamageClass.Melee;
            Item.width = 40;
            Item.height = 40;
            Item.useTime = 120;
            Item.useAnimation = 1;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noUseGraphic = true;
            Item.knockBack = 6;
            Item.value = Item.buyPrice(silver: 1);
            Item.rare = ItemRarityID.Blue;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = false;
            Item.channel = true;
            SetGuardianDefaults();
        }
        public override bool? UseItem(Player player)
        {
            if (player.ownedProjectileCounts[ShieldProjectile] == 0)
            {
                Projectile.NewProjectile(player.GetSource_FromThis(), player.position, Vector2.Zero, ShieldProjectile, Item.damage, 0);
            }
            return base.UseItem(player);
        }
    }
}
