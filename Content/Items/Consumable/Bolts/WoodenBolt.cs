using AuroraMod.Common.Bases.Items;
using AuroraMod.Common.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace AuroraMod.Content.Items.Consumable.Bolts
{
    public class WoodenBolt : BoltItem
    {
        protected override int ProjectileBolt => ModContent.ProjectileType<WoodenBoltProjectile>();

        protected override int Damage => 15;

        protected override int CritChance => 23;

        protected override int Knockback => 5;
    }

    public class WoodenBoltProjectile : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_" + ProjectileID.Stynger;
        public override void SetDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.aiStyle = -1;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 600;
            Projectile.alpha = 60;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
        }

        public override void AI()
        {
            Projectile.velocity.Y += 0.85f / MathF.Abs(Projectile.velocity.X);
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Type].Value;
            Projectile.EasyDraw(lightColor, origin: new Vector2(texture.Width * 0.5f, 10));
            return false;
        }
    }
}
