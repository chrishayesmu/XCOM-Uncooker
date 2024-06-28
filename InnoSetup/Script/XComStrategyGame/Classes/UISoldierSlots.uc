/*******************************************************************************
 * UISoldierSlots generated by Eliot.UELib using UE Explorer.
 * Eliot.UELib ? 2009-2015 Eliot van Uytfanghe. All rights reserved.
 * http://eliotvu.com
 *
 * All rights belong to their respective owners.
 *******************************************************************************/
class UISoldierSlots extends UI_FxsShellScreen
    hidecategories(Navigation);

const MAX_NUM_PATIENTS = 3;

enum UISlotOptionCategories
{
    eSOCat_Name,
    eSOCat_Status,
    eSOCat_ButtonLabel,
    eSOCat_MAX
};

struct UISlotOption
{
    var int iIndex;
    var string strName;
    var string strStatus;
    var string strButtonLabel;
    var bool IsDisabled;

    structdefaultproperties
    {
        iIndex=-1
        strName="Unset Name"
        strStatus="Unset Status"
    }
};

var name DisplayTag;
var string m_strCameraTag;
var const localized string m_strSoldierSlotBaseStatusLabel;
var protected int m_iCurrentSelection;
var protected int m_iNumAvailableSlots;
var protected array<UISlotOption> m_arrUIOptions;

defaultproperties
{
    m_iCurrentSelection=-1
    s_package="/ package/gfxSoldierSlots/SoldierSlots"
    s_screenId="gfxSoldierSlots"
    s_name=theScreen
}