/*******************************************************************************
 * UIMissionControl_MissionList generated by Eliot.UELib using UE Explorer.
 * Eliot.UELib ? 2009-2015 Eliot van Uytfanghe. All rights reserved.
 * http://eliotvu.com
 *
 * All rights belong to their respective owners.
 *******************************************************************************/
class UIMissionControl_MissionList extends UI_FxsPanel
    dependson(XGMissionControlUI)
    hidecategories(Navigation);

var const localized string m_strMissionListTitle;
var XGMissionControlUI m_kLocalMgr;
var int m_hMissionWatchHandle;
var int m_hHighlightWatchHandle;
var int m_hTimeScaleHandle;
var array<TMCMission> arrCurrDisplayedMissions;

defaultproperties
{
    s_name=theMissionList
}