using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using CombatRework;
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

        List<CodeInstruction> myInstructs = new List<CodeInstruction>();

        //load ldarg.4
        myInstructs.Add(new CodeInstruction(OpCodes.Ldarg_S, 4));

        myInstructs.Add(new CodeInstruction(OpCodes.Ldind_Ref, null));

        Type thingDefType = typeof(DamageDef);
        //ldfld class verse.thingdef vese.thing::defName
        myInstructs.Add(new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(thingDefType, "defName")));
        //calls CombatRework.DamageDefAdjustManager.pullDamageDef()
        myInstructs.Add(CodeInstruction.Call(typeof(CombatRework.DamageDefAdjustManager), "pullDamageDef"));

        myInstructs.Add(new CodeInstruction(OpCodes.Pop, null));// this pop is to check if its working we need to add some real logic in but is just test

        //for(int i = 0; i < myInstructs.Count; i++)
        //{
        //    Verse.Log.Warning(myInstructs[i].ToString());
        //}


        lineList.InsertRange(0, myInstructs);

        return lineList;
    }
}
[HarmonyPatch(typeof(Verse.PlayDataLoader))]
[HarmonyPatch("DoPlayLoad")]
public static class DataLoader_Patch
{
    [HarmonyTranspiler]
    static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> lines, ILGenerator il)
    {
        List<CodeInstruction> lineList = new List<CodeInstruction>(lines);
        int adjustPoint = 0;
        bool found = false;
        while (!found && adjustPoint < lineList.Count) 
        {
            adjustPoint += 1;
            if (lineList[adjustPoint].operand != null)
            {
                if (lineList[adjustPoint].operand.ToString() == "Other def binding, resetting and global operations (post-resolve).")
                {
                    found = true;
                }
            }
        }
        adjustPoint += 3;
        
        List<CodeInstruction> myInstructs = new List<CodeInstruction>();
        
        
        myInstructs.Add(CodeInstruction.Call(typeof(CombatRework.DamageDefAdjustManager), "onLoad"));

        myInstructs.Add(new CodeInstruction(OpCodes.Pop, null));

        lineList.InsertRange(adjustPoint, myInstructs);
        //okay this is where we need to add our shielddamage onload function

        return lineList;
    }
}