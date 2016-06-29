Imports System.DirectoryServices.AccountManagement
Imports System.Windows.Forms


Public Module ADTools
    Dim msg As New eMailMessage
    Dim userCTX As PrincipalContext = getConnection("as.internal", usersCTXString)
    Dim tutorsCTX As PrincipalContext = getConnection("as.internal", tutorsCTXString)
    Dim yearCTX As PrincipalContext = getConnection("as.internal", yearCTXString)
    Dim classCTX As PrincipalContext = getConnection("as.internal", classCTXString)
    Dim myUTree As UserTree
    Dim rflag As Boolean = False

    Public Sub setupMail(ByVal eMailFrom As String, ByVal Optional eMailto As String = "itsupport@ashbyschool.org.uk",
                         Optional ByVal emailSubject As String = "Automailed Message", Optional ByVal body As String = "Blank",
                         Optional ByVal ishtml As Boolean = False)
        msg.mailfrom = eMailFrom
        msg.mailto = eMailto
        msg.subject = emailSubject
        msg.isBodyHTML = ishtml
        msg.body = body
    End Sub

    Public Function getConnection(ByVal domain As String, ByVal OU As String) As PrincipalContext
        Return New PrincipalContext(ContextType.Domain, domain, OU, adUser, adPass)
    End Function

    Public Function userExists(ByVal ctx As PrincipalContext, ByVal user As UserDetails) As Boolean
        Dim usr As UserPrincipal = UserPrincipal.FindByIdentity(ctx, user.Username)
        If usr IsNot Nothing Then
            usr.Dispose()
            Return True
        Else
            Return False
        End If
    End Function

    Public Sub addToGroups(ByVal ctx As PrincipalContext, ByVal user As UserDetails)
        ' add User to group
        If user.Groups.Count > 0 Then
            For Each group As String In user.Groups
                addUserToGroup(ctx, user.Username, group)
            Next
        End If
    End Sub

    Public Function getUserPrincipalByID(ByVal ctx As PrincipalContext, ByVal admission As String) As DirectoryServices.AccountManagement.UserPrincipal
        Dim userList As New List(Of DirectoryServices.AccountManagement.UserPrincipal)
        Dim searchusr As DirectoryServices.AccountManagement.UserPrincipal = New DirectoryServices.AccountManagement.UserPrincipal(ctx)
        searchusr.EmployeeId = admission
        Dim searcher As PrincipalSearcher = New PrincipalSearcher(searchusr)
        For Each user As DirectoryServices.AccountManagement.UserPrincipal In searcher.FindAll()
            userList.Add(user)
        Next
        searcher.Dispose()
        If userList.Count > 1 Then
            Dim str As String = "Too many users with same id number - " & admission & vbCrLf
            For Each user In userList
                str = String.Format("{0}{1}{2}", str, vbCrLf, user.SamAccountName)
            Next
            Dim msg As New eMailMessage
            msg.body = str
            Sendmail.sendmail(msg)
            Return Nothing
        End If
        If userList.Count = 0 Then
            Return Nothing
        End If
        Return userList(0)
    End Function

    Public Function getGroupPrincipalbyName(ByVal ctx As PrincipalContext, group As String) As GroupPrincipal
        Try
            Dim usr As GroupPrincipal = GroupPrincipal.FindByIdentity(ctx, group)
            Return usr
        Catch ex As Exception
            Throw
        End Try
    End Function

    Public Sub addUserToGroup(ByVal ctx As PrincipalContext, user As String, ByVal group As String)
        Try
            Dim usr As DirectoryServices.AccountManagement.UserPrincipal = getUserPrincipalbyUsername(ctx, user)
            Dim grp As GroupPrincipal = getGroupPrincipalbyName(ctx, group)
            If Not grp.Members.Contains(ctx, IdentityType.SamAccountName, usr.SamAccountName) Then
                grp.Members.Add(usr)
            End If
            grp.Save()
        Catch ex As Exception

            'no groups to add
        End Try
    End Sub

    Public Function createUser(ByVal ctx As PrincipalContext, ByVal user As UserDetails) As createStatus
        'User already exist?
        If Not userExists(ctx, user) Then
            Dim lusr = New UserPrincipal(ctx)
            lusr.SamAccountName = user.Username
            lusr.DisplayName = user.DisplayName
            lusr.GivenName = user.GivenName
            lusr.Surname = user.Surname
            Try
                lusr.Save(ctx)
                lusr.Dispose()
                modify(ctx, user)
                Return createStatus.ok
            Catch ex As Exception
                Return createStatus.noAD
            End Try
            Return createStatus.fubar
        Else
            Return createStatus.alreadyExists
        End If

    End Function

    Private Sub modify(ByVal ctx As PrincipalContext, ByVal user As UserDetails)
        Dim usr As UserPrincipalex = UserPrincipalex.FindByIdentity(ctx, user.Username)
        If usr Is Nothing Then
            Throw New Exception("User Doesn't Exist")
        End If
        If user.Enabled.ToLower.Equals("true") Then
            usr.Enabled = True
        Else
            usr.Enabled = False
        End If
        If user.PasswordNeverExpires.ToLower.Equals("true") Then
            usr.PasswordNeverExpires = True
        Else
            usr.PasswordNeverExpires = False
        End If
        If user.UserCannotChangePassword.ToLower.Equals("true") Then
            usr.UserCannotChangePassword = True
        Else
            usr.UserCannotChangePassword = False
        End If

        usr.ProfilePath = "\\" & user.ProfilePath
        usr.HomeDrive = user.HomeDrive
        usr.HomeDirectory = "\\" & user.HomeDirectory
        usr.SetPassword(user.Password)
        usr.Description = user.Description
        usr.MiddleName = user.MiddleName
        usr.EmployeeId = user.BromcomID

        usr.UserPrincipalName = user.Username & emailDomain
        usr.Save()
        usr.Dispose()
    End Sub

    Public Function getUserPrincipalexbyUsername(ByVal ctx As PrincipalContext, userName As String) As UserPrincipal
        Try
            Dim usr As UserPrincipal = UserPrincipal.FindByIdentity(ctx, userName)
            Return usr
        Catch ex As Exception
            Return Nothing
        End Try
    End Function

    Public Function getUserPrincipalbyUsername(ByVal ctx As PrincipalContext, userName As String) As UserPrincipal
        Try
            Dim usr As DirectoryServices.AccountManagement.UserPrincipal = DirectoryServices.AccountManagement.UserPrincipal.FindByIdentity(ctx, userName)
            Return usr
        Catch ex As Exception
            Return Nothing
        End Try
    End Function

    Public Function getUserPrincipalsByFullName(ByVal ctx As PrincipalContext, fullname As String) As List(Of DirectoryServices.AccountManagement.UserPrincipal)
        Dim userlist As New List(Of DirectoryServices.AccountManagement.UserPrincipal)
        Dim searchusr As DirectoryServices.AccountManagement.UserPrincipal = New DirectoryServices.AccountManagement.UserPrincipal(ctx)
        searchusr.GivenName = fullname.Split(" ")(0)
        searchusr.Surname = fullname.Split(" ")(1)
        Dim searcher As PrincipalSearcher = New PrincipalSearcher(searchusr)
        For Each user As DirectoryServices.AccountManagement.UserPrincipal In searcher.FindAll()
            userlist.Add(user)
        Next
        searcher.Dispose()
        Return userlist
    End Function

    Public Function getUserPrincipalByAdmission(ByVal ctx As PrincipalContext, ByVal admission As String) As DirectoryServices.AccountManagement.UserPrincipal
        Dim userList As New List(Of DirectoryServices.AccountManagement.UserPrincipal)
        Dim searchusr As DirectoryServices.AccountManagement.UserPrincipal = New DirectoryServices.AccountManagement.UserPrincipal(ctx)
        searchusr.EmployeeId = admission
        Dim searcher As PrincipalSearcher = New PrincipalSearcher(searchusr)
        For Each user As DirectoryServices.AccountManagement.UserPrincipal In searcher.FindAll()
            userList.Add(user)
        Next
        searcher.Dispose()
        If userList.Count > 1 Then
            Dim str As String = "Too many users with same admin number - " & admission & vbCrLf
            For Each user In userList
                str = String.Format("{0}{1}{2}", str, vbCrLf, user.SamAccountName)
            Next
            Sendmail.sendmail(msg)
            Return Nothing
        End If
        If userList.Count = 0 Then
            Return Nothing
        End If
        Return userList(0)
    End Function

    Public Function getAllUsers(ByVal ctx) As List(Of UserPrincipalex)
        Dim userlist As New List(Of UserPrincipalex)
        Dim searchusr As UserPrincipalex = New UserPrincipalex(ctx)
        Dim searcher As PrincipalSearcher = New PrincipalSearcher(searchusr)
        For Each user As UserPrincipal In searcher.FindAll()
            userlist.Add(user)
        Next
        searcher.Dispose()
        searchusr.Dispose()
        Return userlist
    End Function

    Public Function getManagedGroups(ByVal ctx) As List(Of GroupPrincipal)
        Dim groupList As New List(Of GroupPrincipal)
        Dim searchgrp As New GroupPrincipal(ctx)
        Dim searcher As New PrincipalSearcher(searchgrp)
        For Each grp As GroupPrincipal In searcher.FindAll()
            groupList.Add(grp)
        Next
        Return groupList
    End Function

    Public Function createGroup(ByVal ctx As PrincipalContext, ByVal groupName As String) As GroupPrincipal
        If groupName.Contains("&") Then
            groupName = groupName.Replace("&", "and")
        End If
        Dim grptest As GroupPrincipal = getGroup(ctx, groupName)
        If grptest Is Nothing Then
            msg.body = String.Format("Group Created - {0} in {1}", groupName, ctx.Container)
            Sendmail.sendmail(msg)
            Try
                Dim newGroup As GroupPrincipal = New GroupPrincipal(ctx)
                With newGroup
                    .Name = groupName
                    .IsSecurityGroup = True
                    .GroupScope = GroupScope.Universal
                    .SamAccountName = groupName
                    .Save()
                End With
                Return newGroup
            Catch ex As Exception
                Return Nothing
            End Try
        End If
        grptest.Dispose()
        Return Nothing
    End Function

    Public Function getGroup(ByVal ctx As PrincipalContext, ByVal groupName As String) As GroupPrincipal
        Try
            Dim usr As GroupPrincipal = GroupPrincipal.FindByIdentity(ctx, groupName)
            Return usr
        Catch ex As Exception
            Return Nothing
        End Try
    End Function

#Region "ADTree"
    Public Function getUserTree(ctx As PrincipalContext) As UserTree
        Dim uTree As New UserTree
        uTree.name = "DC=internal"
        uTree.type = containerType.dn

        Dim uList As List(Of UserPrincipalex) = getAllUsers(ctx)
        For Each user As UserPrincipalex In uList
            addToTree(user, uTree)
        Next
        myUTree = uTree
        Return uTree
    End Function

    Public Sub addToTree(ByVal user As UserPrincipalex, ByVal uTree As UserTree)
        'get treeNode from user's DistingushedName
        Dim locations As String() = reverseArray(user.DistinguishedName.Split(","))
        Dim treeNode As UserTree = getReleventNode(locations, uTree)
        If treeNode.userList Is Nothing Then
            treeNode.userList = New List(Of UserPrincipalex)
        End If
        treeNode.userList.Add(user)

    End Sub

    Public Function getReleventNode(ByVal locations As String(), ByVal cNode As UserTree) As UserTree
        Dim userNode As UserTree = cNode
        For i As Integer = 2 To locations.Count - 2
            userNode = findMatchingNode(locations(i), userNode)
        Next
        Return userNode
    End Function

    Public Function findMatchingNode(ByVal locationName As String, cNode As UserTree) As UserTree
        If cNode.children IsNot Nothing Then
            For Each node As UserTree In cNode.children
                If node.name.Equals(locationName) Then
                    Return node
                End If
            Next
        Else
            cNode.children = New List(Of UserTree)
        End If

        Dim newNode As UserTree = New UserTree
        newNode.name = locationName
        cNode.children.Add(newNode)
        Return newNode
    End Function

    Public Function getNodeByPath(ByVal path As String) As UserTree
        rflag = False
        Return (getNodeByPath(path, myUTree))
    End Function

    Private Function getNodeByPath(ByVal path As String, treeNode As UserTree) As UserTree
        If rflag Then Return treeNode
        If treeNode.children Is Nothing Then
            rflag = True
            Return treeNode
        End If
        For Each pathElement As String In path.Split(",")
            For Each childnode As UserTree In treeNode.children
                If childnode.name.Equals(pathElement) Then
                    Return getNodeByPath(path.Replace(pathElement & ",", ""), childnode)
                End If
            Next
        Next
        Return treeNode
    End Function
#End Region

End Module

<DirectoryRdnPrefix("CN")>
<DirectoryObjectClass("Person")>
Public Class UserPrincipalex
    Inherits DirectoryServices.AccountManagement.UserPrincipal

    Public Sub New(context As PrincipalContext)
        MyBase.New(context)
    End Sub

    ' Implement the constructor with initialization parameters.
    Public Sub New(context As PrincipalContext, samAccountName As String, password As String, enabled As Boolean)
        MyBase.New(context, samAccountName, password, enabled)
    End Sub

    <DirectoryProperty("extensionAttribute1")>
    Public Property extensionAttribute1() As String
        Get
            If ExtensionGet("extensionAttribute1").Length <> 1 Then
                Return String.Empty
            End If
            Return DirectCast(ExtensionGet("extensionAttribute1")(0), String)
        End Get
        Set(value As String)
            ExtensionSet("extensionAttribute1", value)
        End Set
    End Property

    <DirectoryProperty("extensionAttribute2")>
    Public Property extensionAttribute2() As String
        Get
            If ExtensionGet("extensionAttribute2").Length <> 1 Then
                Return String.Empty
            End If
            Return DirectCast(ExtensionGet("extensionAttribute2")(0), String)
        End Get
        Set(value As String)
            ExtensionSet("extensionAttribute2", value)
        End Set
    End Property

    <DirectoryProperty("extensionAttribute3")>
    Public Property extensionAttribute3() As String
        Get
            If ExtensionGet("extensionAttribute3").Length <> 1 Then
                Return String.Empty
            End If
            Return DirectCast(ExtensionGet("extensionAttribute3")(0), String)
        End Get
        Set(value As String)
            ExtensionSet("extensionAttribute3", value)
        End Set
    End Property

    <DirectoryProperty("extensionAttribute4")>
    Public Property extensionAttribute4() As String
        Get
            If ExtensionGet("extensionAttribute4").Length <> 1 Then
                Return String.Empty
            End If
            Return DirectCast(ExtensionGet("extensionAttribute4")(0), String)
        End Get
        Set(value As String)
            ExtensionSet("extensionAttribute4", value)
        End Set
    End Property

    <DirectoryProperty("profilePath")>
    Public Property ProfilePath() As String
        Get
            If ExtensionGet("profilePath").Length <> 1 Then
                Return String.Empty
            End If

            Return DirectCast(ExtensionGet("profilePath")(0), String)
        End Get
        Set(value As String)
            ExtensionSet("profilePath", value)
        End Set
    End Property

    Public Property Department() As String
        Get
            If ExtensionGet("department").Length <> 1 Then
                Return String.Empty
            End If

            Return DirectCast(ExtensionGet("department")(0), String)
        End Get
        Set(value As String)
            ExtensionSet("department", value)
        End Set
    End Property

    ' Implement the overloaded search method FindByIdentity.
    Public Shared Shadows Function FindByIdentity(context As PrincipalContext, identityValue As String) As UserPrincipal
        Try
            Return DirectCast(FindByIdentityWithType(context, GetType(UserPrincipal), identityValue), UserPrincipal)
        Catch ex As Exception
            Return Nothing
        End Try

    End Function

    ' Implement the overloaded search method FindByIdentity.
    Public Shared Shadows Function FindByIdentity(context As PrincipalContext, identityType As IdentityType, identityValue As String) As UserPrincipal
        Try
            Return DirectCast(FindByIdentityWithType(context, GetType(UserPrincipal), identityType, identityValue), UserPrincipal)
        Catch ex As Exception
            Return Nothing
        End Try

    End Function
End Class

Public Class UserDetails
    Property Username As String
    Property Surname As String
    Property DisplayName As String
    Property Password As String
    Property Description As String
    Property ScriptPath As String
    Property GivenName As String
    Property MiddleName As String
    Property BromcomID
    Property _DistinguishedName As String
    Property EmailAddress As String
    Property _Guid As String
    Property HomeDirectory As String
    Property ProfilePath As String
    Property HomeDrive As String
    Property _LastLogin As String
    Property _SID As String
    Property Enabled As String
    Property UserCannotChangePassword As String
    Property PasswordNeverExpires As String
    Property _LastPasswordSet As String
    Property Groups As List(Of String)
End Class

Public Enum createStatus
    ok = 0
    alreadyExists = 1
    noAD = 2
    fubar = 3
End Enum

Public Class groupDetails
    Private _name As String
    Public members As New List(Of UserPrincipal)

    Property name
        Get
            Return _name
        End Get
        Set(value)
            _name = value
        End Set
    End Property
End Class

Public Enum containerType
        dn
        ou
        cn
    End Enum

Public Class UserTree
    Property type As containerType
    Property name As String
    Property parent As UserTree
    Property children As List(Of UserTree)
    Property userList As List(Of UserPrincipalex)
End Class