﻿using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using System.IO;
using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.Items.Potions;
using CalamityMod.Items.Pets;
using CalamityMod.Projectiles.Boss;
using CalamityMod.World;
using CalamityMod.Balancing;

namespace CalamityMod.NPCs.Other
{
    // I would like to say first and foremost, that 95% of this is directly ported from his code when he got removed in early 2018
    public class THELORDE : ModNPC
    {
        public int aiSwitchCounter = 420;
        public int deathrayCounter = 0;
        public int invincibleCounter = 0;

        public bool ajitPaiDidNothingWrong = false; // christ this is old
        public bool canDespawn = false;
        public bool urAMemeNow = false;
        public bool hasBeenPlantera = false;
        public bool hasBeenGolem = false;

        public static readonly SoundStyle DeathSound = new("CalamityMod/Sounds/NPCKilled/Lordeath");

        public override void SetStaticDefaults()
        {
            NPCID.Sets.MPAllowedEnemies[Type] = true;
            NPCDebuffImmunityData debuffData = new NPCDebuffImmunityData { ImmuneToAllBuffsThatAreNotWhips = true };
            NPCID.Sets.DebuffImmunitySets.Add(Type, debuffData);
            NPCID.Sets.ShouldBeCountedAsBoss[Type] = true;
            this.HideFromBestiary();
        }

        public override void SetDefaults()
        {
            NPC.aiStyle = -1;
            NPC.damage = 69;
            NPC.width = 200;
            NPC.height = 200;
            NPC.defense = 100;
            NPC.lifeMax = 2500000;
            NPC.knockBackResist = 0f;
            NPC.value = Item.buyPrice(100, 0, 0, 0);
            NPC.HitSound = SoundID.NPCHit13;
            NPC.DeathSound = null;
            NPC.boss = true;
            Music = MusicID.LunarBoss;
            NPC.Calamity().canBreakPlayerDefense = true;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(NPC.dontTakeDamage);
            writer.Write(NPC.localAI[0]);
            writer.Write(NPC.localAI[1]);
            writer.Write(NPC.localAI[2]);
            writer.Write(NPC.localAI[3]);
            writer.Write(ajitPaiDidNothingWrong);
            writer.Write(canDespawn);
            writer.Write(urAMemeNow);
            writer.Write(hasBeenPlantera);
            writer.Write(hasBeenGolem);
            writer.Write(invincibleCounter);
            writer.Write(aiSwitchCounter);
            writer.Write(deathrayCounter);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            NPC.dontTakeDamage = reader.ReadBoolean();
            NPC.localAI[0] = reader.ReadSingle();
            NPC.localAI[1] = reader.ReadSingle();
            NPC.localAI[2] = reader.ReadSingle();
            NPC.localAI[3] = reader.ReadSingle();
            ajitPaiDidNothingWrong = reader.ReadBoolean();
            canDespawn = reader.ReadBoolean();
            urAMemeNow = reader.ReadBoolean();
            hasBeenPlantera = reader.ReadBoolean();
            hasBeenGolem = reader.ReadBoolean();
            invincibleCounter = reader.ReadInt32();
            aiSwitchCounter = reader.ReadInt32();
            deathrayCounter = reader.ReadInt32();
        }

        public override void OnSpawn(IEntitySource source)
        {
            // kill all existing non-hostile projectiles on spawn
            foreach (Projectile proj in Main.projectile)
            {
                if (proj.active && proj != null && !proj.hostile)
                {
                    proj.active = false;
                }
            }
        }

        public override void AI()
        {
            aiSwitchCounter++;
            if (ajitPaiDidNothingWrong && invincibleCounter < 6000)
            {
                invincibleCounter++;
            }
            NPC.alpha -= 100;
            if (NPC.alpha < 0)
            {
                NPC.alpha = 0;
            }
            if (Main.rand.NextBool(50))
            {
                // old was zombie 1 - zombie 62
                // ideally would be collaborative inferal screeches of various devs
                SoundEngine.PlaySound(Polterghast.Polterghast.creepySounds[Main.rand.Next(1, Polterghast.Polterghast.creepySounds.Count)] with { PitchVariance = 2}, NPC.position);
            }
            Player playerLOL = Main.player[NPC.target];
            playerLOL.velocity.X *= 0.99f;
            playerLOL.velocity.Y *= 0.99f;
            if (!DownedBossSystem.downedCalamitas && !DownedBossSystem.downedExoMechs)
            {
                if (Main.netMode != NetmodeID.Server)
                {
                    if (!playerLOL.dead && playerLOL.active)
                    {
                        playerLOL.AddBuff(ModContent.BuffType<NOU>(), 2);
                    }
                }
            }
            if (playerLOL.mount.Active)
            {
                playerLOL.mount.Dismount(playerLOL);
            }
            if (!playerLOL.active || playerLOL.dead)
            {
                NPC.TargetClosest(false);
                playerLOL = Main.player[NPC.target];
                if (!playerLOL.active || playerLOL.dead)
                {
                    canDespawn = true;
                    if (NPC.timeLeft > 10)
                    {
                        NPC.timeLeft = 10;
                    }
                }
            }
            else
            {
                canDespawn = false;
            }
            if (urAMemeNow)
            {
                NPC.rotation += 5f;
                NPC.TargetClosest(true);
                float num1372 = 69f;
                Vector2 vector167 = new Vector2(NPC.Center.X + (float)(NPC.direction * 20), NPC.Center.Y + 6f);
                float num1373 = playerLOL.position.X + (float)playerLOL.width * 0.5f - vector167.X;
                float num1374 = playerLOL.Center.Y - vector167.Y;
                float num1375 = (float)Math.Sqrt((double)(num1373 * num1373 + num1374 * num1374));
                float num1376 = num1372 / num1375;
                num1373 *= num1376;
                num1374 *= num1376;
                NPC.ai[0] -= 1f;
                if (num1375 < 50f || NPC.ai[0] > 0f)
                {
                    if (num1375 < 50f)
                    {
                        NPC.ai[0] = 20f;
                    }
                    if (NPC.velocity.X < 0f)
                    {
                        NPC.direction = -1;
                    }
                    else
                    {
                        NPC.direction = 1;
                    }
                    return;
                }
                NPC.velocity.X = (NPC.velocity.X * 50f + num1373) / 51f;
                NPC.velocity.Y = (NPC.velocity.Y * 50f + num1374) / 51f;
                if (num1375 < 150f)
                {
                    NPC.velocity.X = (NPC.velocity.X * 10f + num1373) / 11f;
                    NPC.velocity.Y = (NPC.velocity.Y * 10f + num1374) / 11f;
                }
                if (num1375 < 100f)
                {
                    NPC.velocity.X = (NPC.velocity.X * 7f + num1373) / 8f;
                    NPC.velocity.Y = (NPC.velocity.Y * 7f + num1374) / 8f;
                }
                return;
            }
            if (aiSwitchCounter >= 600)
            {
                int aiChoice = 1;
                switch (Main.rand.Next(33))
                {
                    case 0: aiChoice = NPCAIStyleID.Slime; NPC.noTileCollide = false; NPC.noGravity = false; aiSwitchCounter = 480; break; //slime
                    case 1: aiChoice = NPCAIStyleID.Flying; NPC.noTileCollide = false; NPC.noGravity = true; aiSwitchCounter = 480; break; //eater of souls
                    case 2: aiChoice = NPCAIStyleID.Caster; NPC.noTileCollide = false; NPC.noGravity = false; aiSwitchCounter = 480; break; //fire imp
                    case 3: aiChoice = NPCAIStyleID.CursedSkull; NPC.noTileCollide = true; NPC.noGravity = true; aiSwitchCounter = 480; break; //cursed skull
                    case 4: aiChoice = NPCAIStyleID.Bat; NPC.noTileCollide = false; NPC.noGravity = true; aiSwitchCounter = 480; break; //harpy
                    case 5: aiChoice = NPCAIStyleID.KingSlime; NPC.noTileCollide = false; NPC.noGravity = false; aiSwitchCounter = 420; break; //king slime
                    case 6: aiChoice = NPCAIStyleID.Vulture; NPC.noTileCollide = false; NPC.noGravity = true; aiSwitchCounter = 480; break; //vulture
                    case 7: aiChoice = NPCAIStyleID.Antlion; NPC.noTileCollide = false; NPC.noGravity = false; aiSwitchCounter = 480; break; //antlion
                    case 8: aiChoice = NPCAIStyleID.HoveringFighter; NPC.noTileCollide = false; NPC.noGravity = true; aiSwitchCounter = 480; break; //pixie
                    case 9: aiChoice = NPCAIStyleID.EnchantedSword; NPC.noTileCollide = true; NPC.noGravity = true; aiSwitchCounter = 480; break; //enchanted sword
                    case 10: aiChoice = NPCAIStyleID.Mimic; NPC.noTileCollide = false; NPC.noGravity = false; aiSwitchCounter = 480; break; //small mimic
                    case 11: aiChoice = NPCAIStyleID.Unicorn; NPC.noTileCollide = false; NPC.noGravity = false; aiSwitchCounter = 480; break; //unicorn
                    //case 12: aiChoice = NPCAIStyleID.GiantTortoise; NPC.noTileCollide = false; NPC.noGravity = false; aiSwitchCounter = 300; break; //giant tortoise
                    case 13: aiChoice = NPCAIStyleID.Herpling; NPC.noTileCollide = false; NPC.noGravity = false; aiSwitchCounter = 480; break; //herpling
                    case 14: aiChoice = NPCAIStyleID.QueenBee; NPC.noTileCollide = true; NPC.noGravity = true; aiSwitchCounter = 420; break; //queen bee
                    case 15: aiChoice = NPCAIStyleID.FlyingFish; NPC.noTileCollide = false; NPC.noGravity = true; aiSwitchCounter = 480; break; //flying fish
                    case 16: aiChoice = NPCAIStyleID.AngryNimbus; NPC.noTileCollide = false; NPC.noGravity = true; aiSwitchCounter = 480; break; //nimbus
                    case 17: aiChoice = NPCAIStyleID.DemonEye; NPC.noTileCollide = false; NPC.noGravity = true; aiSwitchCounter = 480; break; //demon eye
                    case 18: aiChoice = NPCAIStyleID.DungeonSpirit; NPC.noTileCollide = true; NPC.noGravity = true; aiSwitchCounter = 480; break; //dungeon spirit
                    case 19: aiChoice = NPCAIStyleID.DukeFishron; NPC.noTileCollide = true; NPC.noGravity = true; aiSwitchCounter = 0; break; //fishron
                    case 20: aiChoice = NPCAIStyleID.StarCell; NPC.noTileCollide = true; NPC.noGravity = true; aiSwitchCounter = 480; break; //nebula headcrab
                    case 21: aiChoice = NPCAIStyleID.GraniteElemental; NPC.noTileCollide = false; NPC.noGravity = true; aiSwitchCounter = 480; break; //granite elemental
                    case 22: aiChoice = NPCAIStyleID.FlowInvader; NPC.noTileCollide = false; NPC.noGravity = true; aiSwitchCounter = 480; break; //flow invader
                    case 23: aiChoice = NPCAIStyleID.NebulaFloater; NPC.noTileCollide = true; NPC.noGravity = true; aiSwitchCounter = 0; break; //nebula floater
                    case 24: aiChoice = NPCAIStyleID.Worm; NPC.noTileCollide = true; NPC.noGravity = true; aiSwitchCounter = 480; break; //WORM HELL YEAH BEST AI
                    case 25: aiChoice = NPCAIStyleID.Fighter; NPC.noTileCollide = false; NPC.noGravity = false; aiSwitchCounter = 480; break; //zombie
                    case 26: aiChoice = NPCAIStyleID.EyeOfCthulhu; NPC.noTileCollide = true; NPC.noGravity = true; aiSwitchCounter = 420; break; //eye of cthulhu
                    case 27: aiChoice = NPCAIStyleID.SkeletronHead; NPC.noTileCollide = true; NPC.noGravity = true; aiSwitchCounter = 420; break; //skeletron
                    case 28: aiChoice = NPCAIStyleID.Piranha; NPC.noTileCollide = false; NPC.noGravity = false; aiSwitchCounter = 480; break; //goldfish
                    case 29: aiChoice = NPCAIStyleID.Retinazer; NPC.noTileCollide = true; NPC.noGravity = true; aiSwitchCounter = 420; break; //retinazer
                    case 30: aiChoice = NPCAIStyleID.Spaazmatism; NPC.noTileCollide = true; NPC.noGravity = true; aiSwitchCounter = 420; break; //spazmatism
                    case 31: aiChoice = NPCAIStyleID.SkeletronPrimeHead; NPC.noTileCollide = true; NPC.noGravity = true; aiSwitchCounter = 420; break; //prime
                    case 32: aiChoice = NPCAIStyleID.QueenSlime; NPC.noTileCollide = true; NPC.noGravity = true; aiSwitchCounter = 420; break; //queen slime
                }
                if (Main.rand.NextBool(5) && !hasBeenPlantera)
                {
                    aiChoice = NPCAIStyleID.Plantera;
                    NPC.noTileCollide = true;
                    NPC.noGravity = true;
                    aiSwitchCounter = 0;
                    hasBeenPlantera = true;
                }
                if (Main.rand.NextBool(5) && !hasBeenGolem)
                {
                    aiChoice = NPCAIStyleID.GolemBody;
                    NPC.noTileCollide = false;
                    NPC.noGravity = false;
                    aiSwitchCounter = 0;
                    hasBeenGolem = true;
                }
                if (Vector2.Distance(playerLOL.Center, NPC.Center) > 4200f)
                {
                    urAMemeNow = true;
                    NPC.noTileCollide = true;
                    NPC.noGravity = true;
                }
                NPC.knockBackResist = 0f;
                NPC.dontTakeDamage = false;
                NPC.localAI[0] = 0f;
                NPC.localAI[1] = 0f;
                NPC.localAI[2] = 0f;
                NPC.localAI[3] = 0f;
                NPC.ai[0] = 0f;
                NPC.ai[1] = 0f;
                NPC.ai[2] = 0f;
                NPC.ai[3] = 0f;
                NPC.aiStyle = (urAMemeNow ? -1 : aiChoice);
                NPC.netUpdate = true;
                for (int i = 0; i < Main.maxProjectiles; i++)
                {
                    Projectile proj = Main.projectile[i];
                    if (proj.type == ModContent.ProjectileType<AresDeathBeamStart>() || proj.type == ModContent.ProjectileType<AresDeathBeamTelegraph>())
                    {
                        proj.Kill();
                    }
                }
                deathrayCounter = 0;
            }
            int totalProjectiles = 8;

            float radians = MathHelper.TwoPi / totalProjectiles;
            float velocity = 6f;
            double angleA = radians * 0.5;
            double angleB = MathHelper.ToRadians(90f) - angleA;
            float velocityX2 = (float)(velocity * Math.Sin(angleA) / Math.Sin(angleB));
            Vector2 spinningPoint = new Vector2(-velocityX2, -velocity);
            spinningPoint.Normalize();
            NPC.Calamity().newAI[2] = 200;

            // blender up during desperation or the entire fight if the pet is equipped
            if (playerLOL.Calamity().lordePet || ajitPaiDidNothingWrong)
            {
                if (deathrayCounter == 0)
                {
                    SoundEngine.PlaySound(Sounds.CommonCalamitySounds.LaserCannonSound, NPC.Center);
                    int type = ModContent.ProjectileType<AresDeathBeamTelegraph>();
                    Vector2 spawnPoint = NPC.Center + new Vector2(-1f, 23f);
                    for (int k = 0; k < totalProjectiles; k++)
                    {
                        Vector2 laserVelocity = spinningPoint.RotatedBy(radians * k);
                        Projectile.NewProjectile(NPC.GetSource_FromAI(), spawnPoint + Vector2.Normalize(laserVelocity) * 17f, laserVelocity, type, 0, 0f, Main.myPlayer, 0f, NPC.whoAmI);
                    }
                }
                if (deathrayCounter == 100)
                {
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        SoundEngine.PlaySound(NPCs.ExoMechs.Ares.AresBody.LaserStartSound, NPC.Center);
                        int type = ModContent.ProjectileType<AresDeathBeamStart>();
                        int damage = 69;
                        Vector2 spawnPoint = NPC.Center + new Vector2(-1f, 23f);
                        for (int k = 0; k < 8; k++)
                        {
                            Vector2 laserVelocity = spinningPoint.RotatedBy(radians * k);
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), spawnPoint + Vector2.Normalize(laserVelocity) * 35f, laserVelocity, type, damage, 0f, Main.myPlayer, 0f, NPC.whoAmI);
                        }
                    }
                }
                deathrayCounter++;
                if (deathrayCounter > 600)
                {
                    deathrayCounter = 0;
                }
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            SpriteEffects spriteEffects = SpriteEffects.None;
            if (NPC.spriteDirection == 1)
                spriteEffects = SpriteEffects.FlipHorizontally;
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Vector2 origin = new Vector2((float)(texture.Width / 2), (float)(texture.Height / 2));
            Vector2 npcOffset = NPC.Center - screenPos;
            npcOffset -= new Vector2((float)texture.Width, (float)texture.Height) * NPC.scale / 2f;
            npcOffset += origin * NPC.scale + new Vector2(0f, NPC.gfxOffY);
            spriteBatch.Draw(texture, npcOffset, NPC.frame, drawColor, NPC.rotation, origin, NPC.scale, spriteEffects, 0f);
            return false;
        }

        public override void ModifyIncomingHit(ref NPC.HitModifiers modifiers)
        {
            // When struck below 1% HP, activates desperation and becomes essentially immune to damage.
            // The hit which causes this has its damage capped at 1, then THE LORDE heals for 1 so that it doesn't take any net damage.
            if ((double)NPC.life <= (double)NPC.lifeMax * 0.01f && invincibleCounter == 0 && !ajitPaiDidNothingWrong)
            {
                ajitPaiDidNothingWrong = true;
                modifiers.SetMaxDamage(1);
                NPC.life += 1;
            }
        }

        // ugly antibutcher implementation because the base damage has to be known
        // assume a crit; if a crit would trigger antibutcher, the regular hit is also blocked
        // similarly, assume highest possible random damage variation
        public override void ModifyHitByItem(Player player, Item item, ref NPC.HitModifiers modifiers)
        {
            int antiButcherLimit = NPC.lifeMax / 250;
            float maxDamageVariation = 1f + 0.01f * BalancingConstants.NewDefaultDamageVariationPercent;
            int highestPossibleDamage = (int)(maxDamageVariation * modifiers.GetDamage(item.damage, true));

            if (highestPossibleDamage > antiButcherLimit)
            {
                string key = "Mods.CalamityMod.Status.Boss.EdgyBossText8";
                Color messageColor = Color.Cyan;
                CalamityUtils.DisplayLocalizedText(key, messageColor);

                // The hit which triggers antibutcher has its damage capped at 1, then THE LORDE heals for 1 so that it doesn't take any net damage.
                modifiers.SetMaxDamage(1);
                NPC.life += 1;
            }
        }

        // see comment on the use style 1 antibutcher
        public override void ModifyHitByProjectile(Projectile projectile, ref NPC.HitModifiers modifiers)
        {
            int antiButcherLimit = NPC.lifeMax / 250;
            float maxDamageVariation = 1f + 0.01f * BalancingConstants.NewDefaultDamageVariationPercent;
            int highestPossibleDamage = (int)(maxDamageVariation * modifiers.GetDamage(projectile.damage, true));

            if (highestPossibleDamage > antiButcherLimit)
            {
                string key = "Mods.CalamityMod.Status.Boss.EdgyBossText8";
                Color messageColor = Color.Cyan;
                CalamityUtils.DisplayLocalizedText(key, messageColor);

                // The hit which triggers antibutcher has its damage capped at 1, then THE LORDE heals for 1 so that it doesn't take any net damage.
                modifiers.SetMaxDamage(1);
                NPC.life += 1;
            }
        }

        // Cannot be struck by any weapons or projectiles while invincible
        public override bool? CanBeHitByItem(Player player, Item item) => (ajitPaiDidNothingWrong && invincibleCounter < 6000) ? false : null;
        public override bool? CanBeHitByProjectile(Projectile projectile) => (ajitPaiDidNothingWrong && invincibleCounter < 6000) ? false : null;

        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            cooldownSlot = 1;
            return true;
        }

        public override void OnHitPlayer(Player player, Player.HurtInfo hitinfo)
        {
            player.AddBuff(ModContent.BuffType<NOU>(), 1337, true);
        }

        public override bool CheckActive()
        {
            return canDespawn;
        }

        public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float bossLifeScale, float anotherthing)
        {
            NPC.lifeMax = (int)(NPC.lifeMax * 0.5f * bossLifeScale);
            NPC.damage = (int)(NPC.damage * 0.5f);
        }
        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            double pisquaredover6 = Math.Pow(MathHelper.Pi, 2) / 6;
            npcLoot.AddIf(()=> CalamityWorld.LegendaryMode && CalamityWorld.revenge, ModContent.ItemType<SuspiciousLookingNOU>()); // guaranteed in legendarev mode
            npcLoot.AddIf(() => !(CalamityWorld.LegendaryMode && CalamityWorld.revenge), ModContent.ItemType<SuspiciousLookingNOU>(), 27); // otherwise 1 in 27
            npcLoot.Add(ModContent.ItemType<DeliciousMeat>(), 1, 22, (int)(pisquaredover6 * 100));
        }

        public override void OnKill()
        {
            SoundEngine.PlaySound(DeathSound with { Volume = 2 }, Main.LocalPlayer.Center);
        }
    }
}