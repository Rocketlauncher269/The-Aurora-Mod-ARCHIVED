using AuroraMod.Common;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace AuroraMod.Content.Items.AbilityItems
{
    public class ChainHook : ModItem, IAbilityItem
    {
        public int Cooldown => 45;
        public Color Color => Color.Gray.MultiplyRGB(Color.Orange);

        public void OnUse(Player player, IEntitySource source)
        {
            Projectile.NewProjectile(source, player.Center, player.Center.DirectionTo(Main.MouseWorld) * 25, ModContent.ProjectileType<ChainHookProjectile>(), 5, 0, player.whoAmI);
        }

        public override void SetDefaults()
        {
            Item.width = 19;
            Item.height = 19;
            Item.useStyle = 1;
            Item.knockBack = 3;
            Item.value = 500;
            Item.scale = 1f;
            Item.rare = 3;
            Item.accessory = true;
        }
    }
}
