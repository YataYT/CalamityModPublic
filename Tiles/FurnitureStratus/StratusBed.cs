using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Tiles.FurnitureStratus
{
    public class StratusBed : ModTile
    {
        public override void SetStaticDefaults()
        {
            this.SetUpBed(true);
            ModTranslation name = CreateMapEntryName();
            name.SetDefault("Bed");
            AddMapEntry(new Color(191, 142, 111), name);
            TileID.Sets.DisableSmartCursor[Type] = true;
            AdjTiles = new int[] { TileID.Beds };
            bed = true;
        }

        public override bool CreateDust(int i, int j, ref int type)
        {
            Dust.NewDust(new Vector2(i, j) * 16f, 16, 16, 1, 0f, 0f, 1, new Color(100, 130, 150), 1f);
            Dust.NewDust(new Vector2(i, j) * 16f, 16, 16, 132, 0f, 0f, 1, new Color(255, 255, 255), 1f);
            return false;
        }

        public override bool HasSmartInteract()
        {
            return true;
        }

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 3;
        }

        public override void KillMultiTile(int i, int j, int frameX, int frameY)
        {
            Item.NewItem(i * 16, j * 16, 64, 32, ModContent.ItemType<Items.Placeables.FurnitureStratus.StratusBed>());
        }

        public override bool RightClick(int i, int j)
        {
            return CalamityUtils.BedRightClick(i, j);
        }

        public override void MouseOver(int i, int j)
        {
            Player player = Main.LocalPlayer;
            player.noThrow = 2;
            player.showItemIcon = true;
            player.showItemIcon2 = ModContent.ItemType<Items.Placeables.FurnitureStratus.StratusBed>();
        }
    }
}
