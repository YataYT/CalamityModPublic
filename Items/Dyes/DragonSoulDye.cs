﻿using CalamityMod.Items.Materials;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Graphics.Shaders;
using ReLogic.Content;
using Terraria.GameContent.Creative;

namespace CalamityMod.Items.Dyes
{
    public class DragonSoulDye : BaseDye
    {
        public override ArmorShaderData ShaderDataToBind => new ArmorShaderData(new Ref<Effect>(Mod.Assets.Request<Effect>("Effects/Dyes/DragonSoulDyeShader", AssetRequestMode.ImmediateLoad).Value), "DyePass");
        public override void SafeSetStaticDefaults()
        {
            DisplayName.SetDefault("Dragon Soul Dye");
        }

        public override void SafeSetDefaults()
        {
            Item.rare = ItemRarityID.Red;
            Item.Calamity().customRarity = CalamityRarity.Violet;
            Item.value = Item.sellPrice(0, 8, 0, 0);
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 3;
        }

        public override void AddRecipes()
        {
            CreateRecipe(2).
                AddIngredient(ItemID.BottledWater, 2).
                AddIngredient<HellcasterFragment>().
                AddTile(TileID.DyeVat).
                Register();
        }
    }
}
