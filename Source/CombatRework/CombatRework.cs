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
            DamageDefAdjustManager.onLoad();
            Verse.Log.Warning("HEY");
            Verse.Log.Warning("HEY2: " + nameof(ArmorUtility.GetPostArmorDamage));
            harmony.PatchAll();
        }
    }
}
[HarmonyPatch(typeof(DamageWorker_AddInjury))]
[HarmonyPatch("ApplyDamageToPart")]
public static class DamageWroker_AddInjury__ApplyDamage_Patch
{
    [HarmonyTranspiler]
    static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> lines, ILGenerator il)
    {
        var lineList = new List<CodeInstruction>(lines);

        //for (int i = 0; i < lineList.Count; i++)
        //{
        //    Verse.Log.Warning(lineList[i].ToString());
        //}

        bool found = false;
        int replacePoint = 0;
        while(!found && replacePoint < lineList.Count)
        {
            replacePoint++;
            if (lineList[replacePoint].ToString().Contains("GetPostArmorDamage")) found = true;
        }

        List<CodeInstruction> myInstructs = new List<CodeInstruction>();


        //dinfo.Weapon.defname
        myInstructs.Add(new CodeInstruction(OpCodes.Ldarga_S, 1));
        myInstructs.Add(CodeInstruction.Call(typeof(DamageInfo), "get_Weapon"));
        myInstructs.Add(new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(ThingDef), "defName")));

        //DamageDefAdjustManager.GetPostArmorDamage(pawn, num, dinfo.ArmorPenetrationInt, dinfo.HitPart, ref damageDef, out deflectedByMetalArmor, out var diminishedByMetalArmor, string dinfo.Weapon.defname);

        myInstructs.Add(CodeInstruction.Call(typeof(DamageDefAdjustManager), "GetPostArmorDamage"));

        //for(int i = 0; i < myInstructs.Count; i++)
        //{
        //    Verse.Log.Warning(myInstructs[i].ToString());
        //}

        lineList.RemoveAt(replacePoint);
        lineList.InsertRange(replacePoint, myInstructs);

        //for (int i = 0; i < lineList.Count; i++)
        //{
        //    Verse.Log.Warning(lineList[i].ToString());
        //}

        return lineList;
    }
}
//[HarmonyPatch(typeof(Verse.PlayDataLoader))]
//[HarmonyPatch("DoPlayLoad")]
//public static class DataLoader_Patch
//{
//    [HarmonyTranspiler]
//    static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> lines, ILGenerator il)
//    {
//        List<CodeInstruction> lineList = new List<CodeInstruction>(lines);
//        int adjustPoint = 0;
//        bool found = false;
//        while (!found && adjustPoint < lineList.Count) 
//        {
//            adjustPoint += 1;
//            if (lineList[adjustPoint].operand != null)
//            {
//                if (lineList[adjustPoint].operand.ToString() == "Other def binding, resetting and global operations (post-resolve).")
//                {
//                    found = true;
//                }
//            }
//        }
//        adjustPoint += 3;
        
//        List<CodeInstruction> myInstructs = new List<CodeInstruction>();
        
        
//        myInstructs.Add(CodeInstruction.Call(typeof(CombatRework.DamageDefAdjustManager), "onLoad"));

//        myInstructs.Add(new CodeInstruction(OpCodes.Pop, null));

//        lineList.InsertRange(adjustPoint, myInstructs);
//        //okay this is where we need to add our shielddamage onload function

//        return lineList;
//    }
//}