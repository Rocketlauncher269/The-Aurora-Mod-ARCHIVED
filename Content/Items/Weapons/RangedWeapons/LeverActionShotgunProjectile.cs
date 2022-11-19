using AuroraMod.Common.ModPlayers;
using AuroraMod.Content.Dusts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
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

            ShootBullets();
            Player.GetModPlayer<AuroraModPlayer>().ShakeScreen(8f, 0.48f);
        }

        const int bulletsCount = 4;
        void ShootBullets()
        {
            if (Main.netMode == NetmodeID.Server)
                return;

            if (Main.myPlayer == Player.whoAmI)
                directionToMouse = RotationToMouse(Center.DirectionTo(Main.MouseWorld).ToRotation()).ToRotationVector2();

            Vector2 muzzlePosition = Center + directionToMouse.RotatedBy(-MathHelper.PiOver2) * 5;

            if (Main.myPlayer == Player.whoAmI)
                directionToMouse = muzzlePosition.DirectionTo(Main.MouseWorld);

            muzzlePosition += directionToMouse * 44;

            //Main.NewText(muzzlePosition.X - Player.Center.X);

            Vector2 shootFrom = Collision.CanHit(Center, 0, 0, muzzlePosition + directionToMouse * 10, 0, 0) ? muzzlePosition : Center;
            for (int i = 0; i < bulletsCount; i++)
            {
                Vector2 velocity = (i == 0 ? directionToMouse : directionToMouse.RotatedByRandom(MathHelper.PiOver4 * 0.14f)) * Main.rand.NextFloat(19, 24);

                Projectile.NewProjectile(
                    Projectile.GetSource_FromThis(),
                    shootFrom - directionToMouse * 15,
                    velocity,
                    //ModContent.ProjectileType<LeverActionShotgunBulletProjectile>(),
                    (int)Projectile.ai[0],
                    Projectile.damage,
                    Projectile.knockBack * 1.5f / bulletsCount,
                    Player.whoAmI
                    );
            }

            for (int i = 0; i < Main.rand.Next(3, 4); i++)
            {
                
                Gore gore = Gore.NewGoreDirect(
                    Projectile.GetSource_FromThis(),
                    muzzlePosition + directionToMouse * 8,
                    directionToMouse.RotatedByRandom(MathHelper.PiOver4 * 0.3f) * Main.rand.Next(8, 11),
                    GoreID.Smoke1 + Main.rand.Next(3),
                    Main.rand.NextFloat(0.4f, 0.7f)
                    );
                

                gore.position -= new Vector2(gore.Width, gore.Height) * 0.5f;
                

                Vector2 dustVel = directionToMouse.RotatedByRandom(MathHelper.PiOver4 * 0.3f) * Main.rand.Next(7, 12);
                Dust.NewDust(muzzlePosition, 0, 0, DustID.MinecartSpark, dustVel.X, dustVel.Y, Scale: 5);

                dustVel = directionToMouse.RotatedByRandom(MathHelper.PiOver4 * 0.4f) * Main.rand.Next(7, 9);
                Dust.NewDust(muzzlePosition, Projectile.width, Projectile.height, ModContent.DustType<LeverGunDust>(), dustVel.X, dustVel.Y, Scale: Main.rand.NextFloat(0.8f, 1.3f));
            }

            for (int i = 0; i < 20; i++)
            {
                Vector2 position = muzzlePosition + Main.rand.NextVector2Unit() * 4 - directionToMouse * 15;
                Vector2 vel = muzzlePosition.DirectionTo(muzzlePosition) * 3;
                Dust.NewDust(position, 0, 0, DustID.MinecartSpark, vel.X, vel.Y, Scale: Main.rand.NextFloat(2.6f, 4.7f));
            }

            // recoil move player
            //Player.velocity -= dirToMouse * 2f;

            float lightAmount = 1.35f;
            Lighting.AddLight(muzzlePosition, 1f * lightAmount, 0.9f * lightAmount, 0.7f * lightAmount);
        }

        Vector2 Center => Player.RotatedRelativePoint(Player.MountedCenter) + new Vector2(Player.direction * -3, -3);
        float RotationToMouse(float rotationToMouse) => rotationToMouse + 0.07f * Player.direction + (Player.direction == -1 ? MathHelper.Pi : 0);

        Player Player => Main.player[Projectile.owner];
        bool playedSound;
        float recoilRot;
        float recoilOffsetX;
        float animationProgress;
        bool spawnedCasings;
        Vector2 directionToMouse;
        public override void AI()
        {
            if (Player.ItemAnimationEndingOrEnded)
            {
                Projectile.Kill();
                return;
            }

            if (Main.myPlayer == Player.whoAmI)
                directionToMouse = Center.DirectionTo(Main.MouseWorld);

            float rotationToMouse = directionToMouse.ToRotation();

            Projectile.Center = Center + directionToMouse * 10;
            Player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, rotationToMouse - MathHelper.PiOver2);

            Player.heldProj = Projectile.whoAmI;
            Player.direction = initialDirection;

            animationProgress = (float)(Player.itemAnimationMax - Player.itemAnimation) / Player.itemAnimationMax;

            Projectile.rotation = 0;
            float rotateAfter = 0.40f;
            float stopRotBefore = 0.65f;
            if (animationProgress >= rotateAfter && animationProgress < stopRotBefore)
            {
                if (!spawnedCasings && Main.netMode != NetmodeID.Server)
                {
                    Gore.NewGore(Projectile.GetSource_FromThis(), Center + directionToMouse * 15, -2.5f * Vector2.UnitY - 2 * Vector2.UnitX * Player.direction, Mod.Find<ModGore>("LeverActionShotgunCasing").Type);
                    spawnedCasings = true;
                }
                Projectile.rotation = -(MathHelper.TwoPi * ((animationProgress - rotateAfter) / (stopRotBefore - rotateAfter))) * Player.direction;
            }
            else
            {
                if (!playedSound && animationProgress > rotateAfter - 0.20f)
                {
                    SoundEngine.PlaySound(SoundID.Item149 with { Pitch = Main.rand.NextFloat(0.3f) }, Projectile.Center);
                    playedSound = true;
                }

                if (animationProgress > 0.008f)
                {
                    recoilRot *= 0.75f;
                    recoilOffsetX *= 0.8f;
                }
                else
                {
                    recoilRot -= 0.4f * Player.direction;
                    recoilOffsetX += 9f * Player.direction;
                }
            }

            Projectile.rotation += RotationToMouse(rotationToMouse) + recoilRot;
            
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(directionToMouse.X);
            writer.Write(directionToMouse.Y);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            directionToMouse.X = reader.ReadSingle();
            directionToMouse.Y = reader.ReadSingle();
        }

        public override bool ShouldUpdatePosition() => false;

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = TextureAssets.Projectile[Type].Value;

            Vector2 origin = (Player.direction == -1 ? new Vector2(tex.Width - 6, 8) : new Vector2(6, 8)) + Vector2.UnitX * recoilOffsetX;

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
                Projectile.Center + rotDir * 30 * Player.direction - rotDir.RotatedBy(MathHelper.PiOver2) * 2f - Main.screenPosition,
                null,
                Color.Orange * mult * 0.27f,
                Projectile.rotation,
                glowTex.Size() * 0.5f + Vector2.UnitX * recoilOffsetX,
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
