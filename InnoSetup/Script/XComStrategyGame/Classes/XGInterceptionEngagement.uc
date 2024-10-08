/*******************************************************************************
 * XGInterceptionEngagement generated by Eliot.UELib using UE Explorer.
 * Eliot.UELib ? 2009-2015 Eliot van Uytfanghe. All rights reserved.
 * http://eliotvu.com
 *
 * All rights belong to their respective owners.
 *******************************************************************************/
class XGInterceptionEngagement extends XGStrategyActor
    config(GameData)
    notplaceable
    hidecategories(Navigation);

const cfMovementPerSecond = -5.0;

struct CombatExchange
{
    var int iSourceShip;
    var int iWeapon;
    var int iTargetShip;
    var bool bHit;
    var int iDamage;
    var float fTime;
};

struct Combat
{
    var array<CombatExchange> m_aInterceptorExchanges;
    var array<CombatExchange> m_aUFOExchanges;
};

var XGInterception m_kInterception;
var Combat m_kCombat;
var array<int> m_aiShipHP;
var array<float> m_afShipDistance;
var float m_fTimeElapsed;
var int m_iPlaybackIndex;
var float m_fEncounterStartingRange;
var array<int> m_aiConsumableQuantitiesInEffect;
var float m_fInterceptorTimeOffset; // doesn't seem to ever be anything but 0 in LW 1.0
var int m_iUFOTarget;
var array<int> m_aiConsumablesUsed;