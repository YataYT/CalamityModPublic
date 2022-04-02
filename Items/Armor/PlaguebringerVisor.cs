using CalamityMod.Buffs.Summon;
using CalamityMod.Projectiles.Summon;
using CalamityMod.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Armor
{
    [AutoloadEquip(EquipType.Head)]
    public class PlaguebringerVisor : ModItem
    {
        public const int PlagueDashIFrames = 4;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Plaguebringer Visor");
            Tooltip.SetDefault("15% increased minion damage\n" +
            "+20 max life");
        }

        public override void SetDefaults()
        {
            item.width = 18;
            item.height = 18;
            item.defense = 7; // 32 total
            item.value = CalamityGlobalItem.Rarity8BuyPrice;
            item.rare = ItemRarityID.Yellow;
            item.Calamity().donorItem = true;
        }

        public override void UpdateEquip(Player player)
        {
            player.minionDamage += 0.15f;
            player.statLifeMax2 += 20;
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ModContent.ItemType<PlaguebringerCarapace>() && legs.type == ModContent.ItemType<PlaguebringerPistons>();
        }

        public override void ArmorSetShadows(Player player)
        {
            player.armorEffectDrawShadow = true;
        }

        public override void UpdateArmorSet(Player player)
        {
            player.setBonus = "Grants a plague dash to ram enemies and afflict them with the plague\n" +
            "Summons a lil' plaguebringer to protect you and empower nearby minions\n" +
            "+3 max minions";

            player.Calamity().plaguebringerPatronSet = true;
            player.Calamity().dashMod = 8;
            player.dash = 0;
            player.maxMinions += 3;
            if (player.whoAmI == Main.myPlayer)
            {
                if (player.FindBuffIndex(ModContent.BuffType<PlaguebringerSummonBuff>()) == -1)
                {
                    player.AddBuff(ModContent.BuffType<PlaguebringerSummonBuff>(), 3600, true);
                }
                if (player.ownedProjectileCounts[ModContent.ProjectileType<PlaguebringerSummon>()] < 1)
                {
                    Projectile.NewProjectile(player.Center.X, player.Center.Y, 0f, -1f, ModContent.ProjectileType<PlaguebringerSummon>(), (int)(80 * player.MinionDamage()), 0f, player.whoAmI, 0f, 0f);
                }
            }

            // Constantly emit dim green light
            Lighting.AddLight(player.Center, 0f, 0.39f, 0.24f);
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.BeeHeadgear);
            recipe.AddIngredient(ModContent.ItemType<PlagueCellCluster>(), 4);
            recipe.AddIngredient(ModContent.ItemType<InfectedArmorPlating>(), 4);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
