using CalamityMod.Items.Accessories;
using CalamityMod.Items.Armor.Vanity;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Mounts;
using CalamityMod.Items.Pets;
using CalamityMod.Items.Weapons.Magic;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.Items.Weapons.Summon;
using CalamityMod.NPCs.BrimstoneElemental;
using CalamityMod.World;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace CalamityMod.Items.TreasureBags
{
    public class BrimstoneWaifuBag : ModItem
    {
        public override int BossBagNPC => ModContent.NPCType<BrimstoneElemental>();

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Treasure Bag");
            Tooltip.SetDefault("{$CommonItemTooltip.RightClickToOpen}");
        }

        public override void SetDefaults()
        {
            item.maxStack = 999;
            item.consumable = true;
            item.width = 24;
            item.height = 24;
            item.expert = true;
            item.rare = ItemRarityID.Cyan;
        }

        public override bool CanRightClick()
        {
            return true;
        }

        public override void OpenBossBag(Player player)
        {
            player.TryGettingDevArmor();

            // Materials
            DropHelper.DropItem(player, ModContent.ItemType<EssenceofChaos>(), 5, 9);
            DropHelper.DropItemCondition(player, ModContent.ItemType<Bloodstone>(), CalamityWorld.downedProvidence, 25, 35);

            // Weapons
            float w = DropHelper.BagWeaponDropRateFloat;
            DropHelper.DropEntireWeightedSet(player,
                DropHelper.WeightStack<Brimlance>(w),
                DropHelper.WeightStack<SeethingDischarge>(w),
                DropHelper.WeightStack<DormantBrimseeker>(w),
				DropHelper.WeightStack<Hellborn>(w),
				DropHelper.WeightStack<FabledTortoiseShell>(w),
				DropHelper.WeightStack<RoseStone>(w)
			);

			// Equipment
			DropHelper.DropItem(player, ModContent.ItemType<Abaddon>());
            DropHelper.DropItem(player, ModContent.ItemType<Gehenna>());
            DropHelper.DropItemCondition(player, ModContent.ItemType<Brimrose>(), CalamityWorld.downedProvidence);

			// Vanity
			DropHelper.DropItemCondition(player, ModContent.ItemType<CharredRelic>(), CalamityWorld.revenge);
            DropHelper.DropItemChance(player, ModContent.ItemType<BrimstoneWaifuMask>(), 7);
        }
    }
}
