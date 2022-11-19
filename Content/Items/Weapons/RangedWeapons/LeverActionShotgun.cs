using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace AuroraMod.Content.Items.Weapons.RangedWeapons
{
    public class LeverActionShotgun : ModItem
    {
        public override void SetStaticDefaults()
        {
            //DisplayName.SetDefault("L . A . S");
        }

        public override void SetDefaults()
        {
            Item.damage = 20;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 40;
            Item.height = 40;
            Item.useTime = 60;
            Item.useAnimation = 60;
            Item.useStyle = -1;
            Item.knockBack = 4;
            Item.value = 17000;
            Item.rare = ItemRarityID.LightRed;
            Item.UseSound = SoundID.Item38;
            Item.autoReuse = true;
            Item.noUseGraphic = true;
            Item.noMelee = true;
            Item.shoot = ModContent.ProjectileType<LeverActionShotgunProjectile>();
            Item.shootSpeed = 9f;
            Item.useAmmo = AmmoID.Bullet;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Projectile.NewProjectile(source, position, velocity, Item.shoot, damage, knockback, player.whoAmI, type);
            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.IllegalGunParts)
                .AddIngredient(ItemID.Boomstick)
                .AddRecipeGroup(RecipeGroupID.IronBar, 15)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}
