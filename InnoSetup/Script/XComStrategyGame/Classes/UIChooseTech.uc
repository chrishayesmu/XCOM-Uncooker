/*******************************************************************************
 * UIChooseTech generated by Eliot.UELib using UE Explorer.
 * Eliot.UELib ? 2009-2015 Eliot van Uytfanghe. All rights reserved.
 * http://eliotvu.com
 *
 * All rights belong to their respective owners.
 *******************************************************************************/
class UIChooseTech extends UI_FxsScreen
    hidecategories(Navigation)
    implements(IScreenMgrInterface);

struct UIOption
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

var const localized string m_strScienceTechSelectTitle;
var const localized string m_strCreditArchiveTitle;
var const localized string m_strConfirm;
var const localized string m_strExit;
var XGResearchUI m_kLocalMgr;
var string m_strCameraTag;
var name DisplayTag;
var private int m_iCurrentSelection;
var protected array<UIOption> m_arrUIOptions;
var protected int m_iView;

defaultproperties
{
    m_strCameraTag="ScienceLab_UIDisplayCam"
    DisplayTag=UIDisplay_ScienceLabs
    s_package="/ package/gfxChooseTech/ChooseTech"
    s_screenId="gfxChooseTech"
    e_InputState=eInputState_Evaluate
    s_name=theChooseResearch
}