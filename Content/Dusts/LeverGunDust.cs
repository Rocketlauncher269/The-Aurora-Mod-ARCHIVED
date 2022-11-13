using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AuroraMod.Content.Dusts
{
    public class LeverGunDust : ModDust
    {
        public override void OnSpawn(Dust dust)
        {
            dust.frame = new Rectangle(0, Main.rand.Next(3) * 8, 8, 8);
        }

        public override bool Update(Dust dust)
        {
            dust.scale -= 0.06f;

            dust.velocity *= 0.9f;

            dust.velocity.Y += 0.16f;
            dust.position += dust.velocity;

            dust.color = Color.Lerp(dust.color, Color.Black, 0.01f);

            Lighting.AddLight(dust.position, 0.1f, 0.05f, 0f);

            if (dust.scale <= 0)
            {
                dust.active = false;
            }

            return false;
        }
    }
}
