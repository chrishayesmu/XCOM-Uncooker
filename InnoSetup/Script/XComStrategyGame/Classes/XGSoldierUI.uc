/*******************************************************************************
 * XGSoldierUI generated by Eliot.UELib using UE Explorer.
 * Eliot.UELib ? 2009-2015 Eliot van Uytfanghe. All rights reserved.
 * http://eliotvu.com
 *
 * All rights belong to their respective owners.
 *******************************************************************************/
class XGSoldierUI extends XGScreenMgr
    config(GameData)
    notplaceable
    hidecategories(Navigation);

const STANDARD_ABILITY_COLUMNS = 7;
const PSI_ABILITY_COLUMNS = 3;
const SOLDIER_SWITCH_SPEED = 0.2f;

enum EBaseView
{
    eSoldierView_MainMenu,
    eSoldierView_Promotion,
    eSoldierView_PsiPromotion,
    eSoldierView_MECPromotion,
    eSoldierView_GeneMods,
    eSoldierView_Gear,
    eSoldierView_Customize,
    eSoldierView_Medals,
    eSoldierView_Dismiss,
    eSoldierView_MAX
};

struct TSoldierHeader
{
    var TText txtName;
    var TText txtNickname;
    var TLabeledText txtStatus;
    var TLabeledText txtOffense;
    var TLabeledText txtDefense;
    var TLabeledText txtHP;
    var TLabeledText txtSpeed;
    var TLabeledText txtWill;
    var TLabeledText txtStrength;
    var TLabeledText txtCritShot;
    var TText txtOffenseMod;
    var TText txtDefenseMod;
    var TText txtHPMod;
    var TText txtSpeedMod;
    var TText txtWillMod;
    var TText txtStrengthMod;
    var TText txtCritShotMod;
};

struct TSoldierDoll
{
    var int iFlag;
    var TImage imgFlag;
    var TImage imgSoldier;
};

struct TSoldierMainMenu
{
    var array<int> arrOptions;
    var TMenu mnuOptions;
};

struct TSoldierAbilities
{
    var TTableMenu tblAbilities;
};

struct TSoldierPerks
{
    var TText txtNickname;
    var TTableMenu tblNewPerks;
};

struct TInventoryOption
{
    var int iOptionType;
    var bool bHighlight;
    var bool bShowItemCard;
    var bool bInfinite;
    var TImage imgItem;
    var TText txtName;
    var TText txtLabel;
    var TText txtQuantity;
    var string strHelp;
    var int iState;
    var int iSlot;
    var int iItem;
};

struct TSoldierGear
{
    var TText txtTitle;
    var int iHighlight;
    var bool bDataDirty;
    var array<TInventoryOption> arrOptions;
};

struct TSoldierLocker
{
    var TText txtTitle;
    var TText txtMsg;
    var int iHighlight;
    var bool bIsSelected;
    var array<TInventoryOption> arrOptions;
};

struct TAbilityTree
{
    var TText txtName;
    var TText txtLabel;
    var int branch;
    var int Option;
    var array<int> arrOptions;
    var string testVar;
};

var const localized string m_strDismissSoldierDialogTitle;
var const localized string m_strDismissSoldierDialogText;
var const localized string m_strDismissTankDialogTitle;
var const localized string m_strDismissTankDialogText;
var const localized string m_strLockedAbilityLabel;
var const localized string m_strLockedAbilityDescription;
var const localized string m_strLockedGeneModDescription;
var const localized string m_strLockedPsiAbilityDescription;
var TSoldierHeader m_kHeader;
var TSoldierMainMenu m_kMainMenu;
var TSoldierDoll m_kDoll;
var TSoldierAbilities m_kAbilities;
var TSoldierPerks m_kPerks;
var TSoldierGear m_kGear;
var TSoldierLocker m_kLocker;
var XGStrategySoldier m_kSoldier;
var TButtonBar m_kButtonBar;
var TAbilityTree m_kSoldierAbilityTree;
var TAbilityTree m_kSoldierPsiTree;
var bool m_bUrgeGollop;
var bool m_bPreventSoldierCycling;
var bool m_bReturnToDebriefUI;
var bool m_bCovertOperativeMode;
var const localized string m_strLabelPressRTForPromote;
var const localized string m_strLabelStatus;
var const localized string m_strLabelOffense;
var const localized string m_strLabelDefense;
var const localized string m_strLabelHPSPACED;
var const localized string m_strLabelMobility;
var const localized string m_strLabelWill;
var const localized string m_strLabelStrength;
var const localized string m_strLabelCritical;
var const localized string m_strLoadout;
var const localized string m_strNoEditSHIV;
var const localized string m_strNoEditAwaySoldiers;
var const localized string m_strChooseEquipGoop;
var const localized string m_strLabelAbilities;
var const localized string m_strLabelAbilityHelp;
var const localized string m_strLabelPsiAbilities;
var const localized string m_strLabelPsiAbilityHelp;
var const localized string m_strLabelCustomize;
var const localized string m_strLabelCustomizeHelp;
var const localized string m_strLabelDismiss;
var const localized string m_strLabelDismissHelp;
var const localized string m_strLabelTankDismiss;
var const localized string m_strLabelTankDismissHelp;
var const localized string m_strLabelGeneView;
var const localized string m_strLabelGeneViewHelp;
var const localized string m_strLabelLocker;
var const localized string m_strLabelNoneAvailable;
var const localized string m_strLabelRequiresResearch;
var const localized string m_strLabelClassTypeOnly;
var const localized string m_strLabelItemUnavailableToClass;
var const localized string m_strLabelUnavailableToVolunteer;
var const localized string m_strLabelUniqueEquip;
var const localized string m_strLabelPrevSoldier;
var const localized string m_strLabelNextSoldier;
var const localized string m_strLabelReaperRoundsRestriction;
var const localized string m_strLabelGeneModHotLink;
var const localized string m_strLabelGeneModConfirm;
var const localized string m_strLabelMedals;
var const localized string m_strLabelMedalsHelp;
var const localized string m_strArmor;
var const localized string m_strPistol;
var const localized string m_strLargeItem;
var const localized string m_strSmallItem;
var const localized string m_strEmpty;
var const localized string m_strNoEditWoundedSoldiers;
var const localized string m_strNoDismissWoundedSoldiers;
var const localized string m_strNoDismissWhilePsiTesting;
var const localized string m_strNoDismissWhileAugmenting;
var const localized string m_strNoDismissWhileGeneModding;
var const localized string m_strNoDismissCovertOperative;
var const localized string m_strNoDismissVolunteer;
var private float m_fSoldierSwitchCount;