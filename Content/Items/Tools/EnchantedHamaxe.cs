using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.GameContent.Creative;
using Terraria.ModLoader;

namespace AuroraMod.Content.Items.Tools
{
	public class EnchantedHamaxe : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Enchanted Hamaxe");
			Tooltip.SetDefault("");

			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
		}

		public override void SetDefaults()
		{
			Item.damage = 12;
			Item.DamageType = DamageClass.Melee;
			Item.width = 16;
			Item.height = 14;
			Item.useTime = 24;
		        Item.axe = 20;
		        Item.hammer = 60;
			Item.useAnimation = 24;
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
			recipe.AddIngredient(ModContent.ItemType<EnchantedShard>(), 20);
			recipe.AddTile(TileID.Anvils);
			recipe.Register();
		}
	}
}

