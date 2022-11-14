using AuroraMod.Common.Utils;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace AuroraMod.Content.Items.Weapons.RangedWeapons
{
    public class LeverActionShotgunBulletProjectile : ModProjectile
    {
        public override void SetStaticDefaults()
        {
			ProjectileID.Sets.TrailCacheLength[Type] = 3;
            ProjectileID.Sets.TrailingMode[Type] = 2;
		}

        public override void SetDefaults()
		{
			Projectile.width = 8;
			Projectile.height = 8;
			Projectile.aiStyle = 1;
			Projectile.friendly = true;
			Projectile.hostile = false;
			Projectile.DamageType = DamageClass.Ranged;
			Projectile.penetrate = 1;
			Projectile.timeLeft = 600;
			Projectile.alpha = 60;
			Projectile.light = 0.15f;
			Projectile.ignoreWater = true;
			Projectile.tileCollide = true;
			Projectile.extraUpdates = 2;

			AIType = ProjectileID.Bullet;
		}

        public override void OnSpawn(IEntitySource source)
        {
			Projectile.scale = Main.rand.NextFloat(0.6f, 1.1f);
        }

        public override void Kill(int timeLeft)
        {
			for (int i = 0; i < Main.rand.Next(2, 3); i++)
			{
				Vector2 dustVel = (-Projectile.velocity * 0.7f).RotatedByRandom(0.3f);
				Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<Dusts.LeverGunDust>(), dustVel.X, dustVel.Y, Scale: Main.rand.NextFloat(0.5f, 1.2f));
			}

			SoundEngine.PlaySound(SoundID.Item118, Projectile.Center);
		}

        public override bool PreDraw(ref Color lightColor)
        {
			Projectile.EasyDrawAfterImage(Color.White * 0.3f);

			lightColor = Color.White;

            return base.PreDraw(ref lightColor);
        }
    }
}
