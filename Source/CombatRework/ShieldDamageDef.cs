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


        public static void onLoad()
        {
            allDamages = new Dictionary<String, DamageDefAdjust>();
            List<DamageDefAdjust> myDamages = DefDatabase<DamageDefAdjust>.AllDefs.ToList();
            List<DamageDef> myActualDamages = DefDatabase<DamageDef>.AllDefs.ToList();
            foreach(DamageDefAdjust damage in myDamages)
            {
                if(myActualDamages.Find(realDamage => realDamage.defName == damage.defName) != null)
                {
                    Verse.Log.Warning("This DamageAdjust Matches a DamageDef: " + damage.defName);
                    allDamages.Add(damage.defName, damage);
                }
                else Verse.Log.Warning("This DamageAdjust Does Not Match a DamageDef: " + damage.defName);
            }
        }
        public static DamageDefAdjust pullDamageDef(string DefName)
        {
            return allDamages[DefName];
        }

    }
}
