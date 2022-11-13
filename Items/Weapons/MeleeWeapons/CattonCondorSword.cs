using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using System.Collections.Generic;
using Terraria.ModLoader.IO;
using System.IO;

namespace AuroraMod.Items.Weapons.MeleeWeapons {
	public class CattonCondorSword : ModItem {
		private Color pack = Color.White;
		private Color oldPack = Color.White;
		private uint timer = 0u;
		private float lerpValue = 1f;
		private bool start = false;

		public Color CottonColor => Color.Lerp(pack, oldPack, lerpValue);

		public static void OnHit(Entity entity, Player player, Projectile projectile, ref int damage, ref bool crit) {
			if (player.HeldItem.type == ModContent.ItemType<CattonCondorSword>()) {
				CattonCondorSword sword = (CattonCondorSword)player.HeldItem.ModItem;
				if (sword.start)
					sword.RandomizePack();

				Main.NewText(sword.pack.ToString());
			}
		}

		public override void SetStaticDefaults() {
			DisplayName.SetDefault("Cattor-Condor Sword");
			Tooltip.SetDefault("Cats, luck, and cotton." +
				"\nHow sweet that is?");

			SacrificeTotal = 1;
		}

		public override void SetDefaults() {
			Item.width = 50;
			Item.height = 50;

			Item.damage = 1;
			Item.DamageType = DamageClass.Melee;
			Item.useTime = 16;
			Item.useAnimation = 16;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.knockBack = 0f;
			Item.scale = 1.2f;
			Item.rare = ItemRarityID.Cyan;
			Item.UseSound = SoundID.Item1;
			Item.autoReuse = true;
		}

		public override void ModifyHitNPC(Player player, NPC target, ref int damage, ref float knockBack, ref bool crit) {
			OnHit(target, player, null, ref damage, ref crit);
		}

		public override void ModifyHitPvp(Player player, Player target, ref int damage, ref bool crit) {
			OnHit(target, player, null, ref damage, ref crit);
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

		public override void NetReceive(BinaryReader reader) {
			pack = reader.ReadRGB();
			oldPack = reader.ReadRGB();
			timer = reader.ReadUInt32();
			lerpValue = reader.ReadSingle();
			start = reader.ReadBoolean();
		}

		public override void NetSend(BinaryWriter writer) {
			writer.WriteRGB(pack);
			writer.WriteRGB(oldPack);
			writer.Write(timer);
			writer.Write(lerpValue);
			writer.Write(start);
		}

		public override void SaveData(TagCompound tag) {
			tag["p"] = pack;
			tag["o"] = oldPack;
			tag["t"] = timer;
			tag["l"] = lerpValue;
			tag["s"] = start;
		}

		public override void LoadData(TagCompound tag) {
			pack = tag.Get<Color>("p");
			oldPack = tag.Get<Color>("o");
			timer = tag.Get<uint>("t");
			lerpValue = tag.GetFloat("t");
			start = tag.GetBool("s");
		}
	}
}
