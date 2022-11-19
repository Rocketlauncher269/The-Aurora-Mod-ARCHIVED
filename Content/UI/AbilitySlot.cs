using AuroraMod.Common.ModPlayers;
using AuroraMod.Common.Systems;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.UI;

namespace AuroraMod.Common.UI
{
    public class AbilitySlot : ModAccessorySlot
    {
        public override bool DrawVanitySlot => false;
        public override bool DrawDyeSlot => false;

        public override void OnMouseHover(AccessorySlotType context)
        {
            Player.cursorItemIconText = "Off-Hand Slot";
        }

        public override bool CanAcceptItem(Item checkItem, AccessorySlotType context)
        {
            return checkItem.ModItem is not null && checkItem.ModItem is IAbilityItem;
        }
    }
}
