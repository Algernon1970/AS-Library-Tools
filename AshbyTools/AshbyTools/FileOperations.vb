Imports System.Security.AccessControl
Imports System.IO
Imports System.Security
Imports Microsoft.SharePoint.Client
Imports System.ComponentModel

Public Class FileOperations
    Public Shared lastError As String = "OK"
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

    Public Shared Function copyToSP(origFile As String, library As String, share As String, user As String, passwd As SecureString) As String
        Try
            Dim spcontext As New Microsoft.SharePoint.Client.ClientContext(library)
            Dim creds As New Microsoft.SharePoint.Client.SharePointOnlineCredentials(user, passwd)
            spcontext.Credentials = creds

            Dim spLib = spcontext.Web.Lists.GetByTitle(share)
            spcontext.Load(spLib)
            spcontext.ExecuteQuery()

            Dim fs As New FileStream(origFile, FileMode.Open)
            Dim fci As New Microsoft.SharePoint.Client.FileCreationInformation
            fci.Overwrite = True
            fci.ContentStream = fs
            fci.Url = Path.GetFileName(origFile)

            Dim upload = spLib.RootFolder.Files.Add(fci)
            spcontext.Load(upload)
            spcontext.Load(spLib.ContentTypes)
            spcontext.ExecuteQuery()
        Catch ex As Exception
            Return ex.Message
        End Try
        Return "OK"
    End Function

    Public Shared Function deleteFromSP(fname As String, library As String, Share As String, user As String, passwd As SecureString) As String
        Try
            Dim spcontext As New Microsoft.SharePoint.Client.ClientContext(library)
            Dim creds As New Microsoft.SharePoint.Client.SharePointOnlineCredentials(user, passwd)
            spcontext.Credentials = creds

            Dim spLib = spcontext.Web.Lists.GetByTitle(Share)
            Dim query As New Microsoft.SharePoint.Client.CamlQuery()
            query.ViewXml = String.Format("<View><Query><Where><Eq><FieldRef Name='FileLeafRef'/><Value Type='File'>{0}</Value></Eq></Where></Query></View>", fname)
            Dim listItems = spLib.GetItems(query)
            spcontext.Load(listItems)
            spcontext.ExecuteQuery()
            For Each listItem In listItems
                listItem.DeleteObject()
                spcontext.ExecuteQuery()
            Next

        Catch ex As Exception
            Return ex.Message
        End Try
        Return "OK"
    End Function

    Public Shared Function loadFromSP(fname As String, library As String, Share As String, user As String, passwd As SecureString) As Stream
        Dim fs As ClientResult(Of Stream)
        Try
            Dim spcontext As New Microsoft.SharePoint.Client.ClientContext(library)
            Dim creds As New Microsoft.SharePoint.Client.SharePointOnlineCredentials(user, passwd)
            spcontext.Credentials = creds

            Dim spLib = spcontext.Web.Lists.GetByTitle(Share)
            Dim query As New Microsoft.SharePoint.Client.CamlQuery()
            query.ViewXml = String.Format("<View><Query><Where><Eq><FieldRef Name='FileLeafRef'/><Value Type='File'>{0}</Value></Eq></Where></Query></View>", fname)
            Dim listItems = spLib.GetItems(query)
            spcontext.Load(listItems)
            spcontext.ExecuteQuery()
            For Each listItem In listItems
                spcontext.Load(listItem)
                spcontext.ExecuteQuery()
                spcontext.Load(listItem.File)
                spcontext.ExecuteQuery()
                fs = listItem.File.OpenBinaryStream()
                spcontext.ExecuteQuery()
                If Not IsNothing(fs) Then
                    Return fs.Value
                End If
            Next

        Catch ex As Exception
            lastError = ex.Message
            Return Nothing
        End Try
        lastError = "Rats"
        Return Nothing
    End Function

    Public Shared Function getAvailableDriveLetters() As List(Of String)
        Dim allPossibleDriveLetters() As String = {"A:\", "B:\", "C:\", "D:\", "E:\", "F:\", "G:\", "H:\", "I:\", "J:\", "K:\", "L:\", "M:\", "N:\", "O:\", "P:\", "Q:\", "R:\", "S:\", "T:\", "U:\", "V:\", "W:\", "X:\", "Y:\", "Z:\"}
        Dim allDrives() As DriveInfo = DriveInfo.GetDrives
        Dim activeDriveLetters As New List(Of String)
        Dim availableDriveLetters As New List(Of String)

        For Each drive As DriveInfo In allDrives
            activeDriveLetters.Add(drive.Name)
        Next

        For Each letter In allPossibleDriveLetters
            If Not activeDriveLetters.Contains(letter) Then
                availableDriveLetters.Add(letter)
            End If
        Next

        Return availableDriveLetters
    End Function


End Class

