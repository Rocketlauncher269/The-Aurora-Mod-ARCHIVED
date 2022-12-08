using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.ModLoader;

namespace AuroraMod.Common
{
    public abstract class CrossbowItem : ModItem
    {
        public abstract int UseTime { get; }
        public abstract int Damage { get; }
        public abstract float ShootSpeed { get; }
        public virtual float TimeToShoot => 0.45f;
        public virtual SoundStyle ShootSound => SoundID.Item10;
        public virtual Vector2 CenterOffset => Vector2.Zero;
        public virtual Vector2 DrawOriginOffset => Vector2.Zero;
        public virtual Vector2 MuzzleOffset => Vector2.Zero;
        /// <summary>
        /// How much recoil the crossbow will have: X - origin offset, Y - rotation offset.
        /// </summary>
        public virtual Vector2 Recoil => new Vector2(9, 0f);
        public virtual float RecoilDiminish => 0.87f;
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
        /// <returns>Return false to disable automatic shooting.</returns>
        public virtual bool ShootCrossbow(Player player, IEntitySource source, Vector2 position, Vector2 velocity, int type, int damage, float knockback) => true;
        public override void SetDefaults()
        {
            Item.damage = Damage;
            Item.DamageType = DamageClass.Ranged;
            Item.knockBack = 4;

            Item.width = 0;
            Item.height = 0;

            Item.useTime = Item.useAnimation = UseTime;
            Item.useStyle = -1;

            Item.autoReuse = true;
            Item.noUseGraphic = true;
            Item.noMelee = true;
            Item.channel = true;

            Item.value = 17000;
            Item.rare = ItemRarityID.LightRed;
            
            Item.shoot = ModContent.ProjectileType<CrossbowHeldProjectile>();
            Item.shootSpeed = 0;
            Item.useAmmo = AmmoID.StyngerBolt;
        }

        public override sealed bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Projectile.NewProjectile(
                source,
                position,
                velocity,
                Item.shoot,
                damage,
                knockback,
                player.whoAmI,
                type
                );
            return false;
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
        }

        CrossbowItem crossbowItem;
        public override void OnSpawn(IEntitySource source)
        {
            if (source is EntitySource_ItemUse_WithAmmo itemSource && itemSource.Item.ModItem is CrossbowItem crossbowItem)
            {
                this.crossbowItem = crossbowItem;
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

            Projectile.Center += directionToMouse.RotatedBy(-MathHelper.PiOver2 * Player.direction) * crossbowItem.CenterOffset.Y;
            directionToMouse = Projectile.Center.DirectionTo(Main.MouseWorld);

            Projectile.netUpdate = true;
        }

        Player Player => Main.player[Projectile.owner];
        Vector2 directionToMouse;
        Vector2 recoil;
        bool canShoot = true;
        public override void AI()
        {
            if (Player.ItemAnimationEndingOrEnded || Player.HeldItem.type != crossbowItem.Type)
            {
                Projectile.Kill();
                return;
            }

            Player.heldProj = Projectile.whoAmI;

            if (Main.myPlayer == Player.whoAmI)
            {
                UpdateCenterRot();

                if (PlayerInput.Triggers.JustReleased.MouseLeft)
                {
                    canShoot = false;
                }

                if (canShoot && (1f - (float)Player.itemAnimation / Player.itemAnimationMax) > crossbowItem.TimeToShoot)
                {
                    IEntitySource source = crossbowItem.Item.GetSource_ItemUse(crossbowItem.Item);
                    Vector2 muzzlePos = Projectile.Center + crossbowItem.MuzzleOffset.RotatedBy(Projectile.rotation);
                    Vector2 velocity = directionToMouse * crossbowItem.ShootSpeed;
                    int type = (int)Projectile.ai[0];
                    if (crossbowItem.ShootCrossbow(Player, source, muzzlePos, velocity, type, Projectile.damage, Projectile.knockBack))
                    {
                        Projectile.NewProjectile(
                            source,
                            muzzlePos,
                            velocity,
                            type,
                            Projectile.damage,
                            Projectile.knockBack,
                            Player.whoAmI
                            );
                    }

                    SoundEngine.PlaySound(crossbowItem.ShootSound, Projectile.Center);

                    recoil += crossbowItem.Recoil;
                    canShoot = false;
                }
            }

            Projectile.rotation = directionToMouse.ToRotation() + -recoil.Y * Player.direction;
            Player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, Projectile.rotation - MathHelper.PiOver2);

            recoil *= crossbowItem.RecoilDiminish;
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

        public override bool PreDraw(ref Color lightColor)
        {
            

            return false;
        }

        public override void PostDraw(Color lightColor)
        {
            Texture2D texture = TextureAssets.Item[crossbowItem.Type].Value;
            Vector2 origin = new Vector2(0, texture.Height * 0.5f) + crossbowItem.DrawOriginOffset + Vector2.UnitX * recoil.X;

            Main.EntitySpriteDraw(
                texture,
                Projectile.Center - Main.screenPosition,
                null,
                lightColor,
                Projectile.rotation + (Player.direction == -1 ? MathHelper.Pi : 0),
                Player.direction == -1 ? new Vector2(texture.Width - origin.X, origin.Y) : origin,
                Projectile.scale,
                Player.direction == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None,
                0
                );
        }
    }
}
