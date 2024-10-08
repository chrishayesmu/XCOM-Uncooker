class XComTacticalCheatManager extends XComCheatManager within XComTacticalController
    native(Core);

var int m_iLastTeleportedAlien;
var bool bShowShotSummaryModifiers;
var bool bDebugDisableHitAdjustment;
var bool bDebugClimbOver;
var bool bDebugClimbOnto;
var bool bDebugCover;
var bool bShowActions;
var bool bAllUnits;
var bool bShowDestination;
var bool bInvincible;
var bool bUnlimitedAmmo;
var bool bMarker;
var bool bShowPaths;
var bool bAIStates;
var bool bPlayerStates;
var bool bShowNamesOnly;
var bool bShowDropship;
var bool bShowTracking;
var bool bShowVisibleEnemies;
var bool bMinimap;
var bool bShowCursorLoc;
var bool bShowCursorFloor;
var bool bShowSpawnPoints;
var bool bShowLoot;
var bool bTurning;
var bool bVerboseOnScreenText;
var bool bAITextSkipBase;
var bool bShowInteractMarkers;
var bool bShowFlankingMarkers;
var bool bForceSuppress;
var bool bForceOverwatch;
var bool bForceAIFire;
var bool bForceFlank;
var bool bForceEngage;
var bool bForceAttackCivilians;
var bool bForceMindMerge;
var bool bForceMindControl;
var bool bForceGrenade;
var bool bForceAscend;
var bool bForceLaunch;
var bool bForcePsiPanic;
var bool bForceHunkerDown;
var bool bShowAttackRange;
var bool bShowProjectilePath;
var bool bShowProjectiles;
var bool bDebugPods;
var bool bDebugGrenades;
var bool bDebugZombies;
var bool bDebugLoot;
var bool bDebugMindMerge;
var bool bShowBadCover;
var bool bDeadEye;
var bool bForceCriticalWound;
var bool bForceNoCriticalWound;
var bool bNoLuck;
var bool bShowAbilitySelection;
var bool bShowExposedCover;
var bool bShowAllBreadcrumbs;
var bool bShowTeamDestinations;
var bool bShowTerrorDestinations;
var bool bShowHiddenDestinations;
var bool bShowTeamDestinationScores;
var bool bShowTerrorDestinationScores;
var bool bShowCivTeamDestinations;
var bool bShowCivTeamDestinationScores;
var bool bAIUseReactionFireData;
var bool bShowTargetEnemy;
var bool bRevealAllCivilians;
var bool bBerserk;
var bool bShowOrientation;
var bool bRandomSpawns;
var bool bTestFlankingLocations;
var bool bDrawFracturedMeshEffectBoxes;
var bool bDebugInputState;
var bool bDebugPOI;
var bool bDebugAnims;
var bool bDisplayAnims;
var bool bDebugDead;
var bool bSkipReactionFire;
var bool bAllGlamAlways;
var bool bShowModifiers;
var bool bShowOverwatch;
var bool bVisualizeMove;
var bool bDebugReaction;
var bool bDebugCCState;
var bool bCloseCombatCheat;
var bool bCloseCombatDesiredResult;
var bool bShowAmmo;
var bool bThirdPersonAllTheTime;
var bool bForceOverheadView;
var bool bShowPathFailures;
var bool bDebugTargetting;
var bool bDebugFireActions;
var bool bDebugCoverActors;
var bool bDebugManeuvers;
var bool bDebugTimeDilation;
var bool bDebugTreads;
var bool bDebugWeaponSockets;
var bool bDebugOvermind;
var bool bTeamAttack;
var bool bAIGrenadeThrowingVis;
var bool bSkipNonMeleeAI;
var bool bDisableClosedMode;
var bool bDebugDestroyCover;
var bool bDebugPoison;
var bool bDisplayPathingFailures;
var bool bDebugBeginMoveHang;
var bool bDebugFlight;
var bool bDebugMouseTrace;
var bool bForceKillCivilians;
var bool bForceIntimidate;
var bool bShowUnitFlags;
var bool bDisableWorldMessages;
var bool bDebugLaunchCover;
var bool bDebugPathCover;
var bool m_bDebugPodValidation;
var bool m_bVisualPodValidationDebugging;
var bool bDisableTargettingOutline;
var bool bAlwaysRushCam;
var bool bDebugActiveAI;
var bool bShowShieldHP;
var bool bShowMaterials;
var bool bTestAttackOnDropDown;
var bool bDebugWaveSystem;
var bool bDebugWaveSystemContentRequests;
var bool bDebugBadAreaLog;
var bool bForcePodQuickMode;
var bool bUseStrangleStopA;
var bool bDebugCamera;
var int iShowPath;
var Vector vDebugLoc;
var int iTestHideableActors;
var name m_DebugAnims_UnitName;
var name m_ToggleUnitVis_UnitName;
var XComRandomList kRandList;
var Vector vReferencePoint;
var array<int> arrAbilityForceEnable;
var array<int> arrAbilityForceDisable;
var int iOverrideAnim;
var Vector vLookAt;
var XComCoverPoint kDebugCover;
var int iRightSidePos;
var name XCom_Anim_DebugUnitName;
var float fStrangleDist;
var array<int> arrSkipAllExceptions;

defaultproperties
{
    bShowUnitFlags=true
    iTestHideableActors=1
    iOverrideAnim=75
    iRightSidePos=1600
    fStrangleDist=64.0
}