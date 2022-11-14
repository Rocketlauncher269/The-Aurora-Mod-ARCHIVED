using AuroraMod.Common.ModPlayers;
using AuroraMod.Content.Dusts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace AuroraMod.Content.Items.Weapons.RangedWeapons
{
    public class LeverActionShotgunProjectile : ModProjectile
    {
        public override string Texture => base.Texture.Replace("Projectile", "");

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

        int initialDirection;
        public override void OnSpawn(IEntitySource source)
        {
            initialDirection = Player.direction;

            ShootBullets(4);
            Player.GetModPlayer<AuroraModPlayer>().ShakeScreen(8f, 0.48f);
        }

        void ShootBullets(int amount)
        {
            if (Main.myPlayer != Player.whoAmI)
                return;

            Vector2 dirToMouse = Center.DirectionTo(Main.MouseWorld);
            Vector2 muzzlePosition = Center + dirToMouse * 42;
            for (int i = 0; i < amount; i++)
            {
                Projectile.NewProjectile(
                    Projectile.GetSource_FromThis(), 
                    muzzlePosition, 
                    dirToMouse.RotatedByRandom(MathHelper.PiOver4 * 0.14f) * Main.rand.NextFloat(12, 14),
                    ModContent.ProjectileType<LeverActionShotgunBulletProjectile>(),
                    Projectile.damage,
                    Projectile.knockBack * 1.5f / amount,
                    Player.whoAmI
                    );
            }

            for (int i = 0; i < Main.rand.Next(3, 4); i++)
            {
                Gore gore = Gore.NewGoreDirect(
                    Projectile.GetSource_FromThis(),
                    muzzlePosition + dirToMouse * 8,
                    dirToMouse.RotatedByRandom(MathHelper.PiOver4 * 0.3f) * Main.rand.Next(8, 11),
                    GoreID.Smoke1 + Main.rand.Next(3),
                    Main.rand.NextFloat(0.3f, 0.6f)
                    );

                gore.position -= new Vector2(gore.Width, gore.Height) * 0.5f;

                Vector2 dustVel = dirToMouse.RotatedByRandom(MathHelper.PiOver4 * 0.3f) * Main.rand.Next(7, 12);
                Dust.NewDust(muzzlePosition, 0, 0, DustID.MinecartSpark, dustVel.X, dustVel.Y, Scale: 5);

                dustVel = dirToMouse.RotatedByRandom(MathHelper.PiOver4 * 0.4f) * Main.rand.Next(7, 9);
                Dust.NewDust(muzzlePosition, Projectile.width, Projectile.height, ModContent.DustType<LeverGunDust>(), dustVel.X, dustVel.Y, Scale: Main.rand.NextFloat(0.8f, 1.3f));
            }

            for (int i = 0; i < 20; i++)
            {
                Vector2 position = muzzlePosition + Main.rand.NextVector2Unit() * 4 - dirToMouse * 15;
                Vector2 vel = muzzlePosition.DirectionTo(muzzlePosition) * 3;
                Dust.NewDust(position, 0, 0, DustID.MinecartSpark, vel.X, vel.Y, Scale: Main.rand.NextFloat(2.6f, 4.7f));
            }

            Player.velocity -= dirToMouse * 2f;

            float lightAmount = 1.35f;
            Lighting.AddLight(muzzlePosition, 1f * lightAmount, 0.9f * lightAmount, 0.7f * lightAmount);
        }

        Vector2 Center => Player.RotatedRelativePoint(Player.MountedCenter) + new Vector2(Player.direction * -3, -3);

        Player Player => Main.player[Projectile.owner];
        bool playedSound;
        float recoilRot;
        float animationProgress;
        public override void AI()
        {
            if (Player.ItemAnimationEndingOrEnded)
            {
                Projectile.Kill();
                return;
            }

            if (Main.myPlayer == Player.whoAmI)
            {
                Vector2 directionToMouse = Center.DirectionTo(Main.MouseWorld);
                float rotationToMouse = directionToMouse.ToRotation();

                Projectile.Center = Center + directionToMouse * 10;
                Player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, rotationToMouse - MathHelper.PiOver2);

                Player.heldProj = Projectile.whoAmI;
                Player.direction = initialDirection;

                animationProgress = (float)(Player.itemAnimationMax - Player.itemAnimation) / Player.itemAnimationMax;

                float rotateAfter = 0.35f;
                float stopRotBefore = 0.7f;
                if (animationProgress >= rotateAfter && animationProgress < stopRotBefore)
                {
                    Projectile.rotation -= (MathHelper.TwoPi / (Player.itemTimeMax * (stopRotBefore - rotateAfter))) * Player.direction;
                }
                else
                {
                    Projectile.rotation = rotationToMouse + (Player.direction == -1 ? MathHelper.Pi : 0) + recoilRot;

                    if (!playedSound && animationProgress > rotateAfter - 0.15f)
                    {
                        SoundEngine.PlaySound(SoundID.Item149 with { Pitch = Main.rand.NextFloat(0.3f) }, Projectile.Center);
                        playedSound = true;
                    }

                    if (animationProgress > 0.008f)
                    {
                        recoilRot *= 0.8f;
                    }
                    else
                    {
                        recoilRot -= 0.5f * Player.direction;
                    }
                }

            }
        }

        public override bool ShouldUpdatePosition() => false;

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = TextureAssets.Projectile[Type].Value;

            Vector2 origin = Player.direction == -1 ? new Vector2(tex.Width - 6, 10) : new Vector2(6, 10);

            Main.spriteBatch.Draw(
                tex,
                Projectile.Center - Main.screenPosition,
                null,
                lightColor,
                Projectile.rotation,
                origin,
                Projectile.scale,
                Player.direction == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None,
                0
                );

            Texture2D texHot = ModContent.Request<Texture2D>(Texture + "_HotMask", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;

            float mult = (1f - animationProgress * 1.4f);

            Main.spriteBatch.Draw(
                texHot,
                Projectile.Center - Main.screenPosition,
                null,
                Color.White * mult,
                Projectile.rotation,
                origin,
                Projectile.scale,
                Player.direction == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None,
                0
                );

            Texture2D glowTex = ModContent.Request<Texture2D>("AuroraMod/Content/Assets/Glow", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
            Vector2 rotDir = Projectile.rotation.ToRotationVector2();

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, null, null, null, null, Main.GameViewMatrix.ZoomMatrix);

            Main.spriteBatch.Draw(
                glowTex,
                Projectile.Center + rotDir * 30 * Player.direction - rotDir.RotatedBy(MathHelper.PiOver2) * 4 - Main.screenPosition,
                null,
                Color.Orange * mult * 0.3f,
                Projectile.rotation,
                glowTex.Size() * 0.5f,
                Projectile.scale * 0.4f * new Vector2(1.4f, 1.15f) * mult,
                SpriteEffects.None,
                0
                );

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null, Main.GameViewMatrix.ZoomMatrix);

            return false;
        }
    }
}
