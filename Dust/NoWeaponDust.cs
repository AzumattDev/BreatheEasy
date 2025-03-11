using HarmonyLib;
using static BreatheEasy.BreatheEasyPlugin;

namespace BreatheEasy.Dust;

[HarmonyPatch(typeof(Attack), nameof(Attack.DoAreaAttack))]
static class AttackDoAreaAttackPatch
{
    static void Prefix(Attack __instance)
    {
        if (IsConflictingModLoaded(ConflictingModConstants.NoWeaponDust)) return;
        if (!__instance.m_character.IsPlayer()) return;
        NoWeaponDust.DisableEffectBasedOnConfig(__instance.m_hitTerrainEffect, RemoveHitTerrainEffects.Value);
        NoWeaponDust.DisableEffectBasedOnConfig(__instance.m_weapon.m_shared.m_hitTerrainEffect, RemoveHitTerrainEffects.Value);
        NoWeaponDust.DisableEffectBasedOnConfig(__instance.m_hitEffect, RemoveHitEffects.Value);
        NoWeaponDust.DisableEffectBasedOnConfig(__instance.m_weapon.m_shared.m_hitEffect, RemoveHitEffects.Value);
        NoWeaponDust.DisableEffectBasedOnConfig(__instance.m_triggerEffect, RemoveTriggerEffects.Value);
        NoWeaponDust.DisableEffectBasedOnConfig(__instance.m_weapon.m_shared.m_triggerEffect, RemoveTriggerEffects.Value);
        NoWeaponDust.DisableEffectBasedOnConfig(__instance.m_startEffect, RemoveStartEffects.Value);
        NoWeaponDust.DisableEffectBasedOnConfig(__instance.m_weapon.m_shared.m_startEffect, RemoveStartEffects.Value);
    }
}

[HarmonyPatch(typeof(Attack), nameof(Attack.DoNonAttack))]
static class AttackDoNonAttackPatch
{
    static void Prefix(Attack __instance)
    {
        if (IsConflictingModLoaded(ConflictingModConstants.NoWeaponDust)) return;
        if (!__instance.m_character.IsPlayer()) return;

        NoWeaponDust.DisableEffectBasedOnConfig(__instance.m_triggerEffect, RemoveTriggerEffects.Value);
        NoWeaponDust.DisableEffectBasedOnConfig(__instance.m_weapon.m_shared.m_triggerEffect, RemoveTriggerEffects.Value);
        NoWeaponDust.DisableEffectBasedOnConfig(__instance.m_startEffect, RemoveStartEffects.Value);
        NoWeaponDust.DisableEffectBasedOnConfig(__instance.m_weapon.m_shared.m_startEffect, RemoveStartEffects.Value);
    }
}

[HarmonyPatch(typeof(Attack), nameof(Attack.DoMeleeAttack))]
static class AttackOnAttackTriggerPatch
{
    static void Prefix(Attack __instance)
    {
        if (IsConflictingModLoaded(ConflictingModConstants.NoWeaponDust)) return;
        if (!__instance.m_character.IsPlayer()) return;
        NoWeaponDust.DisableEffectBasedOnConfig(__instance.m_hitTerrainEffect, RemoveHitTerrainEffects.Value);
        NoWeaponDust.DisableEffectBasedOnConfig(__instance.m_weapon.m_shared.m_hitTerrainEffect, RemoveHitTerrainEffects.Value);
        NoWeaponDust.DisableEffectBasedOnConfig(__instance.m_hitEffect, RemoveHitEffects.Value);
        NoWeaponDust.DisableEffectBasedOnConfig(__instance.m_weapon.m_shared.m_hitEffect, RemoveHitEffects.Value);
        NoWeaponDust.DisableEffectBasedOnConfig(__instance.m_triggerEffect, RemoveTriggerEffects.Value);
        NoWeaponDust.DisableEffectBasedOnConfig(__instance.m_weapon.m_shared.m_triggerEffect, RemoveTriggerEffects.Value);
    }
}

internal static class NoWeaponDust
{
    internal static void DisableEffectBasedOnConfig(EffectList effectList, Toggle shouldDisable)
    {
        if (IsConflictingModLoaded(ConflictingModConstants.NoWeaponDust)) return;
        if (effectList == null) return;

        foreach (EffectList.EffectData? effect in effectList.m_effectPrefabs)
        {
            if (effect.m_prefab == null) continue;
            if (effect.m_prefab.name.Contains("vfx_") || effect.m_prefab.name.StartsWith("fx_"))
            {
                effect.m_enabled = shouldDisable == Toggle.Off;
            }
        }
    }
}