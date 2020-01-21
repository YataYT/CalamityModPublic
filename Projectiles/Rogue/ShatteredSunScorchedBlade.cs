﻿using CalamityMod.Buffs.DamageOverTime;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Rogue
{
    public class ShatteredSunScorchedBlade : ModProjectile
    {
        int counter = 0;
        float multiplier = 1f;
        bool stealthOrigin = false;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Scorched Blade");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 4;
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            projectile.width = 56;
            projectile.height = 56;
            projectile.friendly = true;
            projectile.ignoreWater = true;
            projectile.extraUpdates = 1;
            projectile.penetrate = 1;
            projectile.Calamity().rogue = true;
            projectile.tileCollide = false;
            projectile.timeLeft = 500;
        }

        public override void AI()
        {
            counter++;
            if (counter == 1)
            {
                stealthOrigin = projectile.ai[0] == 1f;
                projectile.alpha += (int) projectile.ai[1];
                projectile.ai[0] = 0f;
            }
            if (counter == 20 && !projectile.Calamity().stealthStrike && !stealthOrigin)
            {
                projectile.tileCollide = true;
            }
            if (counter % 5 == 0)
            {
                projectile.velocity *= 1.15f;
            }
            if (counter % 10 == 0)
            {
                multiplier -= 0.005f;
                if (multiplier >= 0.5f && !stealthOrigin && projectile.alpha < 200)
                    projectile.alpha += Main.rand.Next(5, 7);
            }
            if (counter % 9 == 0 || (counter % 5 == 0 && projectile.Calamity().stealthStrike))
            {
                int timesToSpawnDust = projectile.Calamity().stealthStrike  ? 2 : 1;
                for (int i = 0; i < timesToSpawnDust; i++)
                {
                    int num624 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 127, 0f, 0f, 100, default, projectile.Calamity().stealthStrike ? 1.8f : 1.3f);
                    Main.dust[num624].noGravity = true;
                    Main.dust[num624].velocity *= 5f;
                    num624 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 127, 0f, 0f, 100, default, projectile.Calamity().stealthStrike ? 1.8f : 1.3f);
                    Main.dust[num624].velocity *= 2f;
                }
            }

            projectile.rotation = (float)Math.Atan2((double)projectile.velocity.Y, (double)projectile.velocity.X) + 2.355f;
            if (projectile.spriteDirection == -1)
            {
                projectile.rotation -= 1.57f;
            }

            Lighting.AddLight(projectile.Center, 0.7f, 0.3f, 0f);
            float num472 = projectile.Center.X;
            float num473 = projectile.Center.Y;
            float num474 = 400f;
            bool flag17 = false;
            for (int num475 = 0; num475 < 200; num475++)
            {
                if (Main.npc[num475].CanBeChasedBy(projectile, false) && Collision.CanHit(projectile.Center, 1, 1, Main.npc[num475].Center, 1, 1))
                {
                    float num476 = Main.npc[num475].position.X + (float)(Main.npc[num475].width / 2);
                    float num477 = Main.npc[num475].position.Y + (float)(Main.npc[num475].height / 2);
                    float num478 = Math.Abs(projectile.position.X + (float)(projectile.width / 2) - num476) + Math.Abs(projectile.position.Y + (float)(projectile.height / 2) - num477);
                    if (num478 < num474)
                    {
                        num474 = num478;
                        num472 = num476;
                        num473 = num477;
                        flag17 = true;
                    }
                }
            }
            if (flag17)
            {
                float num483 = 20f;
                Vector2 vector35 = new Vector2(projectile.position.X + (float)projectile.width * 0.5f, projectile.position.Y + (float)projectile.height * 0.5f);
                float num484 = num472 - vector35.X;
                float num485 = num473 - vector35.Y;
                float num486 = (float)Math.Sqrt((double)(num484 * num484 + num485 * num485));
                num486 = num483 / num486;
                num484 *= num486;
                num485 *= num486;
                projectile.velocity.X = (projectile.velocity.X * 20f + num484) / 21f;
                projectile.velocity.Y = (projectile.velocity.Y * 20f + num485) / 21f;
            }
            float num633 = 700f;
            Vector2 vector46 = projectile.position;
            bool flag25 = false;
            for (int num645 = 0; num645 < 200; num645++)
            {
                NPC nPC2 = Main.npc[num645];
                if (nPC2.CanBeChasedBy(projectile, false))
                {
                    float num646 = Vector2.Distance(nPC2.Center, projectile.Center);
                    if (!flag25)
                    {
                        num633 = num646;
                        vector46 = nPC2.Center;
                        flag25 = true;
                    }
                }
            }
            if (flag25 && projectile.ai[0] == 0f)
            {
                Vector2 vector47 = vector46 - projectile.Center;
                float num648 = vector47.Length();
                vector47.Normalize();
                if (num648 > 200f)
                {
                    float scaleFactor2 = 8f;
                    vector47 *= scaleFactor2;
                    projectile.velocity = (projectile.velocity * 40f + vector47) / 41f;
                }
                else
                {
                    float num649 = 4f;
                    vector47 *= -num649;
                    projectile.velocity = (projectile.velocity * 40f + vector47) / 41f;
                }
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            CalamityGlobalProjectile.DrawCenteredAndAfterimage(projectile, lightColor, ProjectileID.Sets.TrailingMode[projectile.type], 1);
            return false;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            if (multiplier < 0.5f)
                multiplier = 0.5f;
            damage = stealthOrigin ? damage : (int)((double)damage * multiplier);
            if (projectile.Calamity().stealthStrike)
            {
                int numProj = 2;
                float rotation = MathHelper.ToRadians(10);
                if (projectile.owner == Main.myPlayer)
                {
                    Player owner = Main.player[projectile.owner];
                    Vector2 correctedVelocity = target.Center - owner.Center;
                    correctedVelocity.Normalize();
                    correctedVelocity *= 10f;
                    int spread = 6;
                    for (int i = 0; i < numProj; i++)
                    {
                        Vector2 perturbedspeed = new Vector2(correctedVelocity.X, correctedVelocity.Y + Main.rand.Next(-3, 4)).RotatedBy(MathHelper.ToRadians(spread));
                        
                        int proj = Projectile.NewProjectile(owner.Center.X, owner.Center.Y - 10, perturbedspeed.X, perturbedspeed.Y, ModContent.ProjectileType<ShatteredSunScorchedBlade>(), (int)((double)projectile.damage * 0.6), 1f, projectile.owner, 1f, projectile.alpha);
                        spread -= Main.rand.Next(2, 6);
                        Main.projectile[proj].ai[0] = 1f;
                    }
                    projectile.Kill();
                }
            }
            Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, 0f, 0f, ModContent.ProjectileType<ShatteredExplosion>(), (int)((double)damage * 0.15), projectile.knockBack, projectile.owner, 1f, 0f);
            target.AddBuff(ModContent.BuffType<HolyFlames>(), 180);
        }

        public override void OnHitPvp(Player target, int damage, bool crit)
        {
            if (multiplier < 0.5f)
                multiplier = 0.5f;
            damage = stealthOrigin ? damage : (int)((double)damage * multiplier);
            if (projectile.Calamity().stealthStrike)
            {
                int numProj = 2;
                float rotation = MathHelper.ToRadians(10);
                if (projectile.owner == Main.myPlayer)
                {
                    Player owner = Main.player[projectile.owner];
                    Vector2 correctedVelocity = target.Center - owner.Center;
                    correctedVelocity.Normalize();
                    correctedVelocity *= 10f;
                    int spread = 6;
                    for (int i = 0; i < numProj; i++)
                    {
                        Vector2 perturbedspeed = new Vector2(correctedVelocity.X, correctedVelocity.Y + Main.rand.Next(-3, 4)).RotatedBy(MathHelper.ToRadians(spread));
                        
                        int proj = Projectile.NewProjectile(owner.Center.X, owner.Center.Y - 10, perturbedspeed.X, perturbedspeed.Y, ModContent.ProjectileType<ShatteredSunScorchedBlade>(), (int)((double)projectile.damage * 0.55), 1f, projectile.owner, 0f, 0f);
                        spread -= Main.rand.Next(2, 6);
                        Main.projectile[proj].ai[0] = 1f;
                    }
                    projectile.Kill();
                }
            }
            Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, 0f, 0f, ModContent.ProjectileType<ShatteredExplosion>(), (int)((double)damage * 0.15), projectile.knockBack, projectile.owner, 1f, 0f);
            target.AddBuff(ModContent.BuffType<HolyFlames>(), 180);
        }

        public override void Kill(int timeLeft)
        {

            Main.PlaySound(2, (int)projectile.position.X, (int)projectile.position.Y, 14);
            projectile.position.X = projectile.position.X + (float)(projectile.width / 2);
            projectile.position.Y = projectile.position.Y + (float)(projectile.height / 2);
            projectile.width = projectile.height = 200;
            projectile.position.X = projectile.position.X - (float)(projectile.width / 2);
            projectile.position.Y = projectile.position.Y - (float)(projectile.height / 2);
            for (int num621 = 0; num621 < 4; num621++)
            {
                int num622 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 244, 0f, 0f, 100, default, 2f);
                Main.dust[num622].velocity *= 3f;
                if (Main.rand.NextBool(2))
                {
                    Main.dust[num622].scale = 0.5f;
                    Main.dust[num622].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
                }
            }
            for (int num623 = 0; num623 < 12; num623++)
            {
                int num624 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 244, 0f, 0f, 100, default, 3f);
                Main.dust[num624].noGravity = true;
                Main.dust[num624].velocity *= 5f;
                num624 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 244, 0f, 0f, 100, default, 2f);
                Main.dust[num624].velocity *= 2f;

            }
            for (int num625 = 0; num625 < 3; num625++)
            {
                float scaleFactor10 = 0.33f;
                if (num625 == 1)
                {
                    scaleFactor10 = 0.66f;
                }
                if (num625 == 2)
                {
                    scaleFactor10 = 1f;
                }
                int num626 = Gore.NewGore(new Vector2(projectile.position.X + (float)(projectile.width / 2) - 24f, projectile.position.Y + (float)(projectile.height / 2) - 24f), default, Main.rand.Next(61, 64), 1f);
                Gore gore = Main.gore[num626];
                gore.velocity *= scaleFactor10;
                gore.velocity.X += 1f;
                gore.velocity.Y += 1f;
                num626 = Gore.NewGore(new Vector2(projectile.position.X + (float)(projectile.width / 2) - 24f, projectile.position.Y + (float)(projectile.height / 2) - 24f), default, Main.rand.Next(61, 64), 1f);
                gore.velocity *= scaleFactor10;
                gore.velocity.X -= 1f;
                gore.velocity.Y += 1f;
                num626 = Gore.NewGore(new Vector2(projectile.position.X + (float)(projectile.width / 2) - 24f, projectile.position.Y + (float)(projectile.height / 2) - 24f), default, Main.rand.Next(61, 64), 1f);
                gore.velocity *= scaleFactor10;
                gore.velocity.X += 1f;
                gore.velocity.Y -= 1f;
                num626 = Gore.NewGore(new Vector2(projectile.position.X + (float)(projectile.width / 2) - 24f, projectile.position.Y + (float)(projectile.height / 2) - 24f), default, Main.rand.Next(61, 64), 1f);
                gore.velocity *= scaleFactor10;
                gore.velocity.X -= 1f;
                gore.velocity.Y -= 1f;
            }
        }
    }
}
