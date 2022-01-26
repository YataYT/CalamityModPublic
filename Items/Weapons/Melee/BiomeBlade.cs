using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.CalPlayer;
using CalamityMod.DataStructures;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables;
using CalamityMod.Particles;
using CalamityMod.Projectiles.Melee;
using Terraria.Graphics.Shaders;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;
using Terraria.ModLoader.IO;
using static CalamityMod.Items.Weapons.Melee.BiomeBlade;
using static CalamityMod.CalamityUtils;



namespace CalamityMod.Items.Weapons.Melee
{
    public class BiomeBlade : ModItem
    {
        public Attunement mainAttunement = null;
        public Attunement secondaryAttunement = null;
        public int Combo = 0;
        public int CanLunge = 1;

        #region stats
        public static int DefaultAttunement_BaseDamage = 55;

        public static int EvilAttunement_BaseDamage = 102;
        public static int EvilAttunement_Lifesteal = 3;
        public static int EvilAttunement_BounceIFrames = 10;

        public static int ColdAttunement_BaseDamage = 70;
        public static float ColdAttunement_SecondSwingBoost = 1.8f; 
        public static float ColdAttunement_ThirdSwingBoost = 3f;

        public static int HotAttunement_BaseDamage = 70;
        public static int HotAttunement_ShredIFrames = 8;
        public static int HotAttunement_LocalIFrames = 30; //Be warned its got one extra update so all the iframes should be divided in 2
        public static int HotAttunement_LocalIFramesCharged = 16;

        public static int TropicalAttunement_BaseDamage = 55;
        public static float TropicalAttunement_ChainDamageReduction = 0.5f;
        public static int TropicalAttunement_LocalIFrames = 60; //Be warned its got 2 extra updates so all the iframes should be divided in 3
        #endregion

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Broken Biome Blade"); //Broken Ecoliburn lmfao. Tbh a proper name instead of just "biome blade" may be neat given the importance of the sword
            Tooltip.SetDefault("FUNCTION_DESC\n" +
                               "Use RMB while standing still on the ground to attune the weapon to the powers of the surrounding biome\n" +
                               "Using RMB otherwise switches between the current attunement and an extra stored one\n" +
                               "Main attunement : None\n" +
                               "Secondary attunement: None\n"); //Theres potential for flavor text as well but im not a writer
        }

        public override void ModifyTooltips(List<TooltipLine> list)
        {
            Player player = Main.player[Main.myPlayer];
            if (player is null)
                return;

            foreach (TooltipLine l in list) 
            {
                if (l.text.StartsWith("FUNCTION_DESC"))
                {
                    if (mainAttunement != null)
                    {
                        l.overrideColor = mainAttunement.tooltipColor;
                        l.text = mainAttunement.function_description;
                    }
                    else
                    {
                        l.overrideColor = new Color(163, 163, 163);
                        l.text = "Does nothing.. yet";
                    }
                }

                if (l.text.StartsWith("Main attunement"))
                {
                    if (mainAttunement != null)
                    {
                        l.overrideColor = mainAttunement.tooltipColor;
                        l.text = "Main Attumenent : [" + mainAttunement.name + "]";
                    }
                    else
                    {
                        l.overrideColor = new Color(163, 163, 163);
                        l.text = "Main Attumenent : [None]";
                    }
                }

                if (l.text.StartsWith("Secondary attunement"))
                {
                    if (secondaryAttunement != null)
                    {
                        l.overrideColor = Color.Lerp(secondaryAttunement.tooltipColor, Color.Gray, 0.5f);
                        l.text = "Secondary Attumenent : [" + secondaryAttunement.name + "]";
                    }
                    else
                    {
                        l.overrideColor = new Color(163, 163, 163);
                        l.text = "Secondary Attumenent : [None]";
                    }
                }
            }
        }

        public override void SetDefaults()
        {
            item.width = item.height = 36;
            item.damage = 55;
            item.melee = true;
            item.useAnimation = 30;
            item.useTime = 30;
            item.useTurn = true;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.shoot = ProjectileID.PurificationPowder; 
            item.knockBack = 5f;
            item.autoReuse = true;
            item.value = Item.buyPrice(0, 4, 0, 0);
            item.rare = ItemRarityID.Orange;
            item.shootSpeed = 12f;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddRecipeGroup("AnyWoodenSword");
            recipe.AddIngredient(ItemID.DirtBlock, 50);
            recipe.AddIngredient(ItemID.StoneBlock, 50);
            recipe.AddIngredient(ItemID.HellstoneBar, 5);
            recipe.AddIngredient(ItemType<VictoryShard>(), 5);
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }

        #region Saving and syncing attunements
        public override bool CloneNewInstances => true;

        public override ModItem Clone(Item item) 
        {
            var clone = base.Clone(item);

            if (Main.mouseItem.type == ItemType<BiomeBlade>())
                item.modItem.HoldItem(Main.player[Main.myPlayer]);

            (clone as BiomeBlade).mainAttunement = (item.modItem as BiomeBlade).mainAttunement;
            (clone as BiomeBlade).secondaryAttunement = (item.modItem as BiomeBlade).secondaryAttunement;

            //As funny as a Broken Broken Biome Blade would be, its also quite funny to make it turn into that. This is only done for a new instance of the item since the goblin tinkerer changes prevent it from happening through reforging
            if (clone.item.prefix == PrefixID.Broken)
            {
                clone.item.Prefix(PrefixID.Legendary);
                clone.item.prefix = PrefixID.Legendary;
            }

            return clone;
        }

        public override ModItem Clone() //ditto
        {
            var clone = base.Clone();

            (clone as BiomeBlade).mainAttunement = mainAttunement;
            (clone as BiomeBlade).secondaryAttunement = secondaryAttunement;

            if (clone.item.prefix == PrefixID.Broken)
            {
                clone.item.Prefix(PrefixID.Legendary);
                clone.item.prefix = PrefixID.Legendary;
            }

            return clone;
        }

        public override TagCompound Save()
        {
            int attunement1 = mainAttunement == null? -1 : (int)mainAttunement.id;
            int attunement2 = secondaryAttunement == null ? -1 : (int)secondaryAttunement.id;
            TagCompound tag = new TagCompound
            {
                { "mainAttunement", attunement1 },
                { "secondaryAttunement", attunement2 }
            };
            return tag;
        }

        public override void Load(TagCompound tag)
        {
            int attunement1 = tag.GetInt("mainAttunement");
            int attunement2 = tag.GetInt("secondaryAttunement");

            mainAttunement = Attunement.attunementArray[attunement1 != -1 ? attunement1 : Attunement.attunementArray.Length - 1];
            secondaryAttunement = Attunement.attunementArray[attunement2 != -1 ? attunement2 : Attunement.attunementArray.Length - 1];

            if (mainAttunement == secondaryAttunement)
                secondaryAttunement = null;
        }

        public override void NetSend(BinaryWriter writer)
        {
            writer.Write(mainAttunement != null ? (byte)mainAttunement.id : Attunement.attunementArray.Length - 1);
            writer.Write(secondaryAttunement != null ? (byte)secondaryAttunement.id : Attunement.attunementArray.Length - 1);
        }

        public override void NetRecieve(BinaryReader reader)
        {
            mainAttunement = Attunement.attunementArray[reader.ReadByte()];
            secondaryAttunement = Attunement.attunementArray[reader.ReadByte()];
        }

        #endregion

        public override void HoldItem(Player player)
        {

            player.Calamity().rightClickListener = true;

            if (player.velocity.Y == 0) //Reset the lunge ability on ground contact
                CanLunge = 1;


            //Change the swords function based on its attunement
            if (mainAttunement == null)
            {
                item.noUseGraphic = false;
                item.useStyle = ItemUseStyleID.SwingThrow;
                item.shoot = ProjectileID.PurificationPowder;
                item.shootSpeed = 12f;
                item.UseSound = SoundID.Item1;
                Combo = 0;
            }

            else
            {
                if (mainAttunement.id < AttunementID.Default || mainAttunement.id > AttunementID.Evil)
                    mainAttunement = Attunement.attunementArray[(int)MathHelper.Clamp((float)mainAttunement.id, (float)AttunementID.Default, (float)AttunementID.Evil)];

                mainAttunement.ApplyStats(item);
            }

            if (mainAttunement != null && mainAttunement.id != AttunementID.Cold)
                Combo = 0;


            if (player.Calamity().mouseRight && CanUseItem(player) && player.whoAmI == Main.myPlayer && !Main.mapFullscreen)
            {
                //Don't shoot out a visual blade if you already have one out
                if (Main.projectile.Any(n => n.active && n.type == ProjectileType<BrokenBiomeBladeHoldout>() && n.owner == player.whoAmI))
                    return;


                bool mayAttune = player.StandingStill() && !player.mount.Active && player.CheckSolidGround(1, 3);
                Vector2 displace = new Vector2(18f, 0f);
                Projectile.NewProjectile(player.Top + displace, Vector2.Zero, ProjectileType<BrokenBiomeBladeHoldout>(), 0, 0, player.whoAmI, mayAttune ? 0f : 1f);
            }
        }

        public override bool CanUseItem(Player player)
        {
            return !Main.projectile.Any(n => n.active && n.owner == player.whoAmI &&
            (n.type == ProjectileType<BitingEmbrace>() || n.type == ProjectileType<GrovetendersTouch>() || n.type == ProjectileType<AridGrandeur>()));
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            if (mainAttunement == null)
                return false;


            int powerLungeCounter = 0; //Unused here
            return mainAttunement.Shoot(player, ref position, ref speedX, ref speedY, ref type, ref damage, ref knockBack, ref Combo, ref CanLunge, ref powerLungeCounter);
        }

        internal static ChargingEnergyParticleSet BiomeEnergyParticles = new ChargingEnergyParticleSet(-1, 2, Color.DarkViolet, Color.White, 0.04f, 20f);
        internal static void UpdateAllParticleSets()
        {
            BiomeEnergyParticles.Update();
        }

        public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            Texture2D itemTexture = Main.itemTexture[item.type];
            Rectangle itemFrame = (Main.itemAnimations[item.type] == null) ? itemTexture.Frame() : Main.itemAnimations[item.type].GetFrame(itemTexture);

            if (mainAttunement == null)
                return true;

            // Draw all particles.

            Vector2 particleDrawCenter = position + new Vector2(12f, 16f) * Main.inventoryScale;

            BiomeEnergyParticles.EdgeColor = mainAttunement.energyParticleEdgeColor;
            BiomeEnergyParticles.CenterColor = mainAttunement.energyParticleCenterColor;
            BiomeEnergyParticles.InterpolationSpeed = 0.1f;
            BiomeEnergyParticles.DrawSet(particleDrawCenter + Main.screenPosition);

            Vector2 displacement = Vector2.UnitX.RotatedBy(Main.GlobalTime * 3f) * 2f * (float)Math.Sin(Main.GlobalTime);

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive);
            spriteBatch.Draw(itemTexture, position + displacement, itemFrame, BiomeEnergyParticles.CenterColor, 0f, origin, scale, SpriteEffects.None, 0f);
            spriteBatch.Draw(itemTexture, position - displacement, itemFrame, BiomeEnergyParticles.CenterColor, 0f, origin, scale, SpriteEffects.None, 0f);
            spriteBatch.End();
            spriteBatch.Begin(default, default);

            return true;
        }

    }

    public class BrokenBiomeBladeHoldout : ModProjectile //Visuals
    {
        private Player Owner => Main.player[projectile.owner];

        public bool OwnerCanUseItem => Owner.HeldItem == associatedItem ? (Owner.HeldItem.modItem as BiomeBlade).CanUseItem(Owner) : false;
        public bool OwnerMayChannel => OwnerCanUseItem && Owner.Calamity().mouseRight && Owner.active && !Owner.dead && Owner.StandingStill() && !Owner.mount.Active && Owner.CheckSolidGround(1, 3);
        public ref float ChanneledState => ref projectile.ai[0];
        public ref float ChannelTimer => ref projectile.ai[1];
        public ref float Initialized => ref projectile.localAI[0];

        private Item associatedItem;
        const int ChannelTime = 120;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Broken Biome Blade");
        }
        public override string Texture => "CalamityMod/Items/Weapons/Melee/BiomeBlade";
        public bool drawIndrawHeldProjInFrontOfHeldItemAndArms = true;
        public override void SetDefaults()
        {
            projectile.width = projectile.height = 36;
            projectile.aiStyle = -1;
            projectile.friendly = true;
            projectile.penetrate = -1;
            projectile.timeLeft = 60;
            projectile.tileCollide = false;
            projectile.damage = 0;
        }

        public CurveSegment anticipation = new CurveSegment(EasingType.SineOut, 0f, 1f, 0.35f);
        public CurveSegment thrust = new CurveSegment(EasingType.ExpIn, 0.85f, 1.35f, -1.45f);
        public CurveSegment bounceback = new CurveSegment(EasingType.SineOut, 0.95f, -0.1f, 0.1f);
        internal float SwordHeight() => PiecewiseAnimation(ChannelTimer / (float)ChannelTime, new CurveSegment[] { anticipation, thrust, bounceback });



        public override void AI()
        {

            if (Initialized == 0f)
            {
                //If dropped, kill it instantly
                if (Owner.HeldItem.type != ItemType<BiomeBlade>())
                {
                    projectile.Kill();
                    return;
                }

                if (Owner.whoAmI == Main.myPlayer)
                    Main.PlaySound(SoundID.DD2_DarkMageHealImpact);

                associatedItem = Owner.HeldItem;
                //Switch up the attunements
                Attunement temporaryAttunementStorage = (associatedItem.modItem as BiomeBlade).mainAttunement;
                (associatedItem.modItem as BiomeBlade).mainAttunement = (associatedItem.modItem as BiomeBlade).secondaryAttunement;
                (associatedItem.modItem as BiomeBlade).secondaryAttunement = temporaryAttunementStorage;
                Initialized = 1f;
            }

            if (!OwnerMayChannel && ChanneledState == 0f) //IF the channeling gets interrupted for any reason
            {
                projectile.Center = Owner.Top + new Vector2(18f, 0f);
                ChanneledState = 1f;
                projectile.timeLeft = 60;
                return;
            }

            if (ChanneledState == 0f)
            {
                Owner.heldProj = projectile.whoAmI;

                projectile.Center = Owner.Center + new Vector2(16f * Owner.direction, -30 * SwordHeight() + 10f) ;
                projectile.rotation = Utils.AngleLerp(-MathHelper.PiOver4, MathHelper.PiOver4 + MathHelper.PiOver2, MathHelper.Clamp(((ChannelTimer - 20f) / 50f), 0f, 1f));
                ChannelTimer++;
                projectile.timeLeft = 60;

                if (ChannelTimer >= ChannelTime)
                {
                    Attune((BiomeBlade)associatedItem.modItem);
                    projectile.timeLeft = 120;
                    ChanneledState = 2f; //State where it stays invisible doing nothing. Acts as a cooldown

                    Color particleColor = (associatedItem.modItem as BiomeBlade).mainAttunement.tooltipColor;

                    for (int i = 0; i <= 5; i++)
                    {
                        Vector2 displace = Vector2.UnitX * 20 * Main.rand.NextFloat(-1f, 1f);
                        Particle Glow = new GenericBloom(Owner.Bottom + displace, -Vector2.UnitY * Main.rand.NextFloat(1f, 5f), particleColor, 0.02f + Main.rand.NextFloat(0f, 0.2f), 20 + Main.rand.Next(30));
                        GeneralParticleHandler.SpawnParticle(Glow);
                    }
                    for (int i = 0; i <= 10; i++)
                    {
                        Vector2 displace = Vector2.UnitX * 16 * Main.rand.NextFloat(-1f, 1f);
                        Particle Sparkle = new GenericSparkle(Owner.Bottom + displace, -Vector2.UnitY * Main.rand.NextFloat(1f, 5f), particleColor, particleColor, 0.5f + Main.rand.NextFloat(-0.2f, 0.2f), 20 + Main.rand.Next(30), 1, 2f);
                        GeneralParticleHandler.SpawnParticle(Sparkle);
                    }
                }
            }

            if (ChanneledState == 1f)
                projectile.position += Vector2.UnitY * -0.3f * (1f + projectile.timeLeft/60f);
        }

       public void Attune(BiomeBlade item)
       {
            bool jungle = Owner.ZoneJungle;
            bool snow = Owner.ZoneSnow;
            bool evil = Owner.ZoneCorrupt || Owner.ZoneCrimson;
            bool desert = Owner.ZoneDesert;
            bool hell = Owner.ZoneUnderworldHeight;
            bool ocean = Owner.ZoneBeach;

            Attunement attunement = Attunement.attunementArray[(int)AttunementID.Default];

            if (desert || hell)
            {
                attunement = Attunement.attunementArray[(int)AttunementID.Hot];
            }
            if (snow)
            {
                attunement = Attunement.attunementArray[(int)AttunementID.Cold];
            }
            if (jungle || ocean)
            {
                attunement = Attunement.attunementArray[(int)AttunementID.Tropical];
            }
            if (evil)
            {
                attunement = Attunement.attunementArray[(int)AttunementID.Evil];
            }

            //If the owner already had the attunement , break out of it (And unswap)
            if (item.secondaryAttunement == attunement)
            {
                Main.PlaySound(SoundID.DD2_LightningBugZap, projectile.Center);
                item.secondaryAttunement = item.mainAttunement;
                item.mainAttunement = attunement;
                return;
            }

            Main.PlaySound(SoundID.DD2_MonkStaffGroundImpact, projectile.Center);
            item.mainAttunement = attunement;
        }       

        public override void Kill(int timeLeft)
        {
            if (associatedItem == null)
            {
                return;
            }

            //If we swapped out the main attunement for the second one despite the second attunement being empty at the time, unswap them.
            if ((associatedItem.modItem as BiomeBlade).mainAttunement == null && (associatedItem.modItem as BiomeBlade).secondaryAttunement != null)
            {
                (associatedItem.modItem as BiomeBlade).mainAttunement = (associatedItem.modItem as BiomeBlade).secondaryAttunement;
                (associatedItem.modItem as BiomeBlade).secondaryAttunement = null;
            }

            //Cool particles
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            if (ChanneledState == 0f && ChannelTimer > 6f)
                return base.PreDraw(spriteBatch, lightColor);

            else if (ChanneledState == 1f)
            {
                Texture2D tex = GetTexture(Texture);
                Vector2 squishyScale = new Vector2(Math.Abs((float)Math.Sin(MathHelper.Pi + MathHelper.TwoPi  * projectile.timeLeft / 30f)), 1f);
                SpriteEffects flip = (float)Math.Sin(MathHelper.Pi + MathHelper.TwoPi * projectile.timeLeft / 30f) > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
                spriteBatch.Draw(tex, projectile.position - Main.screenPosition, null, lightColor * (projectile.timeLeft / 60f), 0, tex.Size() / 2, squishyScale * (2f - (projectile.timeLeft / 60f)), flip, 0);

                return false;
            }

            else
                return false;
        }
    }

}


