using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Dusts;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Rogue
{
	public class ToxicantTwisterTwoPointZero : ModProjectile
    {
		private int lifeTime = 300;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Toxicant Twister");
        }

        public override void SetDefaults()
        {
            projectile.width = 42;
            projectile.height = 46;
            projectile.friendly = true;
            projectile.tileCollide = false;
            projectile.penetrate = 5;
            projectile.timeLeft = lifeTime;
            projectile.Calamity().rogue = true;
			projectile.extraUpdates = 1;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 12;
        }

        public override void AI()
        {
            if (projectile.Calamity().stealthStrike)
            {
                if (projectile.timeLeft % 50f == 0f)
                {
                    for (int i = 0; i < 2; i++)
                    {
                        Projectile.NewProjectile(projectile.Center,
                            (-1f * projectile.velocity).RotatedByRandom(0.1f) * 0.6f,
                            ModContent.ProjectileType<ToxicantTwisterDust>(), projectile.damage, 0f, projectile.owner);
                    }
                }
                projectile.rotation += 0.06f * (projectile.velocity.X > 0).ToDirectionInt();
            }

            // Boomerang rotation
            projectile.rotation += 0.4f * (float)projectile.direction;

            // Boomerang sound
            if (projectile.soundDelay == 0)
            {
                projectile.soundDelay = 8;
                Main.PlaySound(SoundID.Item7, projectile.position);
            }

            // Returns after 2.5 seconds in the air
            if (projectile.timeLeft < lifeTime - 150)
                projectile.ai[0] = 1f;

            if (projectile.ai[0] != 0f)
            {
                float returnSpeed = 30f;
                float acceleration = 1.4f;

                Player owner = Main.player[projectile.owner];

                // Delete the projectile if it's excessively far away.
				Vector2 projVector = owner.Center - projectile.Center;
                float dist = projVector.Length();
                if (dist > 3000f)
                    projectile.Kill();

                dist = returnSpeed / dist;
                projVector.X *= dist;
                projVector.Y *= dist;

                // Home back in on the player.
                if (projectile.velocity.X < projVector.X)
                {
                    projectile.velocity.X += acceleration;
                    if (projectile.velocity.X < 0f && projVector.X > 0f)
                        projectile.velocity.X += acceleration;
                }
                else if (projectile.velocity.X > projVector.X)
                {
                    projectile.velocity.X -= acceleration;
                    if (projectile.velocity.X > 0f && projVector.X < 0f)
                        projectile.velocity.X -= acceleration;
                }
                if (projectile.velocity.Y < projVector.Y)
                {
                    projectile.velocity.Y += acceleration;
                    if (projectile.velocity.Y < 0f && projVector.Y > 0f)
                        projectile.velocity.Y += acceleration;
                }
                else if (projectile.velocity.Y > projVector.Y)
                {
                    projectile.velocity.Y -= acceleration;
                    if (projectile.velocity.Y > 0f && projVector.Y < 0f)
                        projectile.velocity.Y -= acceleration;
                }

                // Delete the projectile if it touches its owner.
                if (Main.myPlayer == projectile.owner)
                    if (projectile.Hitbox.Intersects(owner.Hitbox))
                        projectile.Kill();
            }
			else
			{
				CalamityGlobalProjectile.HomeInOnNPC(projectile, false, 600f, 20f, 20f);
			}
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
			if (projectile.penetrate < 3)
				projectile.ai[0] = 1f;
            target.AddBuff(ModContent.BuffType<SulphuricPoisoning>(), 180);
            Main.PlaySound(SoundID.Item20, projectile.position);
            for (int k = 0; k < 10; k++)
            {
                Dust.NewDust(projectile.position + projectile.velocity, 
                    projectile.width, projectile.height, (int)CalamityDusts.SulfurousSeaAcid,
                    projectile.oldVelocity.X * 0.5f, projectile.oldVelocity.Y * 0.5f);
            }
        }

        public override void OnHitPvp(Player target, int damage, bool crit)
        {
			if (projectile.penetrate < 3)
				projectile.ai[0] = 1f;
            target.AddBuff(ModContent.BuffType<SulphuricPoisoning>(), 180);
            Main.PlaySound(SoundID.Item20, projectile.position);
            for (int k = 0; k < 10; k++)
            {
                Dust.NewDust(projectile.position + projectile.velocity,
                    projectile.width, projectile.height, (int)CalamityDusts.SulfurousSeaAcid,
                    projectile.oldVelocity.X * 0.5f, projectile.oldVelocity.Y * 0.5f);
            }
        }
    }
}
