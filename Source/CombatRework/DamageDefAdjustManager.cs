using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;

namespace CombatRework
{
    public static class DamageDefAdjustManager
    {
        private static Dictionary<String, DamageDefAdjust> allDamages;


        public static int onLoad()
        {
            allDamages = new Dictionary<String, DamageDefAdjust>();
            List<DamageDefAdjust> myDamages = DefDatabase<DamageDefAdjust>.AllDefsListForReading;
            List<ThingDef> myBullets = DefDatabase<ThingDef>.AllDefsListForReading;
            Verse.Log.Warning("WE ARE IN THE STATIC MANAGER CLASS");
            myBullets.RemoveAll(thing =>
            {
                return thing.label != "arrow" && thing.label != "bullet";
            });
            foreach(DamageDefAdjust damage in myDamages)
            {
                if(myBullets.Find(realDamage => realDamage.defName == damage.defName) != null)
                {
                    allDamages.Add(damage.defName, damage);
                }
            }
            return 0;
        }
        public static DamageDefAdjust pullDamageDef(string DefName)
        {
            Verse.Log.Warning("Weapon Name: " + DefName);
            Verse.Log.Warning("THIS IS THIS WEAPONS SHIELD DAMAGE: " + allDamages[DefName].shieldDamage.ToString());
            Verse.Log.Warning("THIS IS THIS WEAPONS ARMOR DAMAGE: " + allDamages[DefName].armorDamage.ToString());
            return allDamages[DefName];
        }

    }
}
