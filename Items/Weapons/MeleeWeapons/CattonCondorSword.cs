using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using System.Collections.Generic;
using Terraria.ModLoader.IO;
using System.IO;
using System.Linq;
using System;
using Terraria.Utilities;
using System.Reflection.PortableExecutable;
using Terraria.DataStructures;

namespace AuroraMod.Items.Weapons.MeleeWeapons {
	public class CattonCondorSword : ModItem {
		private Color pack = Color.White;
		private Color oldPack = Color.White;
		private uint timer = 0u;
		private float lerpValue = 1f;
		private bool start = false;

		public Color CottonColor => Color.Lerp(pack, oldPack, lerpValue);
		public UnifiedRandom Random => new((int)CottonColor.PackedValue);

		public static void OnHit(Entity entity, Player player, Projectile projectile, ref int damage, ref bool crit) {
			if (player.HeldItem.type == ModContent.ItemType<CattonCondorSword>()) {
				CattonCondorSword sword = (CattonCondorSword)player.HeldItem.ModItem;
				bool rand(int num) => sword.Random.NextBool(num) || player.RollLuck(num * 64) == 0;

				crit = false;
				if (entity is NPC nPC)
					damage += nPC.defense;
				else if (entity is Player d)
					damage += d.statDefense;
				damage *= 50 + Main.rand.Next(-8, 9);

				if (rand(Utils.Clamp(101 - player.HeldItem.crit, 1, int.MaxValue)))
					crit = true;
				if (rand(4))
					damage *= Main.rand.Next(8) + 9;
				if (rand(8))
					damage *= Main.rand.Next(4) + 1;
				if (rand(20)) {
					if (entity is NPC n)
						damage += n.lifeMax / 100;
					else if (entity is Player t)
						damage += t.statLifeMax2 / 20;
				}
				if (rand(5))
					player.Heal(damage / 10);
				if (rand(32)) {
					for (int i = 0; i < Player.MaxBuffs; i++) {
						if (Main.debuff[player.buffType[i]]) {
							player.DelBuff(i);
						}
					}
				}
				if (rand(31)) {
					int type = sword.Random.Next(BuffLoader.BuffCount);
					while (Main.debuff[type]) {
						type = sword.Random.Next(BuffLoader.BuffCount);
					}
					player.AddBuff(type, (int)(sword.Random.NextFloat() * 3.14E+03));
				}
				if (entity is NPC npc && rand(33)) {
					(bool x, bool y) = (npc.velocity.X < 0f, npc.velocity.Y < 0f);
					npc.velocity = new Vector2(MathF.Sqrt(MathF.Abs(npc.velocity.X)), MathF.Sqrt(MathF.Abs(npc.velocity.Y)));
					if (x)
						npc.velocity.X *= -1f;
					if (y)
						npc.velocity.Y *= -1f;
				}

				if (sword.start)
					sword.RandomizePack();
			}
		}

		public override ModItem Clone(Item newEntity) {
			CattonCondorSword c = base.Clone(newEntity) as CattonCondorSword;
			c.lerpValue = lerpValue;
			c.oldPack = oldPack;
			c.pack = pack;
			c.start = start;
			c.timer = timer;
			return c;
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
			Item.DamageType = DamageClass.Magic;
			Item.useTime = 16;
			Item.useAnimation = 16;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.knockBack = 0f;
			Item.scale = 1.2f;
			Item.rare = ItemRarityID.Cyan;
			Item.UseSound = SoundID.Item1;
			Item.autoReuse = true;
			Item.shootSpeed = 12.5f;
			Item.shoot = ModContent.ProjectileType<Cotton>();
		}

		public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback) {
			if (Random.NextBool(24))
				type = 0;
		}

		public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback) {
			int index = Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI, pack.PackedValue);
			return false;
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

			lerpValue = (timer += 1) / 60f;
			if (lerpValue >= 1f)
				start = true;
		}

		public override float UseSpeedMultiplier(Player player) {
			float e = new UnifiedRandom((int)(pack.PackedValue * 5f)).NextFloat();
			return Random.NextBool(3) ? MathF.Abs(MathF.Cos(player.moveSpeed) + Random.NextFloat()) + e : 1f;
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

	public class Cotton : ModProjectile {
		public Color Color => AuroraUtils.ColorFromPacked((uint)Projectile.ai[0]);

		public override void SetDefaults() {
			Projectile.width = 20;
			Projectile.height = 20;
			Projectile.friendly = true;
			Projectile.hostile = false;
		}

		public override void AI() {
			Projectile.velocity *= 1.02075920079f;
			Projectile.rotation = Projectile.velocity.ToRotation();

			for (int i = 0; i < 15; i++) {
				int index = Dust.NewDust(Projectile.Center, 4, 4, DustID.PinkCrystalShard, SpeedX: Projectile.velocity.X, SpeedY: Projectile.velocity.Y);
				Color color = Color;
				color.R = (byte)(color.R + Main.rand.Next(64, 128));
				color.G = (byte)(color.G + Main.rand.Next(64, 128));
				color.B = (byte)(color.B + Main.rand.Next(64, 128));
				Main.dust[index].color = color;
			}
		}

		public override Color? GetAlpha(Color lightColor) {
			Color color = Color.MultiplyRGBA(lightColor * 0.5f);
			color.A = byte.MaxValue;
			return color;
		}

		public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection) {
			CattonCondorSword.OnHit(target, Main.player[Projectile.owner], Projectile, ref damage, ref crit);
		}

		public override void ModifyHitPvp(Player target, ref int damage, ref bool crit) {
			CattonCondorSword.OnHit(target, Main.player[Projectile.owner], Projectile, ref damage, ref crit);
		}
	}
}
