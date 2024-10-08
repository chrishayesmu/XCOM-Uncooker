class XComSpecialMissionHandler extends Actor
    notplaceable
    hidecategories(Navigation);

struct CheckpointRecord
{
    var name SavedState;
    var int NumDeadXCom;
};

var bool bInitialized;
var bool bSetSavedState;
var name SavedState;
var XComTacticalController TacticalController;
var string ObjectiveIDs[5];
var string ObjectiveText[5];
var int NumDeadXCom;

defaultproperties
{
    ObjectiveIDs[0]="ObjID_1"
    ObjectiveIDs[1]="ObjID_2"
    ObjectiveIDs[2]="ObjID_3"
    ObjectiveIDs[3]="ObjID_4"
    ObjectiveIDs[4]="ObjID_5"
}