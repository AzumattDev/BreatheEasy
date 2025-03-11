using System.Linq;
using HarmonyLib;
using UnityEngine;
using static BreatheEasy.BreatheEasyPlugin;

namespace BreatheEasy.Dust;

#region Just In Case

[HarmonyPatch(typeof(TerrainModifier), nameof(TerrainModifier.OnPlaced))]
[HarmonyPriority(Priority.VeryHigh)]
static class TerrainModifierAwakePatch
{
    static void Prefix(TerrainModifier __instance)
    {
        if(IsConflictingModLoaded(ConflictingModConstants.NoHoeDust)) return;
        if (RemoveAllVFX_Nhd.Value.IsOff()) return;
        // Make the __instance.m_onPlacedEffect an empty EffectList to prevent the hoe dust from spawning
        __instance.m_onPlacedEffect = new EffectList();
    }

    static void Postfix(TerrainModifier __instance)
    {
        if(IsConflictingModLoaded(ConflictingModConstants.NoHoeDust)) return;
        if (RemoveAllVFX_Nhd.Value.IsOff()) return;
        // Make the __instance.m_onPlacedEffect an empty EffectList to prevent the hoe dust from spawning
        __instance.m_onPlacedEffect = new EffectList();
    }
}

[HarmonyPatch(typeof(TerrainOp), nameof(TerrainOp.OnPlaced))]
[HarmonyPriority(Priority.VeryHigh)]
static class TerrainOpAwakePatch
{
    static void Prefix(TerrainOp __instance)
    {
        if (IsConflictingModLoaded(ConflictingModConstants.NoHoeDust)) return;
        if (RemoveAllVFX_Nhd.Value.IsOff()) return;
        // Make the __instance.m_onPlacedEffect an empty EffectList to prevent the hoe dust from spawning
        __instance.m_onPlacedEffect = new EffectList();
    }

    static void Postfix(TerrainOp __instance)
    {
        if (IsConflictingModLoaded(ConflictingModConstants.NoHoeDust)) return;
        if (RemoveAllVFX_Nhd.Value.IsOff()) return;
        // Make the __instance.m_onPlacedEffect an empty EffectList to prevent the hoe dust from spawning
        __instance.m_onPlacedEffect = new EffectList();
    }
}

[HarmonyPatch(typeof(ZNetScene), nameof(ZNetScene.Awake))]
static class ZNetSceneAwakePatch_NoHoeDust
{
    [HarmonyPriority(Priority.Last)]
    static void Postfix(ZNetScene __instance)
    {
        if (IsConflictingModLoaded(ConflictingModConstants.NoHoeDust)) return;
        if (RemoveAllVFX_Nhd.Value.IsOff()) return;
        BreatheEasyLogger.LogDebug("ZNetScene Awake Postfix, turning off build dust");
        foreach (GameObject instanceMPrefab in __instance.m_prefabs.Where(instanceMPrefab =>
                     instanceMPrefab.GetComponent<Piece>()))
        {
            if (!instanceMPrefab.name.Contains("road") && !instanceMPrefab.name.Contains("raise") &&
                !instanceMPrefab.name.Contains("paved")) continue;
            Piece? pieceComponent = instanceMPrefab.GetComponent<Piece>();
            pieceComponent.m_placeEffect.m_effectPrefabs = pieceComponent.m_placeEffect.m_effectPrefabs
                .Where(effect => !effect.m_prefab.name.Contains("vfx")).ToArray();
            BreatheEasyLogger.LogDebug("Removed build dust from " + instanceMPrefab.name +
                                       " Current list of effect prefabs: " + string.Join("\n",
                                           pieceComponent.m_placeEffect.m_effectPrefabs.Select(
                                               effect => effect.m_prefab.name)));
        }
    }
}

#endregion

[HarmonyPatch(typeof(Player), nameof(Player.PlacePiece))]
static class PiecePlacePiecePatch
{
    [HarmonyPriority(Priority.VeryHigh)]
    static void Prefix(Player __instance, ref Piece piece)
    {
        if (IsConflictingModLoaded(ConflictingModConstants.NoHoeDust)) return;
        if (RemoveAllVFX_Nhd.Value.IsOff()) return;
        // cache the piece.gameObject.name
        string pieceName = piece.gameObject.name;
        if (!pieceName.Contains("road") && !pieceName.Contains("raise") && !pieceName.Contains("path_") &&
            !pieceName.Contains("paved")) return;
        BreatheEasyLogger.LogDebug("Preventing hoe dust from spawning " + pieceName);
        piece.m_placeEffect.m_effectPrefabs = piece.m_placeEffect.m_effectPrefabs
            .Where(effect => !effect.m_prefab.name.Contains("vfx")).ToArray();
        BreatheEasyLogger.LogDebug("Removed build dust from " + pieceName +
                                   " Current list of effect prefabs: " + string.Join("\n",
                                       piece.m_placeEffect.m_effectPrefabs.Select(
                                           effect => effect.m_prefab.name)));
    }
}