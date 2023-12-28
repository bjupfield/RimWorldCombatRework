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
[HarmonyPatch(typeof(RimWorld.CompProjectileInterceptor))]
[HarmonyPatch("CheckIntercept")]
public static class SheildIntercept_Patch
{
    [HarmonyTranspiler]
    static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> lines, ILGenerator il)
    {
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

        myInstructs.Add(new CodeInstruction(OpCodes.Ldarg, 1));
        myInstructs.Add(CodeInstruction.Call(typeof(Verse.Projectile), "get_DamageAmount"));
        myInstructs.Add(CodeInstruction.Call(typeof(CombatRework.DamageDefAdjustManager), "printString"));
        //myInstructs.Add(new CodeInstruction(OpCodes.Pop, null));

        myInstructs.Add(new CodeInstruction(OpCodes.Ldstr, "FUCK YOU I SHOULD BE IN HERE"));
        Type[] myParams = { typeof(string) };
        myInstructs.Add(CodeInstruction.Call(typeof(Verse.Log), "Warning", myParams));

        lineList.InsertRange(0, myInstructs);
        //okay this is where we need to add our shielddamage onload function

        return lineList;
    }
}
//[HarmonyPatch(typeof(RimWorld.Bullet))]
//[HarmonyPatch("Impact")]
//public static class Bullet_Impact_Patch
//{
//    [HarmonyTranspiler]
//    static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> lines, ILGenerator il)
//    {
//        Verse.Log.Warning("This iniatilized");
//        List<CodeInstruction> lineList = new List<CodeInstruction>(lines);

//        List<CodeInstruction> myInstructs = new List<CodeInstruction>();

//        myInstructs.Add(new CodeInstruction(OpCodes.Ldstr, "FUCK YOU I SHOULD BE IN HERE"));
//        Type[] myParams = { typeof(string) };
//        myInstructs.Add(CodeInstruction.Call(typeof(Verse.Log), "Warning", myParams));

//        lineList.InsertRange(0, myInstructs);
//        return lineList;
//    }
//}
//[HarmonyPatch(typeof(Verse.Thing))]
//[HarmonyPatch("TakeDamage")]
//public static class Thing_TakeDamage_Patch
//{
//    [HarmonyTranspiler]
//    static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> lines, ILGenerator il)
//    {
//        Verse.Log.Warning("This iniatilized");
//        List<CodeInstruction> lineList = new List<CodeInstruction>(lines);

//        List<CodeInstruction> myInstructs = new List<CodeInstruction>();

//        //myInstructs.Add(new CodeInstruction(OpCodes.Ldstr, "FUCK YOU I SHOULD BE IN HERE IN TAKE DAMAGE"));
//        //Type[] myParams = { typeof(string) };
//        //myInstructs.Add(CodeInstruction.Call(typeof(Verse.Log), "Warning", myParams));

//        lineList.InsertRange(0, myInstructs);
//        return lineList;
//    }
//}
[HarmonyPatch(typeof(Verse.DamageWorker))]
[HarmonyPatch("Apply")]
public static class DamageWorker_Apply_Patch
{
    [HarmonyTranspiler]
    static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> lines, ILGenerator il)
    {
        Verse.Log.Warning("This iniatilized Damage Worker Apply");
        List<CodeInstruction> lineList = new List<CodeInstruction>(lines);

        List<CodeInstruction> myInstructs = new List<CodeInstruction>();

        myInstructs.Add(new CodeInstruction(OpCodes.Ldstr, "FUCK YOU I SHOULD BE IN HERE in damageworkerapply"));
        Type[] myParams = { typeof(string) };
        myInstructs.Add(CodeInstruction.Call(typeof(Verse.Log), "Warning", myParams));


        myInstructs.Add(new CodeInstruction(OpCodes.Ldarga_S, 1));
        myInstructs.Add(CodeInstruction.Call(typeof(Verse.Thing), "get_DescriptionFlavor"));
        // myInstructs.Add(new CodeInstruction(OpCodes.Ldsfld, AccessTools.Field(typeof(ThingDef), "defName")));
        //myInstructs.Add(CodeInstruction.Call(typeof(Verse.Log), "Warning", myParams));
        myInstructs.Add(new CodeInstruction(OpCodes.Pop, null));

        lineList.InsertRange(0, myInstructs);
        return lineList;
    }
}
//the above function doesnt seem to be taking damage for the shield, but the ones above it do seem to be having the bullets damage filterered through it