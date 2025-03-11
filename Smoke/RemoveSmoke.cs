using BreatheEasy.Fuel;
using HarmonyLib;
using UnityEngine;
using static BreatheEasy.BreatheEasyPlugin;

namespace BreatheEasy.Smoke;

[HarmonyPatch(typeof(SmokeSpawner), nameof(SmokeSpawner.Spawn))]
static class SmokeSpawnerSpawnPatch
{
    static bool Prefix(SmokeSpawner __instance, float time)
    {
        if (!DisableSmoke.Value.IsOn()) return true;
        // Keep internal timing consistent (so IsBlocked doesn’t return an unintended value)
        __instance.m_lastSpawnTime = time;
        return false;
    }
}

[HarmonyPatch(typeof(SmokeSpawner), nameof(SmokeSpawner.IsBlocked))]
static class SmokeSpawnerIsBlockedPatch
{
    static bool Prefix(SmokeSpawner __instance, ref bool __result)
    {
        if (DisableSmoke.Value.IsOn())
        {
            __result = false;
            return false;
        }

        return true;
    }
}

[HarmonyPatch(typeof(CookingStation), nameof(CookingStation.Awake))]
static class CookingStationDisableSmokeChild_JustInCase
{
    static void Postfix(CookingStation __instance, ref ZNetView ___m_nview)
    {
        StayLit.SetToMaxFuel(__instance, ___m_nview);

        if (DisableSmoke.Value.IsOff())
            return;

        Transform smokeChild = __instance.transform.Find("SmokeSpawner");
        if (smokeChild == null) return;
        SmokeSpawner spawner = smokeChild.GetComponent<SmokeSpawner>();
        if (spawner != null)
        {
            spawner.enabled = false;
        }
    }
}

[HarmonyPatch(typeof(Smelter), nameof(Smelter.Awake))]
static class SmelterDisableSmokeChild_JustInCase
{
    static void Postfix(Smelter __instance, ref ZNetView ___m_nview)
    {
        StayLit.SetToMaxFuel(__instance, ___m_nview);
        if (DisableSmoke.Value.IsOn() && __instance.m_smokeSpawner != null)
        {
            __instance.m_smokeSpawner.enabled = false;
        }
    }
}

[HarmonyPatch(typeof(Fermenter), nameof(Fermenter.Awake))]
static class FermenterAwakePatch
{
    static void Postfix(Fermenter __instance)
    {
        if (__instance.m_fermentingObject == null) return;
        Transform? smokeObject = __instance.m_fermentingObject.transform.Find("smoke");
        if (smokeObject != null)
            smokeObject.gameObject.SetActive(DisableSmoke.Value.IsOn());
    }
}