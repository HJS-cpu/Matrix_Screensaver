; ============================================================================
; Matrix Screensaver - NSIS Installer Script
; ============================================================================
; Build: makensis /NOCD installer\MatrixScreensaver.nsi
; Requires: NSIS 3.x with MUI2, nsDialogs, x64, System plugins
; ============================================================================

!include "MUI2.nsh"
!include "x64.nsh"
!include "LogicLib.nsh"
!include "nsDialogs.nsh"

; --- Product Info ---
!define PRODUCT_NAME "Matrix Screensaver"
!define PRODUCT_VERSION "1.0"
!define PRODUCT_PUBLISHER "HJS"
!define PRODUCT_WEB_SITE "https://github.com/HJS-cpu/Matrix_Screensaver"
!define PRODUCT_UNINST_KEY "Software\Microsoft\Windows\CurrentVersion\Uninstall\MatrixScreensaver"
!define PRODUCT_SETTINGS_KEY "SOFTWARE\MatrixScreensaver"

; --- Download URLs ---
!define DOTNET_DOWNLOAD_URL "https://dotnet.microsoft.com/download/dotnet/8.0"
!define WEBVIEW2_DOWNLOAD_URL "https://developer.microsoft.com/en-us/microsoft-edge/webview2/"

; --- General ---
Name "${PRODUCT_NAME}"
OutFile "installer\MatrixScreensaverSetup.exe"
InstallDir "$PROGRAMDATA\Matrix"
RequestExecutionLevel admin
SetCompressor /SOLID lzma

; --- Variables ---
Var DotNetFound
Var WebView2Found
Var DotNetVersion
Var WebView2Version
Var Dialog
Var LabelDotNet
Var LabelWebView2
Var LinkDotNet
Var LinkWebView2
Var LabelSummary

; ============================================================================
; MUI Configuration
; ============================================================================
!define MUI_ABORTWARNING
!define MUI_ICON "${NSISDIR}\Contrib\Graphics\Icons\modern-install.ico"
!define MUI_UNICON "${NSISDIR}\Contrib\Graphics\Icons\modern-uninstall.ico"

; --- Installer Pages ---
!insertmacro MUI_PAGE_WELCOME
Page custom RuntimeCheckPageCreate RuntimeCheckPageLeave
!insertmacro MUI_PAGE_INSTFILES

; Finish page: checkbox to set Matrix as active screensaver
!define MUI_FINISHPAGE_SHOWREADME ""
!define MUI_FINISHPAGE_SHOWREADME_CHECKED
!define MUI_FINISHPAGE_SHOWREADME_TEXT "$(FinishSetScreensaver)"
!define MUI_FINISHPAGE_SHOWREADME_FUNCTION SetActiveScreensaver
!insertmacro MUI_PAGE_FINISH

; --- Uninstaller Pages ---
!insertmacro MUI_UNPAGE_CONFIRM
!insertmacro MUI_UNPAGE_INSTFILES

; --- Languages ---
!insertmacro MUI_LANGUAGE "English"
!insertmacro MUI_LANGUAGE "German"

; --- Language Strings ---
LangString FinishSetScreensaver ${LANG_ENGLISH} "Set Matrix as active screensaver"
LangString FinishSetScreensaver ${LANG_GERMAN} "Matrix als aktiven Screensaver setzen"

LangString RuntimePageTitle ${LANG_ENGLISH} "Runtime Check"
LangString RuntimePageTitle ${LANG_GERMAN} "Laufzeitumgebung"

LangString RuntimePageSubtitle ${LANG_ENGLISH} "Checking required runtime dependencies..."
LangString RuntimePageSubtitle ${LANG_GERMAN} "Erforderliche Laufzeitumgebungen werden gepr$\"uft..."

LangString DotNetOK ${LANG_ENGLISH} ".NET 8 Desktop Runtime:  Installed"
LangString DotNetOK ${LANG_GERMAN} ".NET 8 Desktop Runtime:  Installiert"

LangString DotNetMissing ${LANG_ENGLISH} ".NET 8 Desktop Runtime:  NOT FOUND"
LangString DotNetMissing ${LANG_GERMAN} ".NET 8 Desktop Runtime:  NICHT GEFUNDEN"

LangString WebView2OK ${LANG_ENGLISH} "WebView2 Runtime:  Installed"
LangString WebView2OK ${LANG_GERMAN} "WebView2 Runtime:  Installiert"

LangString WebView2Missing ${LANG_ENGLISH} "WebView2 Runtime:  NOT FOUND"
LangString WebView2Missing ${LANG_GERMAN} "WebView2 Runtime:  NICHT GEFUNDEN"

LangString DownloadDotNet ${LANG_ENGLISH} "Download .NET 8 Desktop Runtime"
LangString DownloadDotNet ${LANG_GERMAN} ".NET 8 Desktop Runtime herunterladen"

LangString DownloadWebView2 ${LANG_ENGLISH} "Download WebView2 Runtime"
LangString DownloadWebView2 ${LANG_GERMAN} "WebView2 Runtime herunterladen"

LangString RuntimeAllOK ${LANG_ENGLISH} "All required runtimes are installed. Click Next to continue."
LangString RuntimeAllOK ${LANG_GERMAN} "Alle erforderlichen Laufzeitumgebungen sind installiert. Klicken Sie auf Weiter."

LangString RuntimeWarning ${LANG_ENGLISH} "Warning: Missing runtimes must be installed for the screensaver to work. You can continue and install them later."
LangString RuntimeWarning ${LANG_GERMAN} "Warnung: Fehlende Laufzeitumgebungen m$\"ussen installiert werden. Sie k$\"onnen fortfahren und sie sp$\"ater installieren."

LangString PreviousInstall ${LANG_ENGLISH} "${PRODUCT_NAME} is already installed.$\nDo you want to uninstall the previous version first?"
LangString PreviousInstall ${LANG_GERMAN} "${PRODUCT_NAME} ist bereits installiert.$\nSoll die vorherige Version zuerst deinstalliert werden?"

; ============================================================================
; Runtime Detection Functions
; ============================================================================

Function CheckDotNet8
    StrCpy $DotNetFound 0
    StrCpy $DotNetVersion ""
    SetRegView 64

    ; Method 1: Registry enumeration
    StrCpy $1 0
    ${Do}
        EnumRegValue $2 HKLM "SOFTWARE\dotnet\Setup\InstalledVersions\x64\sharedfx\Microsoft.WindowsDesktop.App" $1
        ${If} $2 == ""
            ${ExitDo}
        ${EndIf}
        StrCpy $3 $2 2 ; first 2 characters
        ${If} $3 == "8."
            StrCpy $DotNetFound 1
            StrCpy $DotNetVersion $2
            Return
        ${EndIf}
        IntOp $1 $1 + 1
    ${Loop}

    ; Method 2: Fallback — check file system via native 64-bit Program Files
    ExpandEnvStrings $0 "%ProgramW6432%"
    IfFileExists "$0\dotnet\shared\Microsoft.WindowsDesktop.App\8.*" 0 done
        StrCpy $DotNetFound 1
        StrCpy $DotNetVersion "8.x"
    done:
FunctionEnd

Function CheckWebView2
    StrCpy $WebView2Found 0
    StrCpy $WebView2Version ""

    ; Try 64-bit registry
    SetRegView 64
    ReadRegStr $0 HKLM "SOFTWARE\Microsoft\EdgeUpdate\Clients\{F3017226-FE2A-4295-8BDF-00C3A9A7E4C5}" "pv"
    ${If} $0 != ""
    ${AndIf} $0 != "0.0.0.0"
        StrCpy $WebView2Found 1
        StrCpy $WebView2Version $0
        Return
    ${EndIf}

    ; Try 32-bit registry (WOW6432Node)
    SetRegView 32
    ReadRegStr $0 HKLM "SOFTWARE\Microsoft\EdgeUpdate\Clients\{F3017226-FE2A-4295-8BDF-00C3A9A7E4C5}" "pv"
    ${If} $0 != ""
    ${AndIf} $0 != "0.0.0.0"
        StrCpy $WebView2Found 1
        StrCpy $WebView2Version $0
    ${EndIf}

    SetRegView 64
FunctionEnd

; ============================================================================
; Runtime Check Custom Page
; ============================================================================

Function RuntimeCheckPageCreate
    Call CheckDotNet8
    Call CheckWebView2

    !insertmacro MUI_HEADER_TEXT "$(RuntimePageTitle)" "$(RuntimePageSubtitle)"

    nsDialogs::Create 1018
    Pop $Dialog
    ${If} $Dialog == error
        Abort
    ${EndIf}

    ; --- .NET 8 Status ---
    ${If} $DotNetFound == 1
        ${NSD_CreateLabel} 0 10u 100% 16u "$(DotNetOK) ($DotNetVersion)"
        Pop $LabelDotNet
        SetCtlColors $LabelDotNet 0x008000 transparent
    ${Else}
        ${NSD_CreateLabel} 0 10u 100% 16u "$(DotNetMissing)"
        Pop $LabelDotNet
        SetCtlColors $LabelDotNet 0xCC0000 transparent
        ${NSD_CreateLink} 16u 28u 90% 14u "$(DownloadDotNet)"
        Pop $LinkDotNet
        ${NSD_OnClick} $LinkDotNet OnDotNetLinkClick
    ${EndIf}

    ; --- WebView2 Status ---
    ${If} $WebView2Found == 1
        ${NSD_CreateLabel} 0 52u 100% 16u "$(WebView2OK) ($WebView2Version)"
        Pop $LabelWebView2
        SetCtlColors $LabelWebView2 0x008000 transparent
    ${Else}
        ${NSD_CreateLabel} 0 52u 100% 16u "$(WebView2Missing)"
        Pop $LabelWebView2
        SetCtlColors $LabelWebView2 0xCC0000 transparent
        ${NSD_CreateLink} 16u 70u 90% 14u "$(DownloadWebView2)"
        Pop $LinkWebView2
        ${NSD_OnClick} $LinkWebView2 OnWebView2LinkClick
    ${EndIf}

    ; --- Summary ---
    ${If} $DotNetFound == 1
    ${AndIf} $WebView2Found == 1
        ${NSD_CreateLabel} 0 100u 100% 28u "$(RuntimeAllOK)"
        Pop $LabelSummary
    ${Else}
        ${NSD_CreateLabel} 0 100u 100% 28u "$(RuntimeWarning)"
        Pop $LabelSummary
        SetCtlColors $LabelSummary 0xCC6600 transparent
    ${EndIf}

    nsDialogs::Show
FunctionEnd

Function RuntimeCheckPageLeave
    ; Always allow continuing — runtimes can be installed later
FunctionEnd

Function OnDotNetLinkClick
    ExecShell "open" "${DOTNET_DOWNLOAD_URL}"
FunctionEnd

Function OnWebView2LinkClick
    ExecShell "open" "${WEBVIEW2_DOWNLOAD_URL}"
FunctionEnd

; ============================================================================
; Set Active Screensaver (called from Finish page checkbox)
; ============================================================================

Function SetActiveScreensaver
    WriteRegStr HKCU "Control Panel\Desktop" "SCRNSAVE.EXE" "$SYSDIR\Matrix.scr"
FunctionEnd

; ============================================================================
; Installer Section
; ============================================================================

Section "Install" SecInstall
    ; --- Check for previous installation ---
    SetRegView 64
    ReadRegStr $0 HKLM "${PRODUCT_UNINST_KEY}" "UninstallString"
    ${If} $0 != ""
        MessageBox MB_YESNO|MB_ICONQUESTION "$(PreviousInstall)" IDNO skip_uninstall
            ExecWait '"$0" /S'
            Sleep 1000
        skip_uninstall:
    ${EndIf}

    ; --- Copy web assets to ProgramData\Matrix\ ---
    ; Remove old web assets first to avoid overwrite errors
    RMDir /r "$INSTDIR\web"
    SetOverwrite on
    SetOutPath "$INSTDIR"
    File /r "publish\web"

    ; --- Copy .scr and DLLs to System32 ---
    ; NSIS is 32-bit — disable FS redirection to write to real System32
    ${DisableX64FSRedirection}

    SetOutPath "$SYSDIR"
    File "publish\Matrix.scr"
    File "publish\Matrix.dll"
    File "publish\Matrix.deps.json"
    File "publish\Matrix.runtimeconfig.json"
    File "publish\Microsoft.Web.WebView2.Core.dll"
    File "publish\Microsoft.Web.WebView2.WinForms.dll"
    File "publish\Microsoft.Web.WebView2.Wpf.dll"
    File "publish\WebView2Loader.dll"

    ${EnableX64FSRedirection}

    ; --- Write uninstaller ---
    SetOutPath "$INSTDIR"
    WriteUninstaller "$INSTDIR\uninstall.exe"

    ; --- Registry: Add/Remove Programs ---
    SetRegView 64
    WriteRegStr HKLM "${PRODUCT_UNINST_KEY}" "DisplayName" "${PRODUCT_NAME}"
    WriteRegStr HKLM "${PRODUCT_UNINST_KEY}" "UninstallString" '"$INSTDIR\uninstall.exe"'
    WriteRegStr HKLM "${PRODUCT_UNINST_KEY}" "DisplayVersion" "${PRODUCT_VERSION}"
    WriteRegStr HKLM "${PRODUCT_UNINST_KEY}" "Publisher" "${PRODUCT_PUBLISHER}"
    WriteRegStr HKLM "${PRODUCT_UNINST_KEY}" "URLInfoAbout" "${PRODUCT_WEB_SITE}"
    WriteRegDWORD HKLM "${PRODUCT_UNINST_KEY}" "NoModify" 1
    WriteRegDWORD HKLM "${PRODUCT_UNINST_KEY}" "NoRepair" 1
SectionEnd

; ============================================================================
; Uninstaller Section
; ============================================================================

Section "Uninstall"
    ; --- Remove files from System32 ---
    ${DisableX64FSRedirection}

    Delete "$SYSDIR\Matrix.scr"
    Delete "$SYSDIR\Matrix.dll"
    Delete "$SYSDIR\Matrix.deps.json"
    Delete "$SYSDIR\Matrix.runtimeconfig.json"
    Delete "$SYSDIR\Microsoft.Web.WebView2.Core.dll"
    Delete "$SYSDIR\Microsoft.Web.WebView2.WinForms.dll"
    Delete "$SYSDIR\Microsoft.Web.WebView2.Wpf.dll"
    Delete "$SYSDIR\WebView2Loader.dll"

    ${EnableX64FSRedirection}

    ; --- Remove web assets and uninstaller from ProgramData ---
    RMDir /r "$INSTDIR\web"
    Delete "$INSTDIR\uninstall.exe"
    RMDir "$INSTDIR"

    ; --- Remove WebView2 cache ---
    RMDir /r "$TEMP\Matrix"

    ; --- Reset screensaver if Matrix was active ---
    ReadRegStr $0 HKCU "Control Panel\Desktop" "SCRNSAVE.EXE"
    ${If} $0 != ""
        ; Check if path ends with Matrix.scr
        StrCpy $1 $0 "" -10
        ${If} $1 == "Matrix.scr"
            DeleteRegValue HKCU "Control Panel\Desktop" "SCRNSAVE.EXE"
        ${EndIf}
    ${EndIf}

    ; --- Remove registry entries ---
    SetRegView 64
    DeleteRegKey HKLM "${PRODUCT_UNINST_KEY}"
    DeleteRegKey HKCU "${PRODUCT_SETTINGS_KEY}"
SectionEnd
