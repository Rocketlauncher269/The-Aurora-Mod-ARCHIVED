using AuroraMod.Common;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AuroraMod.Content.Items.Weapons.RangedWeapons
{
    public class OldReliable : CrossbowItem
    {
        public override int UseTime => 75;

        public override int Damage => 304;

        public override float ShootSpeed => 14;
        public override Vector2 DrawOriginOffset => new Vector2(20, -7);
        public override Vector2 CenterOffset => Vector2.UnitY * 10;
    }
}
