using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using UnityEngine;
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
        public static float GetPostArmorDamage(Pawn pawn, float amount, float armorPenetration, BodyPartRecord part, ref DamageDef damageDef, string projectileName, out bool deflectedByMetalArmor, out bool diminishedByMetalArmor)
        {
            deflectedByMetalArmor = false;
            diminishedByMetalArmor = false;
            if (damageDef.armorCategory == null)
            {
                return amount;
            }
            StatDef armorRatingStat = damageDef.armorCategory.armorRatingStat;
            if (pawn.apparel != null)
            {
                List<Apparel> wornApparel = pawn.apparel.WornApparel;
                for (int num = wornApparel.Count - 1; num >= 0; num--)
                {
                    Apparel apparel = wornApparel[num];
                    if (apparel.def.apparel.CoversBodyPart(part))
                    {
                        float num2 = amount;
                        ApplyArmor(ref amount, armorPenetration, apparel.GetStatValue(armorRatingStat), apparel, ref damageDef, pawn, projectileName, out var metalArmor);
                        if (amount < 0.001f)
                        {
                            deflectedByMetalArmor = metalArmor;
                            return 0f;
                        }
                        if (amount < num2 && metalArmor)
                        {
                            diminishedByMetalArmor = true;
                        }
                    }
                }
            }
            float num3 = amount;
            ApplyArmor(ref amount, armorPenetration, pawn.GetStatValue(armorRatingStat), null, ref damageDef, pawn, projectileName, out var metalArmor2);
            if (amount < 0.001f)
            {
                deflectedByMetalArmor = metalArmor2;
                return 0f;
            }
            if (amount < num3 && metalArmor2)
            {
                diminishedByMetalArmor = true;
            }
            return amount;
        }
        private static void ApplyArmor(ref float damAmount, float armorPenetration, float armorRating, Thing armorThing, ref DamageDef damageDef, Pawn pawn, string projectileName, out bool metalArmor)
        {
            pullDamageDef(projectileName);
            if (armorThing != null)
            {
                metalArmor = armorThing.def.apparel.useDeflectMetalEffect || (armorThing.Stuff != null && armorThing.Stuff.IsMetal);
            }
            else
            {
                metalArmor = pawn.RaceProps.IsMechanoid;
            }
            if (armorThing != null)
            {
                float f = damAmount * 0.25f;
                armorThing.TakeDamage(new DamageInfo(damageDef, GenMath.RoundRandom(f)));
            }
            float num = Mathf.Max(armorRating - armorPenetration, 0f);
            float value = Rand.Value;
            float num2 = num * 0.5f;
            float num3 = num;
            if (value < num2)
            {
                damAmount = 0f;
            }
            else if (value < num3)
            {
                damAmount = GenMath.RoundRandom(damAmount / 2f);
                if (damageDef.armorCategory == DamageArmorCategoryDefOf.Sharp)
                {
                    damageDef = DamageDefOf.Blunt;
                }
            }
        }
    }

}
