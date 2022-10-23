using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.GameContent.Creative;
using Terraria.ModLoader;

namespace AuroraMod.Items.Tools
{
	public class Toothpick : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Toothpick");
			Tooltip.SetDefault("");

			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
		}

		public override void SetDefaults()
		{
			Item.damage = 15;
			Item.DamageType = DamageClass.Melee;
			Item.width = 16;
			Item.height = 16;
			Item.useTime = 18;
		        Item.pick = 59;
			Item.useAnimation = 18;
			Item.useStyle = 1;
			Item.knockBack = 6;
			Item.value = 500;
			Item.rare = 3;
			Item.UseSound = SoundID.Item1;
			Item.autoReuse = true;
		}


		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
			recipe.AddIngredient(ItemID.Lens, 15);
			recipe.AddIngredient(ItemID.DemoniteBar, 3);
			recipe.AddTile(TileID.Anvils);
			recipe.Register();

			recipe = CreateRecipe();
			recipe.AddIngredient(ItemID.Lens, 15);
			recipe.AddIngredient(ItemID.CrimtaneBar, 3);
			recipe.AddTile(TileID.Anvils);
			recipe.Register();

		}
	}
}

