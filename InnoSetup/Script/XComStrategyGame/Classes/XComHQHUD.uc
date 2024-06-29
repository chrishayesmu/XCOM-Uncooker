/*******************************************************************************
 * XComHQHUD generated by Eliot.UELib using UE Explorer.
 * Eliot.UELib ? 2009-2015 Eliot van Uytfanghe. All rights reserved.
 * http://eliotvu.com
 *
 * All rights belong to their respective owners.
 *******************************************************************************/
class XComHQHUD extends XComHUD
    transient
    config(Game)
    hidecategories(Navigation);

const Y_OFFSET = 20;

struct ChooseFacilityMIC
{
    var EFacilityType Facility;
    var string FacilityMICName;
    var MaterialInstanceConstant FacilityMIC;
};

var privatewrite array<ChooseFacilityMIC> ChooseFacilityMICs;
var privatewrite MaterialInstanceConstant FacilityMIC;
var privatewrite float FacilityTextureScale;
var float FacilityTextureNormSizeX;
var float FacilityTextureNormSizeY;
var Material FacilityBackgroundMaterial;
var float FacilityBackgroundTextureNormSizeX;
var float FacilityBackgroundTextureNormSizeY;

defaultproperties
{
    FacilityTextureScale=1.40
    FacilityTextureNormSizeX=0.70
    FacilityTextureNormSizeY=0.5750
    FacilityBackgroundTextureNormSizeX=0.80
    FacilityBackgroundTextureNormSizeY=0.550
}