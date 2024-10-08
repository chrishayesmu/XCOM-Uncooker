/*******************************************************************************
 * XGFacility_MissionControl generated by Eliot.UELib using UE Explorer.
 * Eliot.UELib ? 2009-2015 Eliot van Uytfanghe. All rights reserved.
 * http://eliotvu.com
 *
 * All rights belong to their respective owners.
 *******************************************************************************/
class XGFacility_MissionControl extends XGFacility
    config(GameData)
    notplaceable
    hidecategories(Navigation);

struct CheckpointRecord_XGFacility_MissionControl extends CheckpointRecord
{
    var bool m_bDetectedOverseer;
    var int m_iNoJetsCounter;
};

var bool m_bDetectedOverseer;
var int m_iNoJetsCounter;