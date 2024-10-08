/*******************************************************************************
 * XGWorld generated by Eliot.UELib using UE Explorer.
 * Eliot.UELib ? 2009-2015 Eliot van Uytfanghe. All rights reserved.
 * http://eliotvu.com
 *
 * All rights belong to their respective owners.
 *******************************************************************************/
class XGWorld extends XGStrategyActor
    config(GameData)
    notplaceable
    hidecategories(Navigation);

struct CheckpointRecord
{
    var array<XGCountry> m_arrCountries;
    var array<XGCity> m_arrCities;
    var array<XGContinent> m_arrContinents;
    var array<TSatNode> m_arrSatNodes;
    var XGFundingCouncil m_kFundingCouncil;
    var TCouncilMeeting m_kCouncil;
    var int m_iNumCountriesLost;
};

var array<XGCountry> m_arrCountries;
var array<XGCity> m_arrCities;
var array<XGContinent> m_arrContinents;
var array<TSatNode> m_arrSatNodes;
var XGFundingCouncil m_kFundingCouncil;
var TCouncilMeeting m_kCouncil;
var int m_iNumCountriesLost;
var const localized string m_aContinentNames[5];