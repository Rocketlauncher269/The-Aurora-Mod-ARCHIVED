using Terraria;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Terraria.ID;
using Microsoft.Xna.Framework;
using Terraria.GameContent.Creative;
using Terraria.GameContent;
using Microsoft.Xna.Framework.Graphics;
using Terraria.Audio;
using System;
namespace AuroraMod.Items.Weapons.RangedWeapons
{
    public class BlazeMaker : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Blaze Maker");
            Tooltip.SetDefault("Has your candle desires all in one\n75% Chance to not consume ammo");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Item.type] = 1;
        }
        public override void SetDefaults()
        {
            Item.width = 54;
            Item.height = 20;
            Item.damage = 15;
            Item.DamageType = DamageClass.Ranged;
            Item.knockBack = 1;
            Item.noMelee = true;
            Item.useTime = 6;
            Item.useAnimation = 25;
            Item.autoReuse = true;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.value = Item.buyPrice(0, 3, 12, 0);
            Item.rare = ItemRarityID.Orange;
            Item.UseSound = SoundID.Item34;
            Item.shoot = ModContent.ProjectileType<FlameTexture>();
            Item.shootSpeed = 4;
            Item.useAmmo = AmmoID.Gel;
        }


		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
			recipe.AddIngredient(ItemID.Gel, 25);
			recipe.AddIngredient(ItemID.GoldBar, 10);
			recipe.AddTile(TileID.Anvils);
			recipe.Register();

			recipe = CreateRecipe();
			recipe.AddIngredient(ItemID.Gel, 25);
			recipe.AddIngredient(ItemID.PlatinumBar, 10);
			recipe.AddTile(TileID.Anvils);
			recipe.Register();
		}

        public override bool CanConsumeAmmo(Item ammo, Player player)
        {
            return Main.rand.NextFloat() >= 0.75f;
        }
        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-3f, 0f);
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Vector2 velocity2 = velocity.RotatedByRandom(MathHelper.ToRadians(19));
            Projectile.NewProjectile(source, position + velocity2 * 12, velocity2, type, damage, knockback, player.whoAmI);
            return false;
        }

    }
    public class FlameTexture : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 12;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.aiStyle = -1;
            Projectile.width = 40;
            Projectile.height = 40;
            Projectile.penetrate = -1;
            Projectile.friendly = true;
            Projectile.tileCollide = true;
            Projectile.timeLeft = 90;
            Projectile.extraUpdates = 2;
            Projectile.scale = Main.rand.NextFloat(0.3f, 0.7f);
        }
    

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            return false;
        }
        public override void AI()
        {
            Lighting.AddLight(Projectile.Center, new Vector3(0.755f, 0.140f, 0f));
            if (Projectile.timeLeft <= 20)
            {
                Projectile.scale -= 0.02f;
            }
            if (Projectile.scale <= 0)
            {
                Projectile.Kill();
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, null, null, null, null, Main.GameViewMatrix.ZoomMatrix);

            Main.instance.LoadProjectile(Projectile.type);
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            for (int k = 0; k < Projectile.oldPos.Length; k++)
            {
                var offset = new Vector2(Projectile.width / 2f, Projectile.height / 2f);
                Vector2 drawPos = (Projectile.oldPos[k] - Main.screenPosition) + offset;
                Color color = new Color(252, 152, 3, Projectile.oldPos.Length * 6) * (1f - Projectile.alpha) * ((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
               Main.spriteBatch.Draw(texture, drawPos, null, color, Projectile.oldRot[k], texture.Size() / 2, Projectile.scale, SpriteEffects.None, 0);
            }
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null, Main.GameViewMatrix.ZoomMatrix);


            return false;
        }
    }
}
