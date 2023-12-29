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
        private static Dictionary<String, DamageDefAdjust> allDamages = new Dictionary<string, DamageDefAdjust>();


        public static int onLoad()
        {
            allDamages = new Dictionary<String, DamageDefAdjust>();
            List<DamageDefAdjust> myDamages = DefDatabase<DamageDefAdjust>.AllDefsListForReading.ListFullCopy();
            List<ThingDef> myGuns = DefDatabase<ThingDef>.AllDefsListForReading.ListFullCopy();
            Verse.Log.Warning("WE ARE IN THE STATIC MANAGER CLASS");
            myGuns.RemoveAll(thing =>
            {
                return thing.weaponTags == null;
            });
            myGuns.RemoveAll(thing =>
            {
                if (thing.Verbs == null) return true;
                if (thing.Verbs.Count == 0) return true;
                return thing.Verbs[0].defaultProjectile == null;
            });
            foreach (DamageDefAdjust damage in myDamages)
            {
                ThingDef myGun = myGuns.Find(gun =>
                {
                    return gun.Verbs[0].defaultProjectile.defName == damage.defName;
                });
                if (myGun != null)
                {
                    allDamages.Add(myGun.defName, damage);
                }
            }

            Verse.Log.Warning("Final Count" + allDamages.Count.ToString());

            return 0;
        }
        public static DamageDefAdjust pullDamageDef(string DefName)
        {
            Verse.Log.Warning("Weapon Name: " + DefName);
            Verse.Log.Warning("Weapons Loaded: " + allDamages.Count);
            Verse.Log.Warning("THIS IS THIS WEAPONS SHIELD DAMAGE: " + allDamages[DefName].shieldDamage.ToString());
            Verse.Log.Warning("THIS IS THIS WEAPONS ARMOR DAMAGE: " + allDamages[DefName].armorDamage.ToString());
            return allDamages[DefName];
        }

        public static void partTwo(string DefName)
        {
            Verse.Log.Warning("Weapon Name: " + DefName);
            Verse.Log.Warning("Weapons Loaded: " + allDamages.Count);
            Verse.Log.Warning("Shield Damage: " + allDamages[DefName].shieldDamage.ToString());
            Verse.Log.Warning("Armor Damage: " + allDamages[DefName].armorDamage.ToString());
        }

        public static float GetPostArmorDamage(Pawn pawn, float amount, float armorPenetration, BodyPartRecord part, ref DamageDef damageDef, out bool deflectedByMetalArmor, out bool diminishedByMetalArmor, string projectileName)//alright just copying this because i need more info in the applyarmor function and this is the best way
        {
            Verse.Log.Warning("HEY THIS IS ACTIVATING");
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
                        ApplyArmor(ref amount, armorPenetration, apparel.GetStatValue(armorRatingStat), apparel, ref damageDef, pawn, out var metalArmor, projectileName);
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
            ApplyArmor(ref amount, armorPenetration, pawn.GetStatValue(armorRatingStat), null, ref damageDef, pawn, out var metalArmor2, projectileName);
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
        private static void ApplyArmor(ref float damAmount, float armorPenetration, float armorRating, Thing armorThing, ref DamageDef damageDef, Pawn pawn, out bool metalArmor, string projectileName)
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
        private static void printString(int b)
        {
            Verse.Log.Warning("This is the damage it is doing to the shield" + b.ToString());
        }
        private static void printBool(bool b)
        {
            Verse.Log.Warning("Is this a shield: " + b.ToString());
        }
        private static void takeDamagePrint(ThingDef def)
        {
            Verse.Log.Warning("It is in TakeDamage and it is: " + def.defName);
        }
    }

}
