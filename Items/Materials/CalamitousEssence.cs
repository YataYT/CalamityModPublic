using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Materials
{
    public class CalamitousEssence : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Ashes of Annihilation");
            Tooltip.SetDefault("It burns with untold power");
            Main.RegisterItemAnimation(item.type, new DrawAnimationVertical(7, 6));
        }

        public override void SetDefaults()
        {
            item.width = 54;
            item.height = 56;
            item.maxStack = 999;
            item.rare = ItemRarityID.Purple;
            item.value = Item.sellPrice(gold: 24);
            item.Calamity().customRarity = CalamityRarity.Violet;
        }

        public void DrawPulsingAfterimage(SpriteBatch spriteBatch, Vector2 baseDrawPosition, Rectangle frame, float baseScale)
        {
            float pulse = Main.GlobalTime * 0.68f % 1f;
            float outwardness = pulse * baseScale * 8f;
            Color drawColor = Color.BlueViolet * (float)Math.Sqrt(1f - pulse) * 0.7f;
            drawColor.A = 0;
            for (int i = 0; i < 4; i++)
            {
                float scale = baseScale * MathHelper.Lerp(1f, 1.45f, pulse);
                Vector2 drawPosition = baseDrawPosition + (MathHelper.TwoPi * i / 4f).ToRotationVector2() * outwardness - Vector2.UnitY * 2f;
                spriteBatch.Draw(Main.itemTexture[item.type], drawPosition, frame, drawColor, item.velocity.X * 0.2f, frame.Size() * 0.5f, scale, SpriteEffects.None, 0f);
            }
        }

        public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            DrawPulsingAfterimage(spriteBatch, position + frame.Size() * 0.25f, frame, scale);
            return true;
        }

        public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            Rectangle frame = Main.itemAnimations[item.type].GetFrame(Main.itemTexture[item.type]);
            DrawPulsingAfterimage(spriteBatch, item.position - Main.screenPosition + frame.Size() * 0.5f, frame, scale);
            return true;
        }

        public override void Update(ref float gravity, ref float maxFallSpeed)
        {
            float brightness = Main.essScale * Main.rand.NextFloat(0.9f, 1.1f);
            Lighting.AddLight(item.Center, 0.34f * brightness, 0.08f * brightness, 0.155f * brightness);
        }
    }
}
