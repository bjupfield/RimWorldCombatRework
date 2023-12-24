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
            if (lineList[i].opcode == OpCodes.Ldarg_S) Verse.Log.Warning(lineList[i].ToString());
        }
        //int finder = 0;
        //int foundOperand = 0;
        //while(finder < lineList.Count)
        //{
        //    finder++;
        //    foundOperand++;
        //    Verse.Log.Warning((lineList[foundOperand].operand != null ? lineList[foundOperand].operand.ToString() : "NOT APPLICABLE"));
        //    if ((lineList[foundOperand].operand != null ? lineList[foundOperand].operand.ToString() : "NOT APPLICABLE") == "damageDef")
        //    {
        //        finder = lineList.Count;
        //    }
        //}

        List<CodeInstruction> myInstructs = new List<CodeInstruction>();

        //load ldarg.4
        myInstructs.Add(new CodeInstruction(OpCodes.Ldarg_S, 4));

        myInstructs.Add(new CodeInstruction(OpCodes.Ldind_Ref, null));

        Type thingDefType = typeof(DamageDef);
        //ldfld class verse.thingdef vese.thing::defName
        myInstructs.Add(new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(thingDefType, "defName")));
        Type[] myParams = { typeof(string) };
        //calls Verse.Log.Warning(DamaageDef.defName);
        myInstructs.Add(CodeInstruction.Call(typeof(Verse.Log), "Warning", myParams));

        for(int i = 0; i < myInstructs.Count; i++)
        {
            Verse.Log.Warning(myInstructs[i].ToString());
        }


        lineList.InsertRange(0, myInstructs);

        //for (int i = 0; i < lineList.Count; i++)
        //{
        //    Verse.Log.Warning(lineList[i].ToString());
        //}
        return lineList;
    }
}