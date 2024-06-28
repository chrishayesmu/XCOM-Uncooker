/*******************************************************************************
 * XGEngineeringUI generated by Eliot.UELib using UE Explorer.
 * Eliot.UELib ? 2009-2015 Eliot van Uytfanghe. All rights reserved.
 * http://eliotvu.com
 *
 * All rights belong to their respective owners.
 *******************************************************************************/
class XGEngineeringUI extends XGScreenMgr
    config(GameData)
    notplaceable
    hidecategories(Navigation);

enum eEngView
{
    eEngView_MainMenu,
    eEngView_Build,
    eEngView_BaseBuilding,
    eEngView_EditQueue,
    eEngView_Foundry,
    eEngView_CyberneticsLab,
    eEngView_MecManufacturing,
    eEngView_GreyMarket,
    eEngView_MAX
};

struct TEngHeader
{
    var TText txtTitle;
    var TLabeledText txtCash;
    var TLabeledText txtElerium;
    var TLabeledText txtAlloys;
    var TLabeledText txtEngineers;
};

struct TEngMainMenu
{
    var TMenu mnuOptions;
};

struct TEngItemTable
{
    var TTableMenu mnuItems;
    var array<TObjectSummary> arrSummaries;
    var array<int> arrTabs;
    var array<TText> arrTabText;
    var int m_iCurrentTab;
};

struct TEngProject
{
    var TText txtTitle;
    var TImage imgStaff;
};

var TEngHeader m_kHeader;
var TEngMainMenu m_kMainMenu;
var TEngItemTable m_kTable;
var TTableMenu m_kQueue;
var TButtonText m_kQueueButton;
var TText m_kQueueTitle;
var array<int> m_arrMenuOptions;
var bool m_bManufacturing;
var bool m_bItemBuilt;
var int m_iCurrentSelection;
var const localized string m_strProjectsTitle;
var const localized string m_strProjectsTitleEmpty;
var const localized string m_strEtaLabel;
var const localized string m_strProjectNameLabel;
var const localized string m_strEngineerLabel;
var const localized string m_strQtyLabel;
var const localized string m_strHeaderEngineering;
var const localized string m_strHeaderBuildItems;
var const localized string m_strOptBuildBuy;
var const localized string m_strOptGreyMarket;
var const localized string m_strOptBuildFacilities;
var const localized string m_strOptFoundry;
var const localized string m_strOptCyberneticsLab;
var const localized string m_strOptManufactureMec;
var const localized string m_strHelpBuildBuy;
var const localized string m_strHelpGreyMarket;
var const localized string m_strHelpBuildFacilities;
var const localized string m_strHelpFoundry;
var const localized string m_strHelpCyberneticsLab;
var const localized string m_strHelpMecManufacture;
var const localized string m_strCatAll;
var const localized string m_strCatWeapons;
var const localized string m_strCatArmor;
var const localized string m_strCatVehicles;
var const localized string m_strCatAlien;
var const localized string m_strBuildCost;
var const localized string m_strEditQueue;
var const localized string m_strFoundryPrefix;
var const localized string m_strPriority;
var const localized string m_strShivFlyName;
var const localized string m_strShivFlyDesc;
var const localized string m_strShivCoverName;
var const localized string m_strShivCoverDesc;
var const localized string m_strShivSuppressName;
var const localized string m_strShivSuppressDesc;