﻿using CalamityMod.BiomeManagers;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Dusts;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables.Banners;
using CalamityMod.Items.Weapons.Summon;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.GameContent.Bestiary;
using Terraria.ModLoader;
using Terraria.Audio;
using ReLogic.Content;
using CalamityMod.Sounds;

namespace CalamityMod.NPCs.Astral
{
    public class HiveEnemy : ModNPC
    {
        private static Texture2D glowmask;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Hive");
            if (!Main.dedServ)
                glowmask = ModContent.Request<Texture2D>("CalamityMod/NPCs/Astral/HiveGlow", AssetRequestMode.ImmediateLoad).Value;
            Main.npcFrameCount[NPC.type] = 6;
        }

        public override void SetDefaults()
        {
            NPC.width = 38;
            NPC.height = 62;
            NPC.aiStyle = -1;
            NPC.damage = 55;
            NPC.defense = 15;
            NPC.DR_NERD(0.15f);
            NPC.lifeMax = 470;
            NPC.DeathSound = CommonCalamitySounds.AstralNPCDeathSound;
            NPC.knockBackResist = 0f;
            NPC.value = Item.buyPrice(0, 0, 15, 0);
            Banner = NPC.type;
            BannerItem = ModContent.ItemType<HiveBanner>();
            if (DownedBossSystem.downedAstrumAureus)
            {
                NPC.damage = 90;
                NPC.defense = 25;
                NPC.lifeMax = 700;
            }
            NPC.Calamity().VulnerableToHeat = true;
            NPC.Calamity().VulnerableToSickness = false;
            SpawnModBiomes = new int[1] { ModContent.GetInstance<UndergroundAstralBiome>().Type };
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {

				// Will move to localization whenever that is cleaned up.
				new FlavorTextBestiaryInfoElement("These seem to erupt from the caverns of the astral itself. The flesh of their hosts twists into a spiral and countless bodies make up the pulsing flesh of the hive.")
            });
        }

        public override void AI()
        {
            NPC.ai[0]++;
            if (NPC.ai[0] > (CalamityWorld.death ? 120f : 180f))
            {
                if (Main.rand.NextBool(100) && NPC.CountNPCS(ModContent.NPCType<Hiveling>()) < 10)
                {
                    NPC.ai[0] = 0;

                    //spawn hiveling, it's ai[0] is the hive npc index.
                    int n = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<Hiveling>(), 0, NPC.whoAmI);
                    Main.npc[n].velocity.X = Main.rand.NextFloat(-0.4f, 0.4f);
                    Main.npc[n].velocity.Y = Main.rand.NextFloat(-0.5f, -0.05f);
                }
            }
        }

        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter++;
            if (NPC.frameCounter > 10)
            {
                NPC.frameCounter = 0;
                NPC.frame.Y += frameHeight;
                if (NPC.frame.Y > frameHeight * 4)
                {
                    NPC.frame.Y = 0;
                }
            }
        }

        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            //draw glowmask
            spriteBatch.Draw(glowmask, NPC.Center - screenPos, NPC.frame, Color.White * 0.6f, NPC.rotation, new Vector2(19, 30), 1f, NPC.spriteDirection == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0);
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            if (NPC.soundDelay == 0)
            {
                NPC.soundDelay = 15;
                SoundEngine.PlaySound(CommonCalamitySounds.AstralNPCHitSound, NPC.Center);
            }

            CalamityGlobalNPC.DoHitDust(NPC, hitDirection, (Main.rand.Next(0, Math.Max(0, NPC.life)) == 0) ? 5 : ModContent.DustType<AstralEnemy>(), 1f, 3, 20);

            //if dead do gores
            if (NPC.life <= 0)
            {
                int type = ModContent.NPCType<Hiveling>();
                for (int i = 0; i < 200; i++)
                {
                    if (Main.npc[i].type == type)
                    {
                        Main.npc[i].ai[0] = -1f;
                    }
                }
            }
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (CalamityGlobalNPC.AnyEvents(spawnInfo.Player) || NPC.AnyNPCs(NPC.type))
            {
                return 0f;
            }
            else if (spawnInfo.Player.InAstral(2))
            {
                return 0.17f;
            }
            return 0f;
        }

        public override void OnHitPlayer(Player player, int damage, bool crit)
        {
            player.AddBuff(ModContent.BuffType<AstralInfectionDebuff>(), 120, true);
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(DropHelper.NormalVsExpertQuantity(ModContent.ItemType<Stardust>(), 1, 1, 3, 1, 4));
            npcLoot.AddIf(() => DownedBossSystem.downedAstrumAureus, ModContent.ItemType<HivePod>(), 7);
        }
    }
}