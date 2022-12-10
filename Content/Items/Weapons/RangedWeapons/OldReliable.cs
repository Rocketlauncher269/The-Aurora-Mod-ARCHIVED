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
        public override int UseTime => 40;
        public override int Damage => 304;
        public override int Knockback => 4;
        public override float ShootSpeed => 19;
        public override Vector2 DrawOriginOffset => new Vector2(20, -7);
        public override Vector2 CenterOffset => Vector2.UnitY * 10;
        public override Vector2 MuzzleOffset => Vector2.UnitX * 30;
        public override Vector2 Recoil => new Vector2(9, 0.1f);
        public override Color ChargeBarColor(float progress) => Color.Lerp(Color.SaddleBrown, Color.GhostWhite, MathF.Pow(progress, 2));
    }
}
