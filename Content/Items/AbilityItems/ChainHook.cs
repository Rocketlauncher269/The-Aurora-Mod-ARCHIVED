using AuroraMod.Common;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace AuroraMod.Content.Items.AbilityItems
{
    public class ChainHook : ModItem, IAbilityItem
    {
        public int Cooldown => 30;
        public Color Color => Color.Gray.MultiplyRGB(Color.Orange);

        public void OnUse(Player player, IEntitySource source)
        {
            SoundEngine.PlaySound(SoundID.Item1, player.Center);
            Projectile.NewProjectile(source, player.Center, player.Center.DirectionTo(Main.MouseWorld) * 10, ModContent.ProjectileType<ChainHookProjectile>(), 1, 0, player.whoAmI);
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
