using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Buffs.StatBuffs
{
    public class BloodfinBoost : ModBuff
    {
        public override void SetDefaults()
        {
            DisplayName.SetDefault("Bloodfin Boost");
            Description.SetDefault("Don't let the blood get to your head");
            Main.debuff[Type] = false;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = true;
            longerExpertDebuff = false;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.Calamity().bloodfinBoost = true;
        }
    }
}
