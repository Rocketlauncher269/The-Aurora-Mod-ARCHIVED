using AuroraMod.Content.Items.Consumable.Bolts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.ModLoader;

namespace AuroraMod.Common.Bases.Items
{
    public abstract class CrossbowItem : ModItem
    {
        public abstract int UseTime { get; }
        public abstract int MaxDamage { get; }
        public abstract int Knockback { get; }
        public abstract float MaxShootSpeed { get; }
        public virtual SoundStyle ShootSound => SoundID.Item102;
        public virtual Vector2 CenterOffset => Vector2.Zero;
        public virtual Vector2 DrawOriginOffset => Vector2.Zero;
        public virtual Vector2 MuzzleOffset => Vector2.UnitX * 20;
        /// <summary>
        /// How much recoil the crossbow will have: X - origin offset, Y - rotation offset.
        /// </summary>
        public virtual Vector2 Recoil => new Vector2(10, 0.15f);
        public virtual void RecoilDiminish(ref Vector2 recoil) => recoil *= 0.98f;
        /// <summary>
        /// Allows you to modify how the crossbow shoots.
        /// </summary>
        /// <param name="player"></param>
        /// <param name="source"></param>
        /// <param name="position"></param>
        /// <param name="velocity"></param>
        /// <param name="type"></param>
        /// <param name="damage"></param>
        /// <param name="knockback"></param>
        /// <returns>Return false to disable automatic shooting. Returns true by default.</returns>
        public virtual bool ShootCrossbow(Player player, IEntitySource source, Vector2 position, Vector2 velocity, int type, int damage, float knockback) => true;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="progress">How long the player charged the bow (0.0 - 1.0).</param>
        /// <returns>Bolt velocity multiplier. Returns progress by default.</returns>
        public virtual float ShootVelocityScaling(float progress) => progress;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="progress">How long the player charged the bow (0.0 - 1.0).</param>
        /// <returns>Bolt damage multiplier. Returns 1 by default.</returns>
        public virtual float ShootDamageScaling(float progress) => 1;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="progress">How long the player charged the bow (0.0 - 1.0).</param>
        /// <returns>Charge progress bar color.</returns>
        public virtual Color ChargeBarColor(float progress) => Color.OrangeRed;
        public virtual int ItemValue => 10;
        public virtual int ItemRare => ItemRarityID.LightRed;
        public override sealed void SetDefaults()
        {
            Item.damage = MaxDamage;
            Item.DamageType = DamageClass.Ranged;
            Item.knockBack = Knockback;

            Item.width = 0;
            Item.height = 0;

            Item.useTime = Item.useAnimation = 1;
            Item.useStyle = -1;

            Item.holdStyle = ItemHoldStyleID.HoldHeavy;

            Item.autoReuse = true;
            Item.noUseGraphic = true;
            Item.noMelee = true;

            Item.value = ItemValue;
            Item.rare = ItemRare;

            Item.shoot = ModContent.ProjectileType<WoodenBoltProjectile>();
            Item.useAmmo = ModContent.ItemType<WoodenBolt>();
        }

        public override bool CanConsumeAmmo(Item ammo, Player player)
        {
            if (crossBowProj is not null)
            {
                crossBowProj.ai[0] = ammo.type;
            }
            return false;
        }

        public override sealed bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback) => false;

        Projectile crossBowProj;
        public override void HoldItem(Player player)
        {
            int type = ModContent.ProjectileType<CrossbowHeldProjectile>();
            if (player.ownedProjectileCounts[type] <= 0)
            {
                crossBowProj = Projectile.NewProjectileDirect(
                    new EntitySource_ItemUse(Item, Item),
                    player.Center,
                    Vector2.Zero,
                    type,
                    Item.damage,
                    Item.knockBack,
                    player.whoAmI
                    );
            }
        }
    }

    public class CrossbowHeldProjectile : ModProjectile
    {
        public override string Texture => "Terraria/Images/Item_" + ItemID.Acorn;

        public override void SetDefaults()
        {
            Projectile.width = 0;
            Projectile.height = 0;
            Projectile.aiStyle = -1;
            Projectile.DamageType = DamageClass.MeleeNoSpeed;
            Projectile.friendly = false;
            Projectile.hostile = false;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 9999;
            Projectile.netImportant = true;
            Projectile.extraUpdates = 3;
        }

        public CrossbowItem CrossbowItem { get; private set; }
        public override void OnSpawn(IEntitySource source)
        {
            if (source is EntitySource_ItemUse itemSource && itemSource.Item.ModItem is CrossbowItem crossbowItem)
            {
                CrossbowItem = crossbowItem;
            }
            else
            {
                Projectile.Kill();
            }
        }

        void UpdateCenterRot()
        {
            // Get player shoulder pos
            Projectile.Center = Player.RotatedRelativePoint(Player.MountedCenter) + new Vector2(-4 * Player.direction, -2);

            directionToMouse = Projectile.Center.DirectionTo(Main.MouseWorld);

            Projectile.Center += directionToMouse.RotatedBy(-MathHelper.PiOver2 * Player.direction) * CrossbowItem.CenterOffset.Y;
            directionToMouse = Projectile.Center.DirectionTo(Main.MouseWorld);

            Projectile.netUpdate = true;
        }

        Player Player => Main.player[Projectile.owner];
        Vector2 directionToMouse;
        Vector2 recoil;
        public int ShootTimer { get; private set; }
        public bool shouldShoot;
        public override void AI()
        {
            if (Player.HeldItem.type != CrossbowItem.Type)
            {
                Projectile.Kill();
                return;
            }

            Player.heldProj = Projectile.whoAmI;

            if (Main.myPlayer == Player.whoAmI)
            {
                UpdateCenterRot();

                int maxTimer = CrossbowItem.UseTime * (Projectile.extraUpdates + 1);

                if (PlayerInput.Triggers.Current.MouseLeft && Player.HasItem((int)Projectile.ai[0]))
                {
                    ShootTimer++;
                    if (ShootTimer >= maxTimer)
                    {
                        ShootTimer = maxTimer;
                    }
                }
                else
                {
                    if (ShootTimer > maxTimer * 0.2f)
                        ShootBolt();
                    ShootTimer = 0;
                }

                Player.direction = MathF.Sign(Main.MouseWorld.X - Player.Center.X);
            }

            

            Projectile.rotation = directionToMouse.ToRotation() + -recoil.Y * Player.direction;
            Player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, Projectile.rotation - MathHelper.PiOver2);
            Player.SetCompositeArmBack(true, Player.CompositeArmStretchAmount.Full, Projectile.rotation - MathHelper.PiOver2);

            CrossbowItem.RecoilDiminish(ref recoil);
            recoil = Vector2.Clamp(recoil, Vector2.Zero, Vector2.One * 999);
        }

        public void ShootBolt()
        {
            float shootProg = (float)ShootTimer / (CrossbowItem.UseTime * (Projectile.extraUpdates + 1));

            IEntitySource source = CrossbowItem.Item.GetSource_ItemUse(CrossbowItem.Item);

            Vector2 muzzlePos = Projectile.Center + CrossbowItem.MuzzleOffset.RotatedBy(Projectile.rotation);
            if (!Collision.CanHit(Projectile.Center, 0, 0, muzzlePos, 0, 0))
            {
                muzzlePos = Projectile.Center;
            }

            Vector2 velocity = directionToMouse * CrossbowItem.MaxShootSpeed * shootProg;
            int type = ContentSamples.ItemsByType[(int)Projectile.ai[0]].shoot;
            int damage = (int)(Projectile.damage * CrossbowItem.ShootDamageScaling(shootProg));
            if (CrossbowItem.ShootCrossbow(Player, source, muzzlePos, velocity, type, damage, Projectile.knockBack))
            {
                Projectile.NewProjectile(
                    source,
                    muzzlePos,
                    velocity,
                    type,
                    damage,
                    Projectile.knockBack,
                    Player.whoAmI
                    );
            }

            Player.ConsumeItem((int)Projectile.ai[0]);

            SoundEngine.PlaySound(CrossbowItem.ShootSound, Projectile.Center);

            recoil += CrossbowItem.Recoil * shootProg;
        }

        public override bool ShouldUpdatePosition() => false;

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.WriteVector2(directionToMouse);
            writer.WriteVector2(recoil);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            directionToMouse = reader.ReadVector2();
            recoil = reader.ReadVector2();
        }

        float shakeTimer;
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Item[CrossbowItem.Type].Value;
            Vector2 origin = new Vector2(0, texture.Height * 0.5f) + CrossbowItem.DrawOriginOffset + Vector2.UnitX * recoil.X;

            float progress = (float)ShootTimer / (CrossbowItem.UseTime * (Projectile.extraUpdates + 1));
            if (progress == 1)
            {
                shakeTimer += 0.01f;
            }
            else
            {
                shakeTimer = 0;
            }

            float progAlpha = -MathF.Pow(progress - 1, 6) + 1;

            Vector2 barPosition = Projectile.Center + directionToMouse * texture.Width * 0.45f + directionToMouse.RotatedBy(-MathHelper.PiOver2 * Player.direction) * texture.Height * 0.55f - Main.screenPosition;
            Color barColor = CrossbowItem.ChargeBarColor(progress);

            Main.EntitySpriteDraw(
                texture,
                Projectile.Center - Main.screenPosition + Main.rand.NextVector2Unit() * 2f * Math.Clamp(shakeTimer, 0f, 1f) * (Main.rand.NextBool(4) ? 1 : 0),
                null,
                lightColor,
                Projectile.rotation + (Player.direction == -1 ? MathHelper.Pi : 0),
                Player.direction == -1 ? new Vector2(texture.Width - origin.X, origin.Y) : origin,
                Projectile.scale,
                Player.direction == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None,
                0
                );

            /*
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);

            Texture2D epicGlow = ModContent.Request<Texture2D>("AuroraMod/Content/Assets/CBBar_Something", AssetRequestMode.ImmediateLoad).Value;
            Main.EntitySpriteDraw(
                epicGlow,
                barPosition,
                null,
                Color.Black * progAlpha * 0.4f,
                Projectile.rotation,
                epicGlow.Size() * 0.5f + Vector2.UnitX * recoil.X,
                new Vector2(progress, 1) / 5,
                SpriteEffects.None,
                0
                );

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
            */

            Texture2D tex = ModContent.Request<Texture2D>("AuroraMod/Content/Assets/CBBarOutline", AssetRequestMode.ImmediateLoad).Value;
            Main.EntitySpriteDraw(
                tex,
                barPosition,
                null,
                Color.White * progAlpha,
                Projectile.rotation,
                tex.Size() * 0.5f + Vector2.UnitX * recoil.X,
                1,
                Player.direction == -1 ? SpriteEffects.FlipVertically : SpriteEffects.None,
                0
                );

            Texture2D texBar = ModContent.Request<Texture2D>("AuroraMod/Content/Assets/CBBar", AssetRequestMode.ImmediateLoad).Value;
            Main.EntitySpriteDraw(
                texBar,
                barPosition,
                null,
                barColor * progAlpha,
                Projectile.rotation,
                texBar.Size() * 0.5f + Vector2.UnitX * recoil.X,
                new Vector2(progress, 1),
                Player.direction == -1 ? SpriteEffects.FlipVertically : SpriteEffects.None,
                0
                );

            return false;
        }
    }
}
