using AuroraMod.Common.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AuroraMod.Content.Items.AbilityItems
{
    public class ChainHookProjectile : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 13;
            Projectile.height = 13;
            Projectile.aiStyle = -1;
            Projectile.DamageType = DamageClass.MeleeNoSpeed;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
            Projectile.timeLeft = MAX_FLY_TIME + MAX_POST_FLY_TIME + 10;
            Projectile.netImportant = true;
            Projectile.penetrate = -1;

            Projectile.extraUpdates = 3;
        }

        Player Player => Main.player[Projectile.owner];
        Vector2 directionToPlayer;
        float distanceToPlayer;

        const int MAX_FLY_TIME = 17 * 3;
        const int MAX_POST_FLY_TIME = 15 * 3;
        ref float AITimer => ref Projectile.ai[0];
        public override void AI()
        {
            Projectile.rotation = (-directionToPlayer).ToRotation();

            if (AITimer > MAX_FLY_TIME)
            {
                Projectile.velocity = Vector2.Zero;
                Projectile.friendly = false;

                if (impaledTarget is null && AITimer < MAX_FLY_TIME + MAX_POST_FLY_TIME)
                    AITimer = MAX_FLY_TIME + MAX_POST_FLY_TIME;

                if (AITimer > MAX_FLY_TIME + MAX_POST_FLY_TIME)
                {
                    Projectile.Center = Vector2.Lerp(Projectile.Center, Player.Center, 0.08f);

                    if (distanceToPlayer < 30f)
                    {
                        Projectile.Kill();
                    }

                    if (distanceToPlayer < 40f)
                    {
                        impaledTarget = null;
                    }                    
                }

                if (impaledTarget is not null)
                {
                    impaledTarget.Center = Projectile.Center + targetCenterOffset;
                    impaledTarget.velocity = Vector2.Zero;
                }
            }

            AITimer++;
        }

        readonly static int[] impaleBlacklist = new int[]
        {
            NPCID.TargetDummy,
            NPCID.GolemFistLeft,
            NPCID.GolemFistRight,
            NPCID.GolemHead,
            NPCID.GolemHeadFree,
            NPCID.EaterofWorldsBody,
            NPCID.EaterofWorldsTail,
            NPCID.EaterofWorldsHead,
            NPCID.TheDestroyer,
            NPCID.TheDestroyerBody,
            NPCID.TheDestroyerTail,
            NPCID.WallofFleshEye,
            NPCID.MartianSaucerTurret,
            NPCID.PirateShipCannon,
            NPCID.WyvernBody,
            NPCID.WyvernBody2,
            NPCID.WyvernBody3,
            NPCID.WyvernHead,
            NPCID.WyvernLegs,
            NPCID.WyvernTail,
            NPCID.DiggerBody,
            NPCID.DiggerHead,
            NPCID.DiggerTail
        };

        NPC impaledTarget;
        Vector2 targetCenterOffset;
        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            if (target.life > 0f && target.active && !target.boss && !impaleBlacklist.Contains(target.type))
            {
                impaledTarget = target;
                targetCenterOffset = impaledTarget.Center - Projectile.Center;

                impaledTarget.GetGlobalNPC<ChainedGlobalNPC>().Chained = true;
            }

            if (AITimer < MAX_FLY_TIME)
                AITimer = MAX_FLY_TIME;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (AITimer < MAX_FLY_TIME)
                AITimer = MAX_FLY_TIME;
            return false;
        }

        //ref float ChainSinMult => ref Projectile.ai[1];
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = ModContent.Request<Texture2D>(Texture.Replace("Projectile", "Link"), ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;

            Vector2 currentPosition = Projectile.Center + directionToPlayer;
            Vector2 directionToLast = currentPosition.DirectionTo(Projectile.Center);

            directionToPlayer = Projectile.Center.DirectionTo(Player.Center);
            distanceToPlayer = Projectile.Distance(Player.Center);

            Vector2 lastPosition = currentPosition;

            float maxLinks = distanceToPlayer / tex.Height;
            for (int i = 0; i < maxLinks; i++)
            {
                float iMult = MathF.Sin((float)((int)maxLinks - i) / (int)maxLinks * MathHelper.Pi);
                Vector2 sinCurve = Vector2.UnitY.RotatedBy(directionToPlayer.ToRotation()) * MathF.Sin((i + Main.GameUpdateCount) * 0.4f) * iMult * 5
                   * (AITimer > MAX_FLY_TIME ? AITimer > MAX_FLY_TIME + MAX_POST_FLY_TIME ? MAX_POST_FLY_TIME * 0.4f / ((AITimer - 5 - MAX_FLY_TIME) * 2) : (AITimer - MAX_FLY_TIME) * 0.4f : 1) / (Projectile.extraUpdates + 1);

                directionToLast = (currentPosition + sinCurve).DirectionTo(lastPosition);

                Vector2 drawPos = currentPosition + sinCurve;

                Main.spriteBatch.Draw(
                    tex,
                    drawPos - Main.screenPosition,
                    null,
                    lightColor,
                    directionToLast.ToRotation() + MathHelper.PiOver2,
                    tex.Size() * 0.5f,
                    1,
                    SpriteEffects.None,
                    0
                    );

                //if (i == (int)maxLinks)Main.NewText(sinCurve);
                lastPosition = drawPos;
                currentPosition += directionToPlayer * tex.Height;
            }

            Projectile.EasyDraw(lightColor, rotation: Projectile.rotation + MathHelper.PiOver2);

            return false;
        }
    }

    public class ChainedGlobalNPC : GlobalNPC
    {
        public override bool InstancePerEntity => true;

        public bool Chained;

        const float DAMAGA_MULT = 1.5f;
        public override void ModifyHitByProjectile(NPC npc, Projectile projectile, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            if (Chained)
            {
                damage = (int)(damage * DAMAGA_MULT);
                Chained = false;
            }
        }

        public override void ModifyHitByItem(NPC npc, Player player, Item item, ref int damage, ref float knockback, ref bool crit)
        {
            if (Chained)
            {
                damage = (int)(damage * DAMAGA_MULT);
                Chained = false;
            }
        }
    }
}