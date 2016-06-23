
Imports System.Runtime.InteropServices

Namespace murrayju.ProcessExtensions
    Public NotInheritable Class ProcessExtensions
        Private Sub New()
        End Sub
#Region "Win32 Constants"

        Private Const CREATE_UNICODE_ENVIRONMENT As Integer = &H400
        Private Const CREATE_NO_WINDOW As Integer = &H8000000

        Private Const CREATE_NEW_CONSOLE As Integer = &H10

        Private Const INVALID_SESSION_ID As UInteger = &HFFFFFFFFUI
        Private Shared ReadOnly WTS_CURRENT_SERVER_HANDLE As IntPtr = IntPtr.Zero

#End Region

#Region "DllImports"

        <DllImport("advapi32.dll", EntryPoint:="CreateProcessAsUser", SetLastError:=True, CharSet:=CharSet.Ansi, CallingConvention:=CallingConvention.StdCall)>
        Private Shared Function CreateProcessAsUser(hToken As IntPtr, lpApplicationName As [String], lpCommandLine As [String], lpProcessAttributes As IntPtr, lpThreadAttributes As IntPtr, bInheritHandle As Boolean,
            dwCreationFlags As UInteger, lpEnvironment As IntPtr, lpCurrentDirectory As [String], ByRef lpStartupInfo As STARTUPINFO, ByRef lpProcessInformation As PROCESS_INFORMATION) As Boolean
        End Function

        <DllImport("advapi32.dll", EntryPoint:="DuplicateTokenEx")>
        Private Shared Function DuplicateTokenEx(ExistingTokenHandle As IntPtr, dwDesiredAccess As UInteger, lpThreadAttributes As IntPtr, TokenType As Integer, ImpersonationLevel As Integer, ByRef DuplicateTokenHandle As IntPtr) As Boolean
        End Function

        <DllImport("userenv.dll", SetLastError:=True)>
        Private Shared Function CreateEnvironmentBlock(ByRef lpEnvironment As IntPtr, hToken As IntPtr, bInherit As Boolean) As Boolean
        End Function

        <DllImport("userenv.dll", SetLastError:=True)>
        Private Shared Function DestroyEnvironmentBlock(lpEnvironment As IntPtr) As <MarshalAs(UnmanagedType.Bool)> Boolean
        End Function

        <DllImport("kernel32.dll", SetLastError:=True)>
        Private Shared Function CloseHandle(hSnapshot As IntPtr) As Boolean
        End Function

        <DllImport("kernel32.dll")>
        Private Shared Function WTSGetActiveConsoleSessionId() As UInteger
        End Function

        <DllImport("Wtsapi32.dll")>
        Private Shared Function WTSQueryUserToken(SessionId As UInteger, ByRef phToken As IntPtr) As UInteger
        End Function

        <DllImport("wtsapi32.dll", SetLastError:=True)>
        Private Shared Function WTSEnumerateSessions(hServer As IntPtr, Reserved As Integer, Version As Integer, ByRef ppSessionInfo As IntPtr, ByRef pCount As Integer) As Integer
        End Function

#End Region

#Region "Win32 Structs"

        Private Enum SW
            SW_HIDE = 0
            SW_SHOWNORMAL = 1
            SW_NORMAL = 1
            SW_SHOWMINIMIZED = 2
            SW_SHOWMAXIMIZED = 3
            SW_MAXIMIZE = 3
            SW_SHOWNOACTIVATE = 4
            SW_SHOW = 5
            SW_MINIMIZE = 6
            SW_SHOWMINNOACTIVE = 7
            SW_SHOWNA = 8
            SW_RESTORE = 9
            SW_SHOWDEFAULT = 10
            SW_MAX = 10
        End Enum

        Private Enum WTS_CONNECTSTATE_CLASS
            WTSActive
            WTSConnected
            WTSConnectQuery
            WTSShadow
            WTSDisconnected
            WTSIdle
            WTSListen
            WTSReset
            WTSDown
            WTSInit
        End Enum

        <StructLayout(LayoutKind.Sequential)>
        Private Structure PROCESS_INFORMATION
            Public hProcess As IntPtr
            Public hThread As IntPtr
            Public dwProcessId As UInteger
            Public dwThreadId As UInteger
        End Structure

        Private Enum SECURITY_IMPERSONATION_LEVEL
            SecurityAnonymous = 0
            SecurityIdentification = 1
            SecurityImpersonation = 2
            SecurityDelegation = 3
        End Enum

        <StructLayout(LayoutKind.Sequential)>
        Private Structure STARTUPINFO
            Public cb As Integer
            Public lpReserved As [String]
            Public lpDesktop As [String]
            Public lpTitle As [String]
            Public dwX As UInteger
            Public dwY As UInteger
            Public dwXSize As UInteger
            Public dwYSize As UInteger
            Public dwXCountChars As UInteger
            Public dwYCountChars As UInteger
            Public dwFillAttribute As UInteger
            Public dwFlags As UInteger
            Public wShowWindow As Short
            Public cbReserved2 As Short
            Public lpReserved2 As IntPtr
            Public hStdInput As IntPtr
            Public hStdOutput As IntPtr
            Public hStdError As IntPtr
        End Structure

        Private Enum TOKEN_TYPE
            TokenPrimary = 1
            TokenImpersonation = 2
        End Enum

        <StructLayout(LayoutKind.Sequential)>
        Private Structure WTS_SESSION_INFO
            Public ReadOnly SessionID As UInt32

            <MarshalAs(UnmanagedType.LPStr)>
            Public ReadOnly pWinStationName As [String]

            Public ReadOnly State As WTS_CONNECTSTATE_CLASS
        End Structure

#End Region

        ' Gets the user token from the currently active session
        Private Shared Function GetSessionUserToken(ByRef phUserToken As IntPtr) As Boolean
            Dim bResult = False
            Dim hImpersonationToken = IntPtr.Zero
            Dim activeSessionId = INVALID_SESSION_ID
            Dim pSessionInfo = IntPtr.Zero
            Dim sessionCount = 0

            ' Get a handle to the user access token for the current active session.
            If WTSEnumerateSessions(WTS_CURRENT_SERVER_HANDLE, 0, 1, pSessionInfo, sessionCount) <> 0 Then
                Dim arrayElementSize = Marshal.SizeOf(GetType(WTS_SESSION_INFO))
                Dim current = pSessionInfo

                For i As Integer = 0 To sessionCount - 1
                    Dim si = CType(Marshal.PtrToStructure(current, GetType(WTS_SESSION_INFO)), WTS_SESSION_INFO)
                    current += arrayElementSize

                    If si.State = WTS_CONNECTSTATE_CLASS.WTSActive Then
                        activeSessionId = si.SessionID
                    End If
                Next
            End If

            ' If enumerating did not work, fall back to the old method
            If activeSessionId = INVALID_SESSION_ID Then
                activeSessionId = WTSGetActiveConsoleSessionId()
            End If

            If WTSQueryUserToken(activeSessionId, hImpersonationToken) <> 0 Then
                ' Convert the impersonation token to a primary token
                bResult = DuplicateTokenEx(hImpersonationToken, 0, IntPtr.Zero, CInt(SECURITY_IMPERSONATION_LEVEL.SecurityImpersonation), CInt(TOKEN_TYPE.TokenPrimary), phUserToken)

                CloseHandle(hImpersonationToken)
            End If

            Return bResult
        End Function

        Public Shared Function StartProcessAsCurrentUser(appPath As String, Optional cmdLine As String = Nothing, Optional workDir As String = Nothing, Optional visible As Boolean = True) As Boolean
            Dim hUserToken = IntPtr.Zero
            Dim startInfo = New STARTUPINFO()
            Dim procInfo = New PROCESS_INFORMATION()
            Dim pEnv = IntPtr.Zero
            Dim iResultOfCreateProcessAsUser As Integer

            startInfo.cb = Marshal.SizeOf(GetType(STARTUPINFO))

            Try
                If Not GetSessionUserToken(hUserToken) Then
                    Throw New Exception("StartProcessAsCurrentUser: GetSessionUserToken failed.")
                End If

                Dim dwCreationFlags As UInteger = CREATE_UNICODE_ENVIRONMENT Or CUInt(If(visible, CREATE_NEW_CONSOLE, CREATE_NO_WINDOW))
                startInfo.wShowWindow = CShort(If(visible, SW.SW_SHOW, SW.SW_HIDE))
                startInfo.lpDesktop = "winsta0\default"

                If Not CreateEnvironmentBlock(pEnv, hUserToken, False) Then
                    Throw New Exception("StartProcessAsCurrentUser: CreateEnvironmentBlock failed.")
                End If

                ' Application Name
                ' Command Line
                ' Working directory
                If Not CreateProcessAsUser(hUserToken, appPath, cmdLine, IntPtr.Zero, IntPtr.Zero, False,
                    dwCreationFlags, pEnv, workDir, startInfo, procInfo) Then
                    Throw New Exception("StartProcessAsCurrentUser: CreateProcessAsUser failed." & vbLf)
                End If

                iResultOfCreateProcessAsUser = Marshal.GetLastWin32Error()
            Finally
                CloseHandle(hUserToken)
                If pEnv <> IntPtr.Zero Then
                    DestroyEnvironmentBlock(pEnv)
                End If
                CloseHandle(procInfo.hThread)
                CloseHandle(procInfo.hProcess)
            End Try

            Return True
        End Function

    End Class
End Namespace

'=======================================================
'Service provided by Telerik (www.telerik.com)
'Conversion powered by NRefactory.
'Twitter: @telerik
'Facebook: facebook.com/telerik
'=======================================================
