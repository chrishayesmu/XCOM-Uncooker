class XGInventoryNativeBase extends Actor
    native(Core)
    notplaceable
    hidecategories(Navigation);

const MAX_INVENTORY_ITEMS_PER_SLOT = 5;

enum ELocation
{
    eSlot_None,
    eSlot_RightBack,
    eSlot_LeftBack,
    eSlot_RightHand,
    eSlot_LeftHand,
    eSlot_Grapple,
    eSlot_RightThigh,
    eSlot_LeftThigh,
    eSlot_LeftBelt,
    eSlot_RightChest,
    eSlot_LeftChest,
    eSlot_RightForearm,
    eSlot_RightSling,
    eSlot_RearBackPack,
    eSlot_PsiSource,
    eSlot_Head,
    eSlot_CenterChest,
    eSlot_Claw_R,
    eSlot_Claw_L,
    eSlot_ChestCannon,
    eSlot_KineticStrike,
    eSlot_Flamethrower,
    eSlot_ElectroPulse,
    eSlot_GrenadeLauncher,
    eSlot_PMineLauncher,
    eSlot_RestorativeMist,
    eSlot_MAX
};

struct native InventorySlot
{
    var private ELocation m_eLoc;
    var private XGInventoryItem m_arrItems[5];
    var private int m_iNumItems;
    var bool m_bMultipleItems;
    var bool m_bReserved;
    var transient XComWeapon m_kLastUnequippedWeapon;
};

struct CheckpointRecord
{
    var XGWeapon m_kPrimaryWeapon;
    var XGWeapon m_kSecondaryWeapon;
    var InventorySlot m_arrStructSlots[ELocation];
    var ELocation m_ActiveWeaponLoc;
    var float m_fModifiableEngagementRange;
    var float m_fFixedEngagementRange;
    var float m_fGrenadeEngagementRange;
};

var name m_SocketNames[ELocation];
var private InventorySlot m_arrStructSlots[ELocation];
var ELocation m_ActiveWeaponLoc;
var float m_fModifiableEngagementRange;
var float m_fFixedEngagementRange;
var float m_fGrenadeEngagementRange;
var XGWeapon m_kPrimaryWeapon;
var XGWeapon m_kSecondaryWeapon;
var XGUnit m_kOwner;
var XGInventoryItem m_kLastEquippedItem;

defaultproperties
{
    m_SocketNames[1]=Inven_R_Back
    m_SocketNames[2]=Inven_L_Back
    m_SocketNames[3]=Inven_R_Hand
    m_SocketNames[4]=Inven_L_Hand
    m_SocketNames[5]=Inven_Grapple
    m_SocketNames[6]=Inven_R_Leg
    m_SocketNames[7]=Inven_L_Leg
    m_SocketNames[8]=Inven_L_Belt
    m_SocketNames[9]=Inven_R_Chest
    m_SocketNames[10]=Inven_L_Chest
    m_SocketNames[11]=Inven_R_Forearm
    m_SocketNames[12]=Inven_RightSling
    m_SocketNames[13]=Inven_RearBackPack
    m_SocketNames[14]=Inven_PsiSource
    m_SocketNames[15]=Inven_Head
    m_SocketNames[16]=Inven_C_Chest
    m_SocketNames[17]=Claw_R
    m_SocketNames[18]=Claw_L
    m_SocketNames[19]=HeatRay
    m_SocketNames[20]=KineticStrike
    m_SocketNames[21]=Flamethrower
    m_SocketNames[22]=ElectroPulse
    m_SocketNames[23]=GrenadeLauncher
    m_SocketNames[24]=PMineLauncher
    m_SocketNames[25]=RestorativeMist
    RemoteRole=ROLE_SimulatedProxy
    bAlwaysRelevant=true
}