/*******************************************************************************
 * UIPendingRequests generated by Eliot.UELib using UE Explorer.
 * Eliot.UELib ? 2009-2015 Eliot van Uytfanghe. All rights reserved.
 * http://eliotvu.com
 *
 * All rights belong to their respective owners.
 *******************************************************************************/
class UIPendingRequests extends UI_FxsScreen
    hidecategories(Navigation)
    implements(IScreenMgrInterface);

var const localized string m_strBack;
var const localized string m_strPendingRequests;
var const localized string m_strFunding;
var const localized string m_strMissions;
var const localized string m_strViewRequest;
var const localized string m_strHoursLeft;
var const localized string m_strSatelliteTransferLabel;
var const localized string m_strNoPendingRequests;
var const localized string m_strStatus_OperationInProgress;
var const localized string m_strStatus_TransferComplete;
var const localized string m_strStatus_AwaitingJetTransfer;
var const localized string m_strStatus_SatelliteEnRoute;
var const localized string m_strStatus_SatelliteCoverageComplete;
var const localized string m_strStatus_AwaitingSatelliteCoverage;
var const localized string m_strStatus_CanNotComplete;
var const localized string m_strStatus_CanComplete;
var int m_iView;
var array<int> m_arrSelectableIndexes;
var XGPendingRequestsUI m_kLocalMgr;

defaultproperties
{
    s_package="/ package/gfxPendingRequests/PendingRequests"
    s_screenId="gfxPendingRequests"
    e_InputState=eInputState_Evaluate
    s_name=thePendingRequests
}