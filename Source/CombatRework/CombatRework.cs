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
[HarmonyPatch(typeof(Verse.DebugThingPlaceHelper))]
[HarmonyPatch("DebugSpawn")]
public static class DebugThingPlaceHelper_DebugSpawn_Patch
{
    [HarmonyTranspiler]
    static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> lines, ILGenerator il)
    {
        List<CodeInstruction> lineList = new List<CodeInstruction>(lines);

        int found = 0;
        int adjustPoint = 0;
        while (found < 2 && adjustPoint < lineList.Count)
        {
            adjustPoint++;
            if (lineList[adjustPoint].opcode == OpCodes.Brfalse_S) found += 1;
        }
        ++adjustPoint;

        List<CodeInstruction> myInstructs = new List<CodeInstruction>();
        //verse.log.warning("This means...");
        //myInstructs.Add(new CodeInstruction(OpCodes.Ldstr, "This Means that the thing does not generate with a comp quality automatically"));
        //Type[] myParams = { typeof(string) };
        //myInstructs.Add(CodeInstruction.Call(typeof(Verse.Log), "Warning", myParams));
        
        lineList.InsertRange(adjustPoint, myInstructs);

        return lineList;
    }
}
[HarmonyPatch(typeof(Verse.PawnGenerator))]
[HarmonyPatch("PostProcessGeneratedGear")]
public static class PawnGenerator_PostProcessGeneratedGear_Patch
{
    [HarmonyTranspiler]
    static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> lines, ILGenerator il)
    {
        List<CodeInstruction> lineList = new List<CodeInstruction>(lines);

        bool found = false;
        int adjustPoint = 0;
        while (!found && adjustPoint < lineList.Count)
        {
            adjustPoint++;
            if (lineList[adjustPoint].opcode == OpCodes.Brfalse_S) found = true;
        }
        ++adjustPoint;

        List<CodeInstruction> myInstructs = new List<CodeInstruction>();
        //verse.log.warning("This means...");
        //myInstructs.Add(new CodeInstruction(OpCodes.Ldstr, "This Means that the thing does not generate with a comp quality automatically || In PostProcessGeneratedGear"));
        //Type[] myParams = { typeof(string) };
        //myInstructs.Add(CodeInstruction.Call(typeof(Verse.Log), "Warning", myParams));


        //print(thing.thingdef.defname)
        //myInstructs.Add(new CodeInstruction(OpCodes.Ldarg_0, null));
        //myInstructs.Add(new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(Thing), "def")));
        //myInstructs.Add(new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(ThingDef), "defName")));
        //myInstructs.Add(CodeInstruction.Call(typeof(Verse.Log), "Warning", myParams));

        //print(pawn.name.fullstring)
        //myInstructs.Add(new CodeInstruction(OpCodes.Ldarg_1, null));
        //myInstructs.Add(CodeInstruction.Call(typeof(Verse.Pawn), "get_Name"));
        //myInstructs.Add(CodeInstruction.Call(typeof(Verse.Name), "get_ToStringFull"));
        //myInstructs.Add(CodeInstruction.Call(typeof(Verse.Log), "Warning", myParams));
        lineList.InsertRange(adjustPoint, myInstructs);

        return lineList;
    }
}
[HarmonyPatch(typeof(Verse.ThingWithComps))]
[HarmonyPatch("PostMake")]
public static class ThingWithComps_PostMake_Patch
{
    [HarmonyTranspiler]
    static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> lines, ILGenerator il)
    {
        List<CodeInstruction> lineList = new List<CodeInstruction>(lines);


        List<CodeInstruction> myInstructs = new List<CodeInstruction>();
        //printmyinfo(thingwithcomps);
        //myInstructs.Add(new CodeInstruction(OpCodes.Ldarg_0, null));
        //myInstructs.Add(CodeInstruction.Call(typeof(CombatRework.DamageDefAdjustManager), "printMyInfo"));

        lineList.InsertRange(0, myInstructs);

        return lineList;
    }
}
[HarmonyPatch(typeof(Verse.ThingWithComps))]
[HarmonyPatch("InitializeComps")]
public static class ThingWithComps_InitializeComps_Patch
{
    [HarmonyTranspiler]
    static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> lines, ILGenerator il)
    {
        List<CodeInstruction> lineList = new List<CodeInstruction>(lines);

        bool found = false;
        int adjustPoint = 0;
        //while (!found && adjustPoint < lineList.Count)
        //{
        //    if (lineList[adjustPoint].opcode == OpCodes.Stfld) found = false;
        //}
        adjustPoint += 1;

        List<CodeInstruction> myInstructs = new List<CodeInstruction>();
        //printmyinfo(thingwithcomps);
        //myInstructs.Add(new CodeInstruction(OpCodes.Ldarg_0, null));
        //myInstructs.Add(CodeInstruction.Call(typeof(CombatRework.DamageDefAdjustManager), "printMyInfo"));

        lineList.InsertRange(lineList.Count - 1, myInstructs);

        return lineList;
    }
}
[HarmonyPatch(typeof(Verse.Thing))]
[HarmonyPatch("TakeDamage")]
public static class Thing_TakeDamage_Patch
{
    [HarmonyTranspiler]
    static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> lines, ILGenerator il)
    {
        List<CodeInstruction> lineList = new List<CodeInstruction>(lines);

        bool found = false;
        int adjustPoint = 0;
        //while (!found && adjustPoint < lineList.Count)
        //{
        //    if (lineList[adjustPoint].opcode == OpCodes.Stloc_2) found = true;
        //}
        //adjustPoint += 1;

        List<CodeInstruction> myInstructs = new List<CodeInstruction>();
        //if(thing thing is pawn);
        //myInstructs.Add(new CodeInstruction(OpCodes.Ldarg_0, null));
        //myInstructs.Add(new CodeInstruction(OpCodes.Isinst, typeof(Verse.Pawn)));
        //myInstructs.Add(new CodeInstruction(OpCodes.Stloc_S, 6));

        //myInstructs.Add(new CodeInstruction(OpCodes.Ldloc_S, 6));
        //myInstructs.Add(new CodeInstruction(OpCodes.Brtrue_S, null));
        //int jf1 = myInstructs.Count - 1;

        //applytopawn(dinfo, pawn, 

        //verse.log.warning(this.def.defname)
        //myInstructs.Add(new CodeInstruction(OpCodes.Ldarg_0, null));
        //myInstructs.Add(new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(Thing), "def")));
        //myInstructs.Add(new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(ThingDef), "defName")));
        //Type[] myParams = { typeof(string) };
        //myInstructs.Add(CodeInstruction.Call(typeof(Verse.Log), "Warning", myParams));

        lineList.InsertRange(0, myInstructs);

        return lineList;
    }
}
[HarmonyPatch(typeof(RimWorld.Bullet))]
[HarmonyPatch("Impact")]
public static class Bullet_Impact_Patch
{
    [HarmonyTranspiler]
    static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> lines, ILGenerator il)
    {
        List<CodeInstruction> lineList = new List<CodeInstruction>(lines);

        bool found = false;
        int adjustPoint = 0;
        while (!found && adjustPoint < lineList.Count)
        {
            if (lineList[adjustPoint].ToString().Contains("Verse.DamageInfo")) found = true;
            adjustPoint++;
        }
        adjustPoint += 1;


        List<CodeInstruction> myInstructs = new List<CodeInstruction>();

        myInstructs.Add(new CodeInstruction(OpCodes.Ldarg_0, null));
        myInstructs.Add(CodeInstruction.Call(typeof(CombatRework.DamageDefAdjustManager), "printBullet"));

        lineList.InsertRange(adjustPoint, myInstructs);

        return lineList;
    }
}