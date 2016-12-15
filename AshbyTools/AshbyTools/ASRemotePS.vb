Imports System.Security
Imports System.Management.Automation
Imports System.Management.Automation.Runspaces

Public Module ASRemotePowerShell
    Dim _pass As New SecureString()
    Dim _user As String
    Dim _server As String
    Dim RSpace As Runspace

    Public WriteOnly Property Password()
        Set(value)
            For Each c As Char In value
                _pass.AppendChar(c)
            Next
        End Set
    End Property

    Public Property User()
        Get
            Return _user
        End Get
        Set(value)
            _user = value
        End Set
    End Property

    Public Property Server() As String
        Get
            Return _server
        End Get
        Set(value As String)
            _server = value
        End Set
    End Property

    Public Sub connect()
        Dim credential As PSCredential = New PSCredential(User, _pass)
        Dim connectionInfo As New WSManConnectionInfo(New Uri(String.Format("http://{0}/powershell?serializationLevel=Full", Server)), "http://schemas.microsoft.com/powershell/Microsoft.Exchange", credential)

        connectionInfo.AuthenticationMechanism = AuthenticationMechanism.Kerberos

        Try
            RSpace = System.Management.Automation.Runspaces.RunspaceFactory.CreateRunspace(connectionInfo)
        Catch ex As Exception
            Throw New Exception("RSPACE: " & ex.Message)
        End Try
    End Sub

    Public Function runCommand(ByVal cmd As String, arg As String) As String
        Dim shell As PowerShell = PowerShell.Create()
        Dim pcommand As New PSCommand
        Dim ret As String = ""
        pcommand.AddCommand(cmd)
        'pcommand.AddArgument(arg)
        shell.Commands = pcommand
        Try
            RSpace.Open()
            shell.Runspace = RSpace
            Dim commandResults = shell.Invoke()

            For Each obj As PSObject In commandResults
                ret = ret & obj.ToString
            Next
            RSpace.Close()

        Catch ex As Exception
            Throw New Exception("Run command: " & ex.Message)
        Finally
            RSpace.Dispose()
            RSpace = Nothing
            shell.Dispose()
            shell = Nothing
        End Try
        Return ret


    End Function

End Module
