﻿using System;
using System.IO;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Boss
{
	public class YharonFireball : ModProjectile
	{
		private float speedX = -3f;
		private float speedX2 = -5f;

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Dragon Fireball");
			ProjectileID.Sets.TrailCacheLength[projectile.type] = 20;
			ProjectileID.Sets.TrailingMode[projectile.type] = 2;
		}

		public override void SetDefaults()
		{
			projectile.width = 30;
			projectile.height = 30;
			projectile.hostile = true;
			projectile.alpha = 255;
			projectile.penetrate = -1;
			projectile.timeLeft = 120;
			projectile.aiStyle = 1;
			aiType = 686;
			cooldownSlot = 1;
		}

		public override void SendExtraAI(BinaryWriter writer)
		{
			writer.Write(speedX);
			writer.Write(projectile.localAI[1]);
			writer.Write(speedX2);
		}

		public override void ReceiveExtraAI(BinaryReader reader)
		{
			speedX = reader.ReadSingle();
			projectile.localAI[1] = reader.ReadSingle();
			speedX2 = reader.ReadSingle();
		}

		public override Color? GetAlpha(Color lightColor)
		{
			return new Color(255, Main.DiscoG, 53, projectile.alpha);
		}

		public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
		{
			Texture2D texture = Main.projectileTexture[projectile.type];
			int frameHeight = texture.Height / Main.projFrames[projectile.type];
			int frameY = frameHeight * projectile.frame;
			Microsoft.Xna.Framework.Rectangle rectangle = new Microsoft.Xna.Framework.Rectangle(0, frameY, texture.Width, frameHeight);
			SpriteEffects spriteEffects = SpriteEffects.None;
			if (projectile.spriteDirection == -1)
				spriteEffects = SpriteEffects.FlipHorizontally;

			for (int i = 0; i < projectile.oldPos.Length; i++)
			{
				Vector2 drawPos = projectile.oldPos[i] + projectile.Size / 2f - Main.screenPosition + new Vector2(0f, projectile.gfxOffY);
				Color color2 = projectile.GetAlpha(lightColor) * ((float)(projectile.oldPos.Length - i) / (float)projectile.oldPos.Length);
				Main.spriteBatch.Draw(texture, drawPos, new Microsoft.Xna.Framework.Rectangle?(rectangle), color2, projectile.rotation, rectangle.Size() / 2f, projectile.scale, spriteEffects, 0f);
			}

			Main.spriteBatch.Draw(texture, projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY),
				new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(0, frameY, texture.Width, frameHeight)),
				projectile.GetAlpha(lightColor), projectile.rotation, new Vector2((float)texture.Width / 2f, (float)frameHeight / 2f), projectile.scale, spriteEffects, 0f);

			return false;
		}

		public override void Kill(int timeLeft)
		{
			if (projectile.owner == Main.myPlayer)
			{
				for (int x = 0; x < 3; x++)
				{
					Projectile.NewProjectile((int)projectile.Center.X, (int)projectile.Center.Y, speedX, -50f, mod.ProjectileType("YharonFireball2"), projectile.damage, 0f, Main.myPlayer, 0f, 0f);
					speedX += 3f;
				}
				for (int x = 0; x < 2; x++)
				{
					Projectile.NewProjectile((int)projectile.Center.X, (int)projectile.Center.Y, speedX2, -75f, mod.ProjectileType("YharonFireball2"), projectile.damage, 0f, Main.myPlayer, 0f, 0f);
					speedX2 += 10f;
				}
			}
			projectile.position = projectile.Center;
			projectile.width = (projectile.height = 144);
			projectile.position.X = projectile.position.X - (float)(projectile.width / 2);
			projectile.position.Y = projectile.position.Y - (float)(projectile.height / 2);
			for (int num193 = 0; num193 < 2; num193++)
			{
				Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 55, 0f, 0f, 50, default(Color), 1.5f);
			}
			for (int num194 = 0; num194 < 20; num194++)
			{
				int num195 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 55, 0f, 0f, 0, default(Color), 2.5f);
				Main.dust[num195].noGravity = true;
				Main.dust[num195].velocity *= 3f;
				num195 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 55, 0f, 0f, 50, default(Color), 1.5f);
				Main.dust[num195].velocity *= 2f;
				Main.dust[num195].noGravity = true;
			}
		}
	}
}
