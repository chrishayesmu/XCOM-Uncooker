/*******************************************************************************
 * UIStrategyHUD_FacilitySubMenu generated by Eliot.UELib using UE Explorer.
 * Eliot.UELib ? 2009-2015 Eliot van Uytfanghe. All rights reserved.
 * http://eliotvu.com
 *
 * All rights belong to their respective owners.
 *******************************************************************************/
class UIStrategyHUD_FacilitySubMenu extends UI_FxsPanel
    hidecategories(Navigation)
    implements(IScreenMgrInterface);

struct UIFacilitySubMenuOption
{
    var int iIndex;
    var string strLabel;
    var int iState;
    var string strHelp;

    structdefaultproperties
    {
        iIndex=-1
        strLabel="Default Label"
    }
};

var protected int m_iCurrentSelection;
var protected array<UIFacilitySubMenuOption> m_arrUIOptions;
var protected int m_iView;
var protected bool m_bIsFocused;
var private bool bItemsLoaded;

defaultproperties
{
    m_bIsFocused=true
    s_name=subMenuMC
}