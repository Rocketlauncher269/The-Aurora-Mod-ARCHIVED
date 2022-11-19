using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace AuroraMod.Common
{
    public interface IAbilityItem
    {
        public int Cooldown { get; }
        public void OnUse(Player player, IEntitySource source);
        public Color Color { get; }
    }

    public class AbilityGlobalItem : GlobalItem
    {
        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            if (item.ModItem is not null && item.ModItem is IAbilityItem abilityItem)
            {
                string lighterHex = (abilityItem.Color * 1.35f).Hex3();
                string hex = abilityItem.Color.Hex3();

                TooltipLine nameT = tooltips.Find(t => t.Name == "ItemName");
                nameT.Text = $"[c/{lighterHex}:{nameT.Text}]";

                tooltips.Add(new TooltipLine(Mod, "AbilityCooldown", $"[c/{hex}:Ability cooldown:] [c/{lighterHex}:{(abilityItem.Cooldown / 60f).ToString("F1")}s]"));
            }
        }
    }
}
