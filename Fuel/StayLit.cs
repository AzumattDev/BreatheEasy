using HarmonyLib;
using UnityEngine;
using static BreatheEasy.BreatheEasyPlugin;

namespace BreatheEasy.Fuel;

public static class StayLit
{
    public static void SetToMaxFuel(Component instance, ZNetView netView)
    {
        if (FireplacesStayLit.Value.IsOff()) return;

        Smelter? smelter = instance.GetComponent<Smelter>();
        CookingStation? cookingStation = instance.GetComponent<CookingStation>();

        if (netView == null || netView.GetZDO() == null) return;

        // Use ShouldConsumeFuel to determine if we should set max fuel
        if (smelter != null && !ShouldConsumeFuel(instance))
            netView.GetZDO().Set(ZDOVars.s_fuel, smelter.m_maxFuel);
        else if (cookingStation != null && !ShouldConsumeFuel(instance))
        {
            BreatheEasyLogger.LogInfo(" Setting fuel to max for " + instance.name);
            netView.GetZDO().Set(ZDOVars.s_fuel, cookingStation.m_maxFuel);
        }
    }

    internal static bool ShouldConsumeFuel(Component instance)
    {
        if (FireplacesStayLit.Value.IsOff()) return true;

        if (instance.GetComponent<Smelter>() != null)
            return SmeltersUseFuel.Value.IsOn();
        if (instance.GetComponent<CookingStation>() != null)
            return CookingStationsUseFuel.Value.IsOn();
        if (instance.GetComponent<Fireplace>() != null)
            return FireplacesUseFuel.Value.IsOn();

        return true;
    }
}

[HarmonyPatch(typeof(Fireplace), nameof(Fireplace.IsBurning))]
public static class Fireplace_IsBurning_AlwaysLit_Patch
{
    static void Postfix(Fireplace __instance, ref bool __result)
    {
        if (FireplacesStayLit.Value.IsOn() && !StayLit.ShouldConsumeFuel(__instance))
        {
            // Only override burning state if we're not using fuel
            __result = true;
        }
    }
}

/// <summary>
/// Prevents fuel consumption by skipping the logic in UpdateFireplace.
/// </summary>
[HarmonyPatch(typeof(Fireplace), nameof(Fireplace.UpdateFireplace))]
public static class Fireplace_UpdateFireplace_NoFuelDrain_Patch
{
    static bool Prefix(Fireplace __instance)
    {
        if (!FireplacesStayLit.Value.IsOn() || StayLit.ShouldConsumeFuel(__instance))
            return true; // Use normal fuel consumption

        // Instead of subtracting fuel, we simply update the visuals.

        // Enable the main fire object.
        if (__instance.m_enabledObject != null)
            __instance.m_enabledObject.SetActive(true);

        // Enable the high-intensity flame object.
        if (__instance.m_enabledObjectHigh != null)
            __instance.m_enabledObjectHigh.SetActive(true);

        // Optionally disable the low-intensity flame (which may indicate low fuel).
        if (__instance.m_enabledObjectLow != null)
            __instance.m_enabledObjectLow.SetActive(false);

        // Hide the "empty" or "off" fire visuals.
        if (__instance.m_emptyObject != null)
            __instance.m_emptyObject.SetActive(false);

        // Force the fuel visual indicators (if any) into a "full" state.
        if (__instance.m_fullObject != null)
            __instance.m_fullObject.SetActive(true);
        if (__instance.m_halfObject != null)
            __instance.m_halfObject.SetActive(false);

        // Skip the original UpdateFireplace method to avoid any fuel reduction.
        return false;
    }
}

[HarmonyPatch(typeof(Fireplace), nameof(Fireplace.GetHoverText))]
public static class Fireplace_GetHoverText_Patch
{
    static void Postfix(Fireplace __instance, ref string __result)
    {
        if (FireplacesStayLit.Value.IsOn() && ShowFuelInHoverText.Value.IsOff())
        {
            __result = Localization.instance.Localize(__instance.m_name);
        }
    }
}

/*[HarmonyPatch(typeof(CookingStation), nameof(CookingStation.UpdateVisual))]
public static class CookingStation_UpdateVisual_InfiniteFuel_Patch
{
    static void Prefix(CookingStation __instance, ref bool fireLit)
    {
        if (FireplacesStayLit.Value.IsOn())
        {
            fireLit = true;
        }
    }

    static void Postfix(CookingStation __instance, ref bool fireLit)
    {
        if (!FireplacesStayLit.Value.IsOn()) return;
        /*if (Configs.ConfigCheck(__instance.name))
            {#1#
        // Force the cooking station to appear lit.
        fireLit = true;
        __instance.m_haveFireObject?.SetActive(true);
        __instance.m_haveFuelObject?.SetActive(true);
        // }
    }
}*/
/*[HarmonyPatch(typeof(CookingStation), nameof(CookingStation.SetFuel))]
public static class CookingStation_SetFuel_InfiniteFuel_Patch
{
    static bool Prefix(CookingStation __instance, ref float fuel)
    {
        if (FireplacesStayLit.Value.IsOn())
        {
            /*if (Configs.ConfigCheck(__instance.name))
            {#1#
            SetToMaxFuel(__instance, __instance.m_nview);
            return false; // Skip the original SetFuel
            //}
        }

        return true;
    }
}*/

[HarmonyPatch(typeof(CookingStation), nameof(CookingStation.GetFuel))]
public static class CookingStation_GetFuel_InfiniteFuel_Patch
{
    static bool Prefix(CookingStation __instance, ref float __result)
    {
        if (FireplacesStayLit.Value.IsOn() && !StayLit.ShouldConsumeFuel(__instance))
        {
            __result = __instance.m_maxFuel;
            return false; // Skip the original GetFuel
        }

        return true;
    }
}

[HarmonyPatch(typeof(CookingStation), nameof(CookingStation.OnHoverFuelSwitch))]
static class CookingStationOnHoverFuelSwitchPatch
{
    static void Postfix(CookingStation __instance, ref string __result)
    {
        if (FireplacesStayLit.Value.IsOn() && ShowFuelInHoverText.Value.IsOff())
        {
            __result = Localization.instance.Localize($"{__instance.m_name}");
        }
    }
}

/*[HarmonyPatch(typeof(Smelter), nameof(Smelter.SetFuel))]
public static class Smelter_SetFuel_InfiniteFuel_Patch
{
    static bool Prefix(Smelter __instance, ref float fuel)
    {
        if (FireplacesStayLit.Value.IsOn())
        {
            /*if (Configs.ConfigCheck(__instance.name))
            {#1#
            SetToMaxFuel(__instance, __instance.m_nview);
            return false; // Skip the original SetFuel
            //}
        }

        return true;
    }
}*/

[HarmonyPatch(typeof(Smelter), nameof(Smelter.GetFuel))]
public static class Smelter_GetFuel_InfiniteFuel_Patch
{
    static bool Prefix(Smelter __instance, ref float __result)
    {
        if (FireplacesStayLit.Value.IsOn() && !StayLit.ShouldConsumeFuel(__instance))
        {
            __result = __instance.m_maxFuel;
            return false; // Skip the original GetFuel
        }

        return true;
    }
}

[HarmonyPatch(typeof(Smelter), nameof(Smelter.OnHoverAddFuel))]
static class SmelterOnHoverFuelSwitchPatch
{
    static void Postfix(Smelter __instance, ref string __result)
    {
        if (FireplacesStayLit.Value.IsOn() && ShowFuelInHoverText.Value.IsOff())
        {
            __result = Localization.instance.Localize($"{__instance.m_name}");
        }
    }
}

[HarmonyPatch(typeof(Smelter), nameof(Smelter.UpdateState))]
public static class Smelter_UpdateState_InfiniteFuel_Patch
{
    static void Postfix(Smelter __instance)
    {
        if (FireplacesStayLit.Value.IsOn() && !StayLit.ShouldConsumeFuel(__instance))
        {
            // For example, ensure the "have fuel" object is active.
            __instance.m_haveFuelObject?.SetActive(true);
            // (You could add more visual overrides as needed.)
        }
    }
}