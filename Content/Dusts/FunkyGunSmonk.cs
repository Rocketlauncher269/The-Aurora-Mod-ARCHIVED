using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AuroraMod.Content.Dusts
{
    public class FunkyGunSmonk : ModDust
    {
        public override string Texture => "Terraria/Images/Gore_" + GoreID.Smoke3;

        public override void OnSpawn(Dust dust)
        {
            dust.color = Color.OrangeRed;
        }

        public override bool Update(Dust dust)
        {
            dust.velocity *= 0.97f;
            dust.position += dust.velocity;

            dust.scale *= 0.97f;

            if (dust.scale < 0.005f)
            {
                dust.active = false;
            }

            return false;
        }
    }
}
