using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.GameContent.Creative;
using Terraria.ModLoader;

namespace AuroraMod.Items.Weapons.MeleeWeapons
{
	public class Shroomsaber : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Shroomsaber");
			Tooltip.SetDefault("");

			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
		}

		public override void SetDefaults()
		{
			Item.damage = 8;
			Item.DamageType = DamageClass.Melee;
			Item.width = 19;
			Item.height = 19;
			Item.useTime = 16;
			Item.useAnimation = 16;
			Item.useStyle = 1;
			Item.knockBack = 3;
			Item.value = 500;
			Item.scale = 1.2f;
			Item.rare = 3;
			Item.UseSound = SoundID.Item1;
			Item.autoReuse = true;
		}
	}
}
