/*******************************************************************************
 * XGGollopUI generated by Eliot.UELib using UE Explorer.
 * Eliot.UELib ? 2009-2015 Eliot van Uytfanghe. All rights reserved.
 * http://eliotvu.com
 *
 * All rights belong to their respective owners.
 *******************************************************************************/
class XGGollopUI extends XGScreenMgr
    config(GameData)
    notplaceable
    hidecategories(Navigation);

enum EGollopView
{
    eGollopView_Main,
    eGollopView_Choose,
    eGollopView_MAX
};

struct TGollopUI
{
    var TButtonText btxtButton;
    var TText txtHelp;
};

struct TGollopList
{
    var TText txtTitle;
    var TTableMenu tmnuSoldiers;
    var array<XGStrategySoldier> arrVolunteers;
};

var TGollopUI m_kButton;
var TGollopList m_kList;
var private XComNarrativeMoment lMoment;
var private Vector kPlacement;
var const localized string m_strLabelUseProgenitorDevice;
var const localized string m_strPsiSoldierIsReady;
var const localized string m_strPsiSoldierIsWeak;
var const localized string m_strDeviceAlreadyUsed;
var const localized string m_strSelectSoldierForDevice;
var const localized string m_strGollopWarningTitle;
var const localized string m_strGollopWarningBody;
var const localized string m_strGollopWarningOK;
var const localized string m_strGollopWarningCancel;
var transient bool m_bAddedToMatinee;
var transient bool m_bCollideWorld;
var transient bool m_bCollideActors;
var transient bool m_bBlockActors;
var transient bool m_bIgnoreEncroachers;