﻿using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Buffs.StatDebuffs
{
    public class WarCleave : ModBuff
    {
        public static int DefenseReduction = 15;

        public override void SetDefaults()
        {
            DisplayName.SetDefault("War Cleave");
            Description.SetDefault("Defense and protection reduced");
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = true;
            longerExpertDebuff = true;
        }

        public override void Update(NPC npc, ref int buffIndex)
        {
			if (npc.Calamity().wCleave < npc.buffTime[buffIndex])
				npc.Calamity().wCleave = npc.buffTime[buffIndex];
			npc.DelBuff(buffIndex);
			buffIndex--;
        }
    }
}
