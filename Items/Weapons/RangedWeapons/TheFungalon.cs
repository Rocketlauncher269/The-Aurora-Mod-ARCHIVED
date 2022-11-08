using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AuroraMod.Items.Weapons.RangedWeapons
{
    public class TheFungalon : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("The Fungalon");
            Tooltip.SetDefault(" ");
        }

        public override void SetDefaults()
        {
            Item.damage = 22;
            Item.width = 12;
            Item.height = 38;
            Item.maxStack = 1;
            Item.useTime = 12;
            Item.useAnimation = 12;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 2;
            Item.rare = ItemRarityID.Green;
            Item.UseSound = SoundID.Item5;
            Item.noMelee = true;
            Item.shoot = ProjectileID.WoodenArrowFriendly;
            Item.useAmmo = AmmoID.Arrow;
            Item.shootSpeed = 10f;
            Item.autoReuse = false;
            
        }
    }
}