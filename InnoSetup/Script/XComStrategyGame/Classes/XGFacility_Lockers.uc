/*******************************************************************************
 * XGFacility_Lockers generated by Eliot.UELib using UE Explorer.
 * Eliot.UELib ? 2009-2015 Eliot van Uytfanghe. All rights reserved.
 * http://eliotvu.com
 *
 * All rights belong to their respective owners.
 *******************************************************************************/
class XGFacility_Lockers extends XGFacility
    config(GameData)
    notplaceable
    hidecategories(Navigation);

enum EInventorySlot
{
    eInvSlot_Armor,
    eInvSlot_Pistol,
    eInvSlot_Large,
    eInvSlot_Small,
    eInvSlot_MAX
};

struct TLockerItem
{
    var int iItem;
    var int iQuantity;
    var bool bTechLocked;
    var bool bClassLocked;
    var ESoldierClass eClassLock;
};

var string m_strError;
var bool m_bNarrArcWarning;