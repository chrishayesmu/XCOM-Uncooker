/*******************************************************************************
 * WatchVariableMgr generated by Eliot.UELib using UE Explorer.
 * Eliot.UELib ? 2009-2015 Eliot van Uytfanghe. All rights reserved.
 * http://eliotvu.com
 *
 * All rights belong to their respective owners.
 *******************************************************************************/
class WatchVariableMgr extends Actor
    native
    notplaceable
    hidecategories(Navigation);

var private native const noexport Pointer VfTable_FTickableObject;
var array<WatchVariable> WatchVariables;
var bool bInWatchVariableUpdate;
var init array<init WatchVariable> WatchVariablesToRemove;

defaultproperties
{
    TickGroup=ETickingGroup.TG_PostAsyncWork
}