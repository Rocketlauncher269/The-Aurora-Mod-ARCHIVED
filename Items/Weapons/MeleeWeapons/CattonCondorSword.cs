using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using System.Collections.Generic;

namespace AuroraMod.Items.Weapons.MeleeWeapons {
	public class CattonCondorSword : ModItem {
		private Color pack = Color.White;
		private Color oldPack = Color.White;
		private uint timer = 0u;
		private float lerpValue = 1f;
		private bool start = false;

		public Color CottonColor => Color.Lerp(pack, oldPack, lerpValue);

		public override void SetStaticDefaults() {
			DisplayName.SetDefault("Cattor-Condor Sword");
			Tooltip.SetDefault("Cats, luck, and cotton." +
				"\nHow sweet that is?");

			SacrificeTotal = 1;
		}

		public override void SetDefaults() {
			Item.width = 50;
			Item.height = 50;

			Item.damage = 8;
			Item.DamageType = DamageClass.Melee;
			Item.useTime = 16;
			Item.useAnimation = 16;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.knockBack = 3;
			Item.scale = 1.2f;
			Item.rare = ItemRarityID.Cyan;
			Item.UseSound = SoundID.Item1;
			Item.autoReuse = true;
		}

		public override void Update(ref float gravity, ref float maxFallSpeed) {
			UpdateInventory(null);
		}

		public override void UpdateInventory(Player player) {
			if (start)
				return;

			lerpValue = ++timer % 60 / 60f;
			if (lerpValue >= 1f)
				start = true;
		}

		public override void ModifyTooltips(List<TooltipLine> tooltips) {
			int i = tooltips.FindIndex(x => x.Name == "ItemName" && x.Mod == "Terraria");
			if (i == -1)
				return;

			tooltips[i].OverrideColor = CottonColor;
		}

		public override void OnCreate(ItemCreationContext context) {
			oldPack = Color.White;
			pack = new Color(Main.rand.Next(byte.MaxValue), Main.rand.Next(byte.MaxValue), Main.rand.Next(byte.MaxValue));
			lerpValue = 1f;
			start = false;
		}

		public void RandomizePack() {
			oldPack = pack;
			pack = new Color(Main.rand.Next(byte.MaxValue), Main.rand.Next(byte.MaxValue), Main.rand.Next(byte.MaxValue));
			lerpValue = 0f;
			start = false;
			timer = 0u;
		}
	}
}
