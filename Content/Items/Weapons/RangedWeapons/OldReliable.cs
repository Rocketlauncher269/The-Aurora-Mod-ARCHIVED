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

        public override Color ChargeBarColor(float progress)
        {
            Color colorProg = Color.Lerp(Color.DarkRed * 1.25f, Color.Orange, MathF.Pow(progress, 2));

            return Color.Lerp(colorProg, Color.Lerp(colorProg, Color.White, 0.3f), (MathF.Sin(Main.GameUpdateCount * 0.08f) + 1) * 0.5f);
        }
    }
}
