/*******************************************************************************
 * UIGollop generated by Eliot.UELib using UE Explorer.
 * Eliot.UELib ? 2009-2015 Eliot van Uytfanghe. All rights reserved.
 * http://eliotvu.com
 *
 * All rights belong to their respective owners.
 *******************************************************************************/
class UIGollop extends UI_FxsScreen
    hidecategories(Navigation)
    implements(IScreenMgrInterface);

var private int m_iView;

defaultproperties
{
    s_package="/ package/gfxGollop/GOLLOP"
    s_screenId="gfxGollop"
    m_bAnimateOutro=false
    e_InputState=eInputState_Evaluate
    s_name=theGollopScreen
}