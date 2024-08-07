﻿using System;
using System.Collections.Generic;
using GuardianClass.Content.Bases;
using GuardianClass.Content.DamageClasses;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace GuardianClass.ModPlayers;

public class GuardianModPlayer : ModPlayer
{
    public GuardianShield CurrentShield;
    
    public int LastDurability = -1;
    public int LastShieldType;

    public int Wards = 0;
    private int WardExpireTimer = 0;
    private int IndividualExprireTimer = 0;
    private bool Countdown = false;

    public int maxCooldown = 0;
    public int cooldown = 0;

    public bool UsingMace = false;

    public bool ShieldPolish = false;
    public void AddWard(int num)
    {
        Main.NewText(Player.name);
        Wards += num;
        WardExpireTimer = 480;
        IndividualExprireTimer = 60;
        Countdown = true;
    }

    public void SetCooldown(int Cooldown)
    {
        cooldown = Cooldown;
        maxCooldown = cooldown;
    }

    public override void PostUpdateMiscEffects()
    {
        if (cooldown > 0)
        {
            cooldown--;
        }
        else
        {
            cooldown = 0;
            maxCooldown = 0;
        }
    }

    public override void PreUpdate()
    {
        ShieldPolish = false;

        

        if (GuardianSystem.GuardianShieldItems.Contains(Player.HeldItem.type))
        {
            CurrentShield = (GuardianShield)Player.HeldItem.ModItem;
        }

        if(Wards > MathF.Min(Player.statLifeMax, 400) / 20)
        {
            Wards = (int)MathF.Min(Player.statLifeMax, 400) / 20;
        }

        if (Countdown)
        {
            if (WardExpireTimer-- <= 0)
            {
                if (IndividualExprireTimer-- <= 0)
                {
                    Wards -= 1;
                    IndividualExprireTimer = 60;

                    if(Wards <= 0)
                    {
                        Wards = 0;
                        WardExpireTimer = 0;
                        IndividualExprireTimer = 0;
                        Countdown = false;
                    }
                }

            }
        }
    }

    public bool ShieldUsable = true;

    public override void DrawEffects(PlayerDrawSet drawInfo, ref float r, ref float g, ref float b, ref float a, ref bool fullBright)
    {
        if(cooldown != 0 && maxCooldown != 0)
        {

            ShieldUsable = false;
            

            Primitives2D.DrawArc(Main.spriteBatch, Player.Center - Main.screenPosition, Player.height - 15, 360, 0f - MathHelper.PiOver2, MathHelper.Lerp(0f, MathHelper.TwoPi, (float)cooldown / (float)maxCooldown), Color.Yellow * 0.5f, 10) ;
        } else
        {
            ShieldUsable = true;
        }
        
    }

    public override void UpdateEquips()
    {
        if (ShieldPolish)
        {

        }
    }
    public override void PostUpdateEquips()
    {
        for(int i  = 0; i < Wards; i++)
        {
            Player.statDefense += 3;
        }
        
    }

}

public class ShieldData
{
    public int durability;
    public int maxDurability;
    public int FlashTim = 0;
    public float Scale = 0;
    public bool Flash = false;
    public ShieldData(int durability, int maxDurability)
    {
        this.durability = durability;
        this.maxDurability = maxDurability;
    }
}

public class GuardianSystem : ModSystem
{
    public static List<int> GuardianShieldItems = new();
    public static Dictionary<Projectile, ShieldData> shieldData = new Dictionary<Projectile, ShieldData>();
    public static List<int> ShieldProjectiles = new List<int>();

   
}

 
