using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using RimWorld;
using Verse;

namespace CombatRework
{
    [StaticConstructorOnStartup]
    [HarmonyDebug]
    public static class CombatRework
    {
        static CombatRework()
        {
            Harmony harmony = new Harmony("rimworld.mod.Pelican.CombatRework");
            Harmony.DEBUG = true;
            Verse.Log.Warning("HEY");
            Verse.Log.Warning("HEY2: " + nameof(ArmorUtility.GetPostArmorDamage));
            harmony.PatchAll();
        }
    }
}
[HarmonyPatch(typeof(ArmorUtility))]
[HarmonyPatch("ApplyArmor")]
public static class Dialog_ArmorUtility_ApplyArmor_Patch
{
    [HarmonyTranspiler]
    static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> lines, ILGenerator il)
    {
        var lineList = new List<CodeInstruction>(lines);
        for (int i = 0; i < lineList.Count(); i++)
        {
            Verse.Log.Warning(lineList[i].ToString());
        }
        return lineList;
    }
}