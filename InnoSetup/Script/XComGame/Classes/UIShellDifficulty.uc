class UIShellDifficulty extends UI_FxsScreen
    notplaceable
    hidecategories(Navigation);

var const localized string m_strSelectDifficulty;
var const localized string m_strChangeDifficulty;
var const localized array<localized string> m_arrDifficultyTypeStrings;
var const localized array<localized string> m_arrDifficultyDescStrings;
var const localized string m_strDifficultyEnableTutorial;
var const localized string m_strDifficultyEnableMeldTutorial;
var const localized string m_strDifficultyEnableProgeny;
var const localized string m_strDifficultyEnableSlingshot;
var const localized string m_strDifficultyEnableIronman;
var const localized string m_strDifficultySuppressFirstTimeNarrativeVO;
var const localized string m_strDifficultyTutorialDesc;
var const localized string m_strDifficultyMeldTutorialDesc;
var const localized string m_strDifficultyProgenyDesc;
var const localized string m_strDifficultySlingshotDesc;
var const localized string m_strDifficultyIronmanDesc;
var const localized string m_strDifficultySuppressFirstTimeNarrativeVODesc;
var const localized string m_strDifficulty_Back;
var const localized string m_strDifficulty_ToggleAdvanced;
var const localized string m_strDifficulty_ToggleAdvancedExit;
var const localized string m_strDifficulty_Accept;
var const localized string m_strDifficulty_SecondWaveButtonLabel;
var const localized string m_strControlTitle;
var const localized string m_strControlBody;
var const localized string m_strControlOK;
var const localized string m_strControlCancel;
var const localized string m_strFirstTimeTutorialTitle;
var const localized string m_strFirstTimeTutorialBody;
var const localized string m_strFirstTimeTutorialYes;
var const localized string m_strFirstTimeTutorialNo;
var const localized string m_strInvalidTutorialClassicDifficulty;
var const localized string m_strInvalidMeldTutorialBody;
var const localized string m_strIronmanTitle;
var const localized string m_strIronmanBody;
var const localized string m_strIronmanOK;
var const localized string m_strIronmanCancel;
var const localized string m_strIronmanLabel;
var const localized string m_strTutorialLabel;
var const localized string m_strTutorialOnImpossible;
var const localized string m_strTutorialNoChangeToImpossible;
var const localized string m_strWaitingForSaveTitle;
var const localized string m_strWaitingForSaveBody;
var UIWidgetHelper m_hDifficultyWidgetHelper;
var UIWidgetHelper m_hAdvancedWidgetHelper;
var UINavigationHelp m_kHelpBar;
var UINavigationHelp m_kHelpBar2;
var int m_iSelectedDifficulty;
var bool m_bControlledStart;
var bool m_bControlledStartMeld;
var bool m_bIronmanFromShell;
var bool m_bOperationProgeny;
var bool m_bOperationSlingshot;
var bool m_bFirstTimeNarrative;
var bool m_bShowedFirstTimeTutorialNotice;
var bool m_bCompletedControlledGame;
var bool m_bReceivedIronmanWarning;
var bool m_bSaveInProgress;
var bool m_bIsPlayingGame;
var bool m_bViewingAdvancedOptions;
var int m_iOptIronMan;
var int m_iOptTutorial;
var int m_iOptMeldTutorial;
var int m_iOptProgeny;
var int m_iOptSlingshot;
var int m_iOptFirstTimeNarrative;

defaultproperties
{
    s_package="/ package/gfxDifficultyMenu/DifficultyMenu"
    s_screenId="gfxDifficultyMenu"
    e_InputState=eInputState_Consume
    s_name="theDifficultyMenu"
}