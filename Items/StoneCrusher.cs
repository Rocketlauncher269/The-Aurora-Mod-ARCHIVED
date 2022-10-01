using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AuroraMod.Items
{
	public class StoneCrusher : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Stone Crusher");
			Tooltip.SetDefault("");
		}

		public override void SetDefaults()
		{
			Item.damage = 12;
			Item.DamageType = DamageClass.Melee;
			Item.width = 40; // will edit width and height later btw - rocket
			Item.height = 40;
			Item.useTime = 27;
			Item.useAnimation = 27;
			Item.useStyle = 1;
			Item.knockBack = 7;
			Item.value = 500;
			Item.rare = 2;
			Item.UseSound = SoundID.Item1;
			Item.autoReuse = false;
		}

		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
			recipe.AddIngredient(ItemID.StoneBlock, 25);
			recipe.AddTile(TileID.Anvils);
			recipe.Register();
		}
	}
}
