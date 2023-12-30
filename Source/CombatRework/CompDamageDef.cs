using CombatRework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace RimWorld
{
    public class CompDamageDef : ThingComp
    {
        private int shieldDamage = 0;
        private int armorDamage = 0;

        public int get_shieldDamage()
        {
            return this.shieldDamage;
        }
        public int get_armorDamage() 
        {
            return this.armorDamage;
        }
        override public void Initialize(CompProperties props)
        {
            Verse.Log.Warning("Hey This is occuring Inside my Comp");
            Shield_Armor_Damage myDamages = DamageDefAdjustManager.pullDamageDef(this.parent.def.defName);
        }
    }
}
