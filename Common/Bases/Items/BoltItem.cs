using System;
using System.Collections.Generic;

using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AuroraMod.Common.Bases.Items
{
    public abstract class BoltItem : ModItem
    {
        protected abstract int ProjectileBolt { get; }
        protected abstract int Damage { get; }
        protected abstract int CritChance { get; }
        protected abstract int Knockback { get; }
        public override sealed void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.maxStack = 9999;

            Item.damage = Damage;
            Item.crit = CritChance;
            Item.knockBack = Knockback;

            Item.consumable = true;
            Item.ammo = Type;

            Item.shoot = ProjectileBolt;
        }
        
        public override bool? CanBeChosenAsAmmo(Item weapon, Player player) => weapon.ModItem is CrossbowItem;
    }
}
