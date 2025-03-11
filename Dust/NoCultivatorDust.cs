using System.Linq;
using HarmonyLib;
using static BreatheEasy.BreatheEasyPlugin;

namespace BreatheEasy.Dust;

[HarmonyPatch(typeof(ZNetScene), nameof(ZNetScene.Awake))]
static class ZNetSceneAwakePatch_NoCultivatorDust
{
    [HarmonyPriority(Priority.Last)]
    static void Postfix(ZNetScene __instance)
    {
        if(IsConflictingModLoaded(ConflictingModConstants.NoCultivatorDust)) return;
        if(RemoveAllVFX_Ncultd.Value.IsOff()) return;
        // For each PieceTable and for each Piece in each PieceTable remove the place effect
        var cultivator = __instance.GetPrefab("Cultivator").GetComponent<ItemDrop>();
        var cultivatorPieceTable = cultivator.m_itemData.m_shared.m_buildPieces;
        if (cultivatorPieceTable == null) return;
        foreach (var piece in cultivatorPieceTable.m_pieces)
        {
            var prefab = ZNetScene.instance.GetPrefab(Utils.GetPrefabName(piece));
            if (prefab == null) continue;
            prefab.TryGetComponent<Piece>(out var pieceComp);
            if (pieceComp == null) continue;
            pieceComp.m_placeEffect.m_effectPrefabs = pieceComp.m_placeEffect.m_effectPrefabs
                .Where(effect => !effect.m_prefab.name.Contains("vfx")).ToArray();
            BreatheEasyLogger.LogDebug("Removed build dust from " +
                                                                   piece.gameObject.name +
                                                                   " Current list of effect prefabs: " +
                                                                   string.Join("\n",
                                                                       pieceComp.m_placeEffect
                                                                           .m_effectPrefabs
                                                                           .Select(
                                                                               effect => effect.m_prefab
                                                                                   .name)));
        }
    }
}

[HarmonyPatch(typeof(Player), nameof(Player.PlacePiece))]
static class PiecePlacePiecePatch_NoCultivatorDust
{
    [HarmonyPriority(Priority.First)]
    static void Prefix(Player __instance, ref Piece piece)
    {
        if(IsConflictingModLoaded(ConflictingModConstants.NoCultivatorDust)) return;
        if(RemoveAllVFX_Ncultd.Value.IsOff()) return;
        // cache the piece.gameObject.name
        string pieceName = piece.gameObject.name;
        if (!pieceName.Contains("replant") && !pieceName.Contains("cultivate")) return;
        BreatheEasyLogger.LogDebug("Preventing cultivator dust from spawning " + pieceName);
        piece.m_placeEffect.m_effectPrefabs = piece.m_placeEffect.m_effectPrefabs
            .Where(effect => !effect.m_prefab.name.Contains("vfx")).ToArray();
        BreatheEasyLogger.LogDebug("Removed build dust from " + pieceName +
                                                               " Current list of effect prefabs: " + string.Join("\n",
                                                                   piece.m_placeEffect.m_effectPrefabs.Select(
                                                                       effect => effect.m_prefab.name)));
    }
}