/*******************************************************************************
 * XGHangarShip generated by Eliot.UELib using UE Explorer.
 * Eliot.UELib ? 2009-2015 Eliot van Uytfanghe. All rights reserved.
 * http://eliotvu.com
 *
 * All rights belong to their respective owners.
 *******************************************************************************/
class XGHangarShip extends SkeletalMeshActor
    hidecategories(Navigation);

var export editinline StaticMeshComponent kRightWeaponMesh;
var export editinline StaticMeshComponent kLeftWeaponMesh;

defaultproperties
{
    Physics=PHYS_Interpolating
    bNoDelete=false

    begin object name=SkeletalMeshComponent0
        Animations=AnimNodeSeq0
        LightEnvironment=MyLightEnvironment
        LightingChannels=(Cinematic_6=true)
    end object
}