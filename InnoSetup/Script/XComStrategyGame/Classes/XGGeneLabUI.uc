/*******************************************************************************
 * XGGeneLabUI generated by Eliot.UELib using UE Explorer.
 * Eliot.UELib ? 2009-2015 Eliot van Uytfanghe. All rights reserved.
 * http://eliotvu.com
 *
 * All rights belong to their respective owners.
 *******************************************************************************/
class XGGeneLabUI extends XGScreenMgr
    config(GameData)
    notplaceable
    hidecategories(Navigation);

enum EGeneLabView
{
    eGeneLabView_MainMenu,
    eGeneLabView_Add,
    eGeneLabView_Results,
    eGeneLabView_MAX
};

struct TSoldierTable
{
    var TTableMenu mnuSoldiers;
};

struct TGeneSubjectList
{
    var TText txtTitle;
    var TTableMenu mnuSlots;
    var TButtonText btxtChoose;
};

var TGeneSubjectList m_kSubjectList;
var TSoldierTable m_kSoldierTable;
var int m_iHighlightedSlot;
var XGStrategySoldier m_kShuttleSoldier;
var int m_iShuttleSelectedRow;
var int m_iShuttleSelectedCol;
var const localized string m_strLabelCurrentPatients;
var const localized string m_strLabelAddSoldier;
var const localized string m_strLabelHours;
var const localized string m_strLabelDays;
var const localized string m_strLabelEmpty;
var const localized string m_strPsiGiftedStatus;