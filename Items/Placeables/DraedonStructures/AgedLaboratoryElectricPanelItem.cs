﻿using CalamityMod.Items.DraedonMisc;
using CalamityMod.Items.Materials;
using CalamityMod.Tiles.DraedonStructures;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Items.Placeables.DraedonStructures
{
    public class AgedLaboratoryElectricPanelItem : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Placeables";
        public override void SetDefaults()
        {
            Item.width = 12;
            Item.height = 12;
            Item.maxStack = 9999;
            Item.useTurn = true;
            Item.autoReuse = true;
            Item.useAnimation = 15;
            Item.useTime = 10;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.consumable = true;
            Item.createTile = ModContent.TileType<AgedLaboratoryElectricPanel>();
        }

        public override void AddRecipes()
        {
            CreateRecipe().AddIngredient<Items.Placeables.DraedonStructures.RustedPlating>(7).AddIngredient<DubiousPlating>().AddIngredient<DraedonPowerCell>(4).AddTile(TileID.Anvils).Register();
        }
    }
}
