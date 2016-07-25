Imports System.Security.AccessControl

Public Class FileOperations

    Private Shared rights As FileSystemRights = FileSystemRights.AppendData Or FileSystemRights.CreateDirectories Or FileSystemRights.CreateFiles Or FileSystemRights.DeleteSubdirectoriesAndFiles Or FileSystemRights.ExecuteFile Or FileSystemRights.ListDirectory Or FileSystemRights.Read Or FileSystemRights.ReadAndExecute Or FileSystemRights.ReadAttributes Or FileSystemRights.ReadData Or FileSystemRights.ReadExtendedAttributes Or FileSystemRights.ReadPermissions Or FileSystemRights.Traverse Or FileSystemRights.WriteAttributes Or FileSystemRights.WriteData Or FileSystemRights.WriteExtendedAttributes

    Public Shared Sub createDirectory(ByVal path As String)
        My.Computer.FileSystem.CreateDirectory(path)
    End Sub

    Public Shared Function setACL(ByVal folderPath As String, userAccount As String, ByVal access As Boolean) As Boolean
        Try
            Dim FolderInfo As IO.DirectoryInfo = New IO.DirectoryInfo(folderPath & "\")

            Dim FolderAcl As New DirectorySecurity
            If access Then
                FolderAcl.AddAccessRule(New FileSystemAccessRule(userAccount, FileSystemRights.FullControl, InheritanceFlags.ContainerInherit Or InheritanceFlags.ObjectInherit, PropagationFlags.None, AccessControlType.Allow))
            Else
                FolderAcl.AddAccessRule(New FileSystemAccessRule(userAccount, rights, InheritanceFlags.ContainerInherit Or InheritanceFlags.ObjectInherit, PropagationFlags.None, AccessControlType.Allow))
            End If
            FolderAcl.SetAccessRuleProtection(False, True)
            FolderInfo.SetAccessControl(FolderAcl)
            FolderInfo.Refresh()
            Return True
        Catch ex As System.Security.Principal.IdentityNotMappedException
            Return False
        End Try
    End Function

    Public Shared Function exists(ByVal path) As Boolean
        If My.Computer.FileSystem.FileExists(path) Then
            Return True
        Else
            Return False
        End If
    End Function

End Class

