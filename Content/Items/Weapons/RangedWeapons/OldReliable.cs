using AuroraMod.Common.Bases.Items;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace AuroraMod.Content.Items.Weapons.RangedWeapons
{
    public class OldReliable : CrossbowItem
    {
        public override int UseTime => 70;
        public override int MaxDamage => 120;
        public override int Knockback => 4;
        public override float MaxShootSpeed => 26;
        public override Vector2 DrawOriginOffset => new Vector2(20, -7);
        public override Vector2 CenterOffset => Vector2.UnitY * 10;
        public override Vector2 MuzzleOffset => Vector2.UnitX * 30;
        public override Vector2 Recoil => new Vector2(9, 0.1f);

        float barColorTimer;
        public override Color ChargeBarColor(float progress)
        {
            barColorTimer += progress * 0.3f;
            if (progress == 0)
            {
                barColorTimer = 0;
            }
            
            return Color.Lerp(Color.Lerp(Color.DarkSlateBlue, Color.BlueViolet, progress), Color.Lerp(Color.White, Color.BlueViolet, 0.5f), progress * MathF.Pow((MathF.Sin(barColorTimer) + 1) / 2f, 4));
        }
    }
}
