﻿using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.World;

namespace CalamityMod.Items
{
	public class MLGRune2 : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Celestial Onion");
			Tooltip.SetDefault("Grants a seventh accesory slot\n" +
                "This will not work if you already have seven or more accessory slots");
		}
		
		public override void SetDefaults()
		{
			item.width = 28;
			item.height = 28;
			item.rare = 10;
			item.maxStack = 99;
			item.useAnimation = 30;
			item.useTime = 30;
			item.useStyle = 4;
			item.UseSound = SoundID.Item4;
			item.consumable = true;
		}
		
		public override bool CanUseItem(Player player)
		{
			CalamityPlayer modPlayer = player.GetModPlayer<CalamityPlayer>(mod);
			if (modPlayer.extraAccessoryML)
			{
				return false;
			}
			return true;
		}

		public override bool UseItem(Player player)
		{
			CalamityPlayer modPlayer = player.GetModPlayer<CalamityPlayer>(mod);
			if (player.itemAnimation > 0 && !modPlayer.extraAccessoryML && player.itemTime == 0)
			{
				player.itemTime = item.useTime;
				modPlayer.extraAccessoryML = true;
				if (!CalamityWorld.onionMode)
				{
					CalamityWorld.onionMode = true;
				}
			}
			return true;
		}
	}
}
