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
        
        //call DamagDefAdjustManager.GetPostArmorDamage(dinfo.weapon.defname)
        myInstructs.Add(CodeInstruction.Call(typeof(DamageDefAdjustManager), "GetPostArmorDamage"));

        lineList.RemoveAt(replacePoint);
        lineList.InsertRange(replacePoint, myInstructs);

        return lineList;
    }
}
[HarmonyPatch(typeof(RimWorld.CompProjectileInterceptor))]
[HarmonyPatch("CheckIntercept")]
public static class SheildIntercept_Patch
{
    [HarmonyTranspiler]
    static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> lines, ILGenerator il)
    {
        //this is the patch for damage for shields that are not on a character, like mechshields and broadshields

        List<CodeInstruction> lineList = new List<CodeInstruction>(lines);
        int adjustPoint = 0;
        int found = 0;
        while (found < 2 && adjustPoint < lineList.Count)
        {
            adjustPoint += 1;
            
            if (lineList[adjustPoint].opcode == OpCodes.Ble_S)
            {
                found += 1;
            }

            if (lineList[adjustPoint].ToString().Contains("TriggerEffecter")) found = 200;
        }
        //adjustPoint += 1;

        List<CodeInstruction> myInstructs = new List<CodeInstruction>();

        lineList.InsertRange(0, myInstructs);
        //okay this is where we need to add our shielddamage onload function

        return lineList;
    }
}

[HarmonyPatch(typeof(RimWorld.CompShield))]
[HarmonyPatch("PostPreApplyDamage")]
public static class CompShield_PostPreApplyDamage_Patch
{
    [HarmonyTranspiler]
    static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> lines, ILGenerator il)
    {
        //this is the patch for the shieldpack damage
        List<CodeInstruction> lineList = new List<CodeInstruction>(lines);

        List<CodeInstruction> myInstructs = new List<CodeInstruction>();


        //damaginfo.weapon.defname
        myInstructs.Add(new CodeInstruction(OpCodes.Ldarga_S, 1));
        myInstructs.Add(CodeInstruction.Call(typeof(DamageInfo), "get_Weapon"));
        myInstructs.Add(new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(ThingDef), "defName")));
        //DamageDefAdjustManager.pulldamagedef(damageinfo.weapon)
        myInstructs.Add(CodeInstruction.Call(typeof(DamageDefAdjustManager), "partTwo"));
        //pop
        //myInstructs.Add(new CodeInstruction(OpCodes.Pop, null));

        lineList.InsertRange(0, myInstructs);

        return lineList;
    }
}