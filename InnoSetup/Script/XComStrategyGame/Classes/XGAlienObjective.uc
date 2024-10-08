/*******************************************************************************
 * XGAlienObjective generated by Eliot.UELib using UE Explorer.
 * Eliot.UELib ? 2009-2015 Eliot van Uytfanghe. All rights reserved.
 * http://eliotvu.com
 *
 * All rights belong to their respective owners.
 *******************************************************************************/
class XGAlienObjective extends XGStrategyActor
    config(GameData)
    notplaceable
    hidecategories(Navigation);

struct CheckpointRecord
{
    var array<XGAlienObjective> m_arrSimultaneousObjs;
    var TObjective m_kTObjective;
    var int m_iCountryTarget;
    var int m_iCityTarget;
    var Vector2D m_v2Target;
    var bool m_bAbandoned;
    var int m_iTimer;
    var int m_iNextMissionTimer;
    var bool m_bComplete;
    var bool m_bLastMissionSuccessful;
    var bool m_bMissionThwarted;
    var int m_iSightings;
    var int m_iDetected;
    var int m_iShotDown;
    var XGShip_UFO m_kLastUFO;
    var bool m_bFoundSat;
    var bool m_bAbductionLaunched;
};

var array<XGAlienObjective> m_arrSimultaneousObjs;
var TObjective m_kTObjective;
var int m_iCountryTarget;
var int m_iCityTarget;
var Vector2D m_v2Target;
var bool m_bAbandoned;
var bool m_bComplete;
var bool m_bLastMissionSuccessful;
var bool m_bMissionThwarted;
var bool m_bFoundSat;
var bool m_bAbductionLaunched;
var int m_iTimer;
var int m_iNextMissionTimer;
var int m_iSightings;
var int m_iDetected;
var int m_iShotDown;
var XGShip_UFO m_kLastUFO;

defaultproperties
{
    m_iCountryTarget=-1
    m_iCityTarget=-1
}