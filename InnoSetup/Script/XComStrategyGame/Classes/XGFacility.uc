/*******************************************************************************
 * XGFacility generated by Eliot.UELib using UE Explorer.
 * Eliot.UELib ? 2009-2015 Eliot van Uytfanghe. All rights reserved.
 * http://eliotvu.com
 *
 * All rights belong to their respective owners.
 *******************************************************************************/
class XGFacility extends XGStrategyActor
    abstract
    config(GameData)
    notplaceable
    hidecategories(Navigation);

struct CheckpointRecord
{
    var bool m_bFirstVisit;
    var bool m_bRequiresAttention;
    var bool m_bDisabled;

    structdefaultproperties
    {
        m_bFirstVisit=false
        m_bRequiresAttention=false
        m_bDisabled=false
    }
};

var bool m_bFirstVisit;
var bool m_bRequiresAttention;
var bool m_bDisabled;
var EFacilityType m_eFacility;
var EMusicCue m_eMusic;
var EAmbienceCue m_eAmbience;
var name m_nmRoomName;