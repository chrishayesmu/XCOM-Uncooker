class UIWidget_Spinner extends UIWidget;

var string StrValue;
var bool bIsHorizontal;
var bool bCanSpin;
var array<string> arrLabels;
var array<int> arrValues;
var int iCurrentSelection;

delegate del_OnIncrease()
{
}

delegate del_OnDecrease()
{
}

defaultproperties
{
    bIsHorizontal=true
    bCanSpin=true
    eType=4
}