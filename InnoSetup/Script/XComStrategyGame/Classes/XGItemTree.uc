/*******************************************************************************
 * XGItemTree generated by Eliot.UELib using UE Explorer.
 * Eliot.UELib ? 2009-2015 Eliot van Uytfanghe. All rights reserved.
 * http://eliotvu.com
 *
 * All rights belong to their respective owners.
 *******************************************************************************/
class XGItemTree extends XGStrategyActor
    config(GameData)
    notplaceable
    hidecategories(Navigation);

var array<TItem> m_arrItems;
var array<TFacility> m_arrFacilities;
var array<TItem> m_arrStaff;
var array<TShip> m_arrShips;
var array<TShipWeapon> m_arrShipWeapons;
var int m_iCurrentCategory;
var XGStrategyActorTag m_kStrategyActorTag;
var const localized string m_strRangeShort;
var const localized string m_strRangeMedium;
var const localized string m_strRangeLong;
var const localized string m_strNotFullyResearched;
var const localized string ItemTypeSummary[EItemType];
var const localized string ShipTypeNames[16];
var const localized string ShipTypeFlavorTxt[16];
var const localized string m_strSizeSmall;
var const localized string m_strSizeMedium;
var const localized string m_strSizeLarge;
var const localized string m_strSizeVeryLarge;