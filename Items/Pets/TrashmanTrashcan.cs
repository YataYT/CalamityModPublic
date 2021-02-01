using CalamityMod.Buffs.Pets;
using CalamityMod.Projectiles.Pets;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Pets
{
    public class TrashmanTrashcan : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Trash Can");
            Tooltip.SetDefault("Summons the trash man");
        }
        public override void SetDefaults()
        {
            item.damage = 0;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.useAnimation = 20;
            item.useTime = 20;
            item.noMelee = true;
            item.width = 30;
            item.height = 30;
            item.shoot = ModContent.ProjectileType<DannyDevitoPet>();
            item.buffType = ModContent.BuffType<DannyDevito>();
            item.UseSound = SoundID.NPCDeath13;

            item.value = Item.sellPrice(gold: 1);
            item.rare = ItemRarityID.Pink;
            item.Calamity().devItem = true;
        }

        public override void UseStyle(Player player)
        {
            if (player.whoAmI == Main.myPlayer && player.itemTime == 0)
            {
                player.AddBuff(item.buffType, 15, true);
            }
        }
    }
}
