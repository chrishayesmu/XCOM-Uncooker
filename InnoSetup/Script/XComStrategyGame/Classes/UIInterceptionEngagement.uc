/*******************************************************************************
 * UIInterceptionEngagement generated by Eliot.UELib using UE Explorer.
 * Eliot.UELib ? 2009-2015 Eliot van Uytfanghe. All rights reserved.
 * http://eliotvu.com
 *
 * All rights belong to their respective owners.
 *******************************************************************************/
class UIInterceptionEngagement extends UI_FxsScreen
    hidecategories(Navigation)
    implements(IScreenMgrInterface);

const SHOT_DURATION = 0.3f;
const MISS_DURATION_EXTENSION = 0.3f;
const ALIEN_SHIP_ID = 0;
const PLAYER_SHIP_ID = 1;

enum eMoveEventType
{
    eUFO_Escape,
    ePlayer_CloseDistance,
    eMoveEventType_MAX
};

enum eGameResult
{
    eUFO_Escaped,
    eUFO_Destroyed,
    ePlayer_Aborted,
    ePlayer_Destroyed,
    eGameResult_MAX
};

enum eDisplayEffectType
{
    eEnhancedAccuracy,
    eDodgeHits,
    eTrack,
    eDisplayEffectType_MAX
};

enum eAbilityStates
{
    eAbilityState_Available,
    eAbilityState_Active,
    eAbilityState_Disabled,
    eAbilityState_Unavailable,
    eAbilityState_MAX
};

struct TAttack
{
    var int iSourceShip;
    var int iTargetShip;
    var int iWeapon;
    var int iDamage;
    var float fDuration;
    var bool bHit;
};

struct TDamageData
{
    var int iShip;
    var int iDamage;
    var int iBulletID;
    var float fTime;
};

var const localized string m_strEstablishingLinkLabel;
var const localized string m_strPlayerDamageLabel;
var const localized string m_strLeaveReportButtonLabel;
var const localized string m_strAbortMission;
var const localized string m_strAbortingMission;
var const localized string m_strAbortedMission;
var const localized string m_strDodgeAbility;
var const localized string m_strAimAbility;
var const localized string m_strTrackAbility;
var const localized string m_strTrackingText;
var const localized string m_strEscapeTimerTitle;
var const localized string m_strResult_UFOCrashed;
var const localized string m_strResult_UFODestroyed;
var const localized string m_strResult_UFOEscaped;
var const localized string m_strResult_UFODisengaged;
var const localized string m_strReport_NoDamage;
var const localized string m_strReport_LightDamage;
var const localized string m_strReport_HeavyDamage;
var const localized string m_strReport_SevereDamage;
var const localized string m_strReport_ShotDown;
var const localized string m_strReport_Title;
var const localized string m_strReport_Subtitle;
var const localized string m_strTimeSufixSymbol;
var const localized string m_strAbilityDescriptions[eDisplayEffectType];

var float INTRO_SEQUENCE_DELAY;
var string m_strCameraTag;
var name DisplayTag;
var float m_fPlaybackTimeElapsed;
var float m_fEnemyEscapeTimer;
var float m_fTotalBattleLength;
var float m_fTrackingTimer;
var int m_iTenthsOfSecondCounter;
var int m_iInterceptorPlaybackIndex;
var int m_iUFOPlaybackIndex;
var int m_iBulletIndex;
var int m_iLastDodgeBulletIndex;
var bool m_bPendingDisengage;
var bool m_bViewingResults;
var bool m_DataInitialized;
var bool m_bIntroSequenceComplete;
var float m_fIntroSequenceTimer;
var array<TDamageData> m_akDamageInformation;
var int m_iDamageDataIndex;
var int m_iView;
var XGInterceptionEngagementUI m_kMgr;
var private XGInterception m_kXGInterception;

defaultproperties
{
    m_strCameraTag="MissionControl_UIDisplayCam_Interception"
    DisplayTag=UIDisplay_Interception
    s_package="/ package/gfxInterception/Interception"
    s_screenId="gfxInterception"
    e_InputState=eInputState_Evaluate
    s_name=theInterceptionScreen
}