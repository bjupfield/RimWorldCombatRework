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

        public CompDamageDef(string defName, QualityCategory quality)
        {
            DamageDefAdjust shieldArmorInfo = DamageDefAdjustManager.pullDamageDef(defName);
            this.shieldDamage = shieldArmorInfo.shieldDamage;
            this.shieldDamage = shieldArmorInfo.armorDamage;
        }
    }
}
