﻿using System;
using CalamityMod.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Ranged
{
    public class DeadSunExplosion : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Ranged";
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";
        public Player Owner => Main.player[Projectile.owner];
        public ref float ExplosionRadius => ref Projectile.ai[0];

        public override void SetDefaults()
        {
            //These shouldn't matter because its circular
            Projectile.width = 100;
            Projectile.height = 100;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 2;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
        }
        public override void AI()
        {
            Particle explosion = new DetailedExplosion(Projectile.Center, Vector2.Zero, Color.Turquoise, new Vector2(1f, 1f), Main.rand.NextFloat(-5, 5), 0f, ExplosionRadius * 0.0065f + 0.1f, Main.rand.Next(15, 22));
            GeneralParticleHandler.SpawnParticle(explosion);
            Particle explosion2 = new DetailedExplosion(Projectile.Center, Vector2.Zero, Color.MediumSpringGreen, new Vector2(1f, 1f), Main.rand.NextFloat(-5, 5), 0f, ExplosionRadius * 0.0045f + 0.1f, Main.rand.Next(15, 22));
            GeneralParticleHandler.SpawnParticle(explosion2);
            Particle orb = new GenericBloom(Projectile.Center, Projectile.velocity, Color.Turquoise, ExplosionRadius * 0.0085f + 0.05f, 10, true);
            GeneralParticleHandler.SpawnParticle(orb);
            Particle orb2 = new GenericBloom(Projectile.Center, Projectile.velocity, Color.MediumSpringGreen, ExplosionRadius * 0.006f + 0.05f, 10, true);
            GeneralParticleHandler.SpawnParticle(orb2);
            float numberOfDusts = ExplosionRadius * 0.1f + 10;
            float rotFactor = 360f / numberOfDusts;
            for (int i = 0; i < numberOfDusts; i++)
            {
                float rot = MathHelper.ToRadians(i * rotFactor);
                Vector2 offset = new Vector2(Main.rand.NextFloat(ExplosionRadius * 0.2f, 3.1f), 0).RotatedBy(rot * Main.rand.NextFloat(1.1f, 9.1f));
                Vector2 velOffset = new Vector2(Main.rand.NextFloat(ExplosionRadius * 0.2f, 3.1f), 0).RotatedBy(rot * Main.rand.NextFloat(1.1f, 9.1f));
                Dust dust = Dust.NewDustPerfect(Projectile.Center + offset, Main.rand.NextBool(4) ? 229 : Projectile.ai[1] == 5 ? 226 : 156, new Vector2(velOffset.X, velOffset.Y));
                dust.noGravity = dust.type == 226 ? false : true;
                dust.color = dust.type == 226 ? Color.Turquoise : default;
                dust.velocity = velOffset;
                dust.scale = dust.type == 226 ? Main.rand.NextFloat(0.7f, 1.3f) : Main.rand.NextFloat(1.6f, 2.2f);
            }
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) => CalamityUtils.CircularHitboxCollision(Projectile.Center, ExplosionRadius + 20, targetHitbox);
    }
}
