/*******************************************************************************
 * XGBase generated by Eliot.UELib using UE Explorer.
 * Eliot.UELib ? 2009-2015 Eliot van Uytfanghe. All rights reserved.
 * http://eliotvu.com
 *
 * All rights belong to their respective owners.
 *******************************************************************************/
class XGBase extends XGStrategyActor
    config(GameData)
    notplaceable
    hidecategories(Navigation);

const ALIENCONTAINMENT_ANIMMAP = "Anim_AlienContainment";

enum ETileState
{
    eTileState_None,
    eTileState_Accessible,
    eTileState_NoAccess,
    eTileState_NoExcavation,
    eTileState_MAX
};

struct TTerrainTile
{
    var int X;
    var int Y;
    var int iType;
    var bool bSecondTile;
    var int iTileState;
    var bool bExcavation;
    var bool bConstruction;
};

struct TFacilityTile
{
    var int X;
    var int Y;
    var int iFacility;
    var bool bRemoval;
};

struct TBaseCost
{
    var int iEngineering;
    var int iCash;
    var int iPower;
    var int iHours;
};

struct CheckpointRecord
{
    var array<TFacilityTile> m_arrFacilities;
    var array<TTerrainTile> m_arrTiles;
    var array<int> m_arrSteamTiles;
    var bool m_bInterrogationQueued;
};

var array<TFacilityTile> m_arrFacilities;
var array<TTerrainTile> m_arrTiles;
var array<int> m_arrSteamTiles;
var SkeletalMeshActor m_kCinDummy;
var EItemType m_currAlienCaptive;
var const localized string m_strLabelInsufficientFunds;
var bool m_bInterrogationQueued;