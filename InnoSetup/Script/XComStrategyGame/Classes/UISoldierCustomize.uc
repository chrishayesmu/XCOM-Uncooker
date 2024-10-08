/*******************************************************************************
 * UISoldierCustomize generated by Eliot.UELib using UE Explorer.
 * Eliot.UELib ? 2009-2015 Eliot van Uytfanghe. All rights reserved.
 * http://eliotvu.com
 *
 * All rights belong to their respective owners.
 *******************************************************************************/
class UISoldierCustomize extends UI_FxsScreen
    hidecategories(Navigation)
    implements(IScreenMgrInterface);

const FIRST_NAME_MAX_CHARS = 11;
const NICKNAME_NAME_MAX_CHARS = 11;
const LAST_NAME_MAX_CHARS = 15;
const NUM_BUTTONS = 3;
const NUM_SPINNERS = 11;

var const localized string m_strCustomizeFirstName;
var const localized string m_strCustomizeLastName;
var const localized string m_strCustomizeNickName;
var const localized array<localized string> m_arrLanguages;
var XGCustomizeUI m_kLocalMgr;
var string m_strCameraTag;
var name DisplayTag;
var private SkeletalMeshActor m_kCameraRig;
var private Vector m_kCameraRigDefaultLocation;
var float m_kCameraRigMecDistanceOffset;
var float m_kCameraRigMecVerticalOffset;
var float m_kCameraRigMecHorizontalOffset;
var private int m_iView;
var private int m_iCustomizeNameType;
var private XGStrategySoldier m_kSoldier;
var private UIStrategyComponent_SoldierInfo m_kSoldierHeader;
var private UIWidgetHelper m_hWidgetHelper;
var private bool m_bRequestUserInputActive;

defaultproperties
{
    m_strCameraTag="Armory_UIDisplayCam_Customize"
    DisplayTag=UIDisplay_SoldierCustomize
    m_kCameraRigMecDistanceOffset=50.0
    m_kCameraRigMecVerticalOffset=-10.0
    m_kCameraRigMecHorizontalOffset=20.0
    s_package="/ package/gfxSoldierCustomization/SoldierCustomization"
    s_screenId="gfxSoldierCustomization"
    m_bAnimateOutro=false
    e_InputState=eInputState_Evaluate
    s_name=theScreen
}