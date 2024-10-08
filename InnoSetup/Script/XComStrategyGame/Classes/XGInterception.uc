/*******************************************************************************
 * XGInterception generated by Eliot.UELib using UE Explorer.
 * Eliot.UELib ? 2009-2015 Eliot van Uytfanghe. All rights reserved.
 * http://eliotvu.com
 *
 * All rights belong to their respective owners.
 *******************************************************************************/
class XGInterception extends XGStrategyActor
    config(GameData)
    notplaceable
    hidecategories(Navigation);

enum EUFOResult
{
    eUR_NONE,
    eUR_Crash,
    eUR_Destroyed,
    eUR_Escape,
    eUR_Disengaged,
    eUR_MAX
};

struct CheckpointRecord
{
    var array<XGShip_Interceptor> m_arrInterceptors;
    var XGShip_UFO m_kUFOTarget;
    var XGInterception.EUFOResult m_eUFOResult;
};

var array<XGShip_Interceptor> m_arrInterceptors;
var XGShip_UFO m_kUFOTarget;
var EUFOResult m_eUFOResult;
var bool m_bSimulatedCombat;