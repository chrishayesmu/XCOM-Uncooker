/*******************************************************************************
 * UIBuildItem generated by Eliot.UELib using UE Explorer.
 * Eliot.UELib ? 2009-2015 Eliot van Uytfanghe. All rights reserved.
 * http://eliotvu.com
 *
 * All rights belong to their respective owners.
 *******************************************************************************/
class UIBuildItem extends UI_FxsScreen
    hidecategories(Navigation)
    implements(IScreenMgrInterface);

struct UIOption
{
    var int iIndex;
    var string strLabel;
    var int iQuantity;
    var int iState;
    var string strHelp;

    structdefaultproperties
    {
        iIndex=-1
        strLabel="Default Label"
    }
};

var const localized string m_strItemLabel;
var const localized string m_strQuantityLabel;
var const localized string m_strConfirm;
var const localized string m_strDetailLabel;
var string m_strCameraTag;
var name DisplayTag;
var private int m_iCurrentSelection;
var protected array<UIOption> m_arrUIOptions;
var protected int m_iView;
var bool m_bSetCancelDisabled;

defaultproperties
{
    m_strCameraTag="Engineering_UIDisplayCam"
    DisplayTag=UIDisplay_Engineering
    s_package="/ package/gfxBuildItem/BuildItem"
    s_screenId="gfxBuildItem"
    e_InputState=eInputState_Evaluate
    s_name=theBuildScreen
}