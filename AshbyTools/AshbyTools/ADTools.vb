﻿Imports System.DirectoryServices.AccountManagement
Imports System.Threading
Imports System.Windows.Forms


Public Module ADTools
    Dim msg As New eMailMessage
    Public userCTX As PrincipalContext = getConnection("as.internal", usersCTXString)
    Public yearCTX As PrincipalContext = getConnection("as.internal", yearCTXString)
    Public classCTX As PrincipalContext = getConnection("as.internal", classCTXString)
    Public groupsCTX As PrincipalContext = getConnection("as.internal", groupsCTXString)
    Public staffGroupsCTX As PrincipalContext = getConnection("as.internal", staffGroupsCTXString)
    Public tlGroupsCTX As PrincipalContext = getConnection("as.internal", tlGroupsCTXString)
    Public subjectGroupsCTX As PrincipalContext = getConnection("as.internal", SubjectGroupsCTXString)
    Public tutorGroupsCTX As PrincipalContext = getConnection("as.internal", TutorGroupsCTXString)
    Public computerCTX As PrincipalContext = getConnection("as.internal", ComputerCTXString)
    Dim myUTree As ObjectTree
    Dim rflag As Boolean = False

    ''' <summary>
    ''' Wait till a newly created AD account is available to read.
    ''' </summary>
    ''' <param name="userdetails"></param>
    ''' <param name="path"></param>
    ''' <returns></returns>
    Public Function waitTillAvailable(ByRef userdetails As UserDetails, ByRef path As String) As PrincipalContext
        Dim myctx As PrincipalContext
        myctx = ADTools.getConnection("as.internal", path)
        While True
            If ADTools.userExists(myctx, userdetails) Then
                Thread.Sleep(2000)
                Return myctx
            End If
            Thread.Sleep(250)
        End While
        Return Nothing
    End Function

    Public Function createDirectories(ByRef user As UserDetails, ByRef path As String, ByRef myctx As PrincipalContext) As createStatus
        Dim result As Boolean = False
        If user.HomeDirectory.ToLower.StartsWith("svr") Then
            user.HomeDirectory = String.Format("\\{0}", user.HomeDirectory)
        End If
        If Not FileOperations.exists(user.HomeDirectory) Then
            FileOperations.createDirectory(user.HomeDirectory)

            Dim looped As Integer = 0
            Dim domainUserName As String = myctx.Name.Split(".")(0) & "\" & user.Username

            For looped = 0 To 40
                result = FileOperations.setACL(user.HomeDirectory, domainUserName, False)
                If result = False Then
                    Thread.Sleep(250)
                Else
                    Exit For
                End If
            Next

            If result = False Then
                Return createStatus.fubar
            End If
        End If
        Return createStatus.ok
    End Function

    Public Sub setupMail(ByRef eMailFrom As String, ByRef Optional eMailto As String = "itsupport@ashbyschool.org.uk",
                         Optional ByRef emailSubject As String = "Automailed Message", Optional ByRef body As String = "Blank",
                         Optional ByRef ishtml As Boolean = False)
        msg.mailfrom = eMailFrom
        msg.mailto = eMailto
        msg.subject = emailSubject
        msg.isBodyHTML = ishtml
        msg.body = body
    End Sub

    Public Function getConnection(ByRef domain As String, ByRef OU As String) As PrincipalContext
        Return New PrincipalContext(ContextType.Domain, domain, OU, adUser, adPass)
    End Function

    Public Function userExists(ByRef ctx As PrincipalContext, ByRef user As UserDetails) As Boolean
        Dim usr As UserPrincipal = UserPrincipal.FindByIdentity(ctx, user.Username)
        If usr IsNot Nothing Then
            usr.Dispose()
            usr = Nothing
            Return True
        Else
            Return False
        End If
    End Function

    Public Sub addToGroups(ByRef ctx As PrincipalContext, ByRef user As UserDetails)
        ' add User to group
        If user.Groups.Count > 0 Then
            For Each group As String In user.Groups
                addUserToGroup(ctx, user.Username, group)
            Next
        End If
    End Sub

    Public Function getUserPrincipalByID(ByRef ctx As PrincipalContext, ByRef admission As String) As DirectoryServices.AccountManagement.UserPrincipal
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
            For Each lusr In userList
                str = String.Format("{0}{1}{2}", str, vbCrLf, lusr.SamAccountName)
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

    Public Function getGroupPrincipalbyName(ByRef ctx As PrincipalContext, group As String) As GroupPrincipal
        Try
            Dim usr As GroupPrincipal = GroupPrincipal.FindByIdentity(ctx, group)
            Return usr
        Catch ex As Exception
            Throw
        End Try
    End Function

    Public Function getComputerPrincipalByName(ByRef ctx As PrincipalContext, name As String) As ComputerPrincipal
        Try
            Dim usr As ComputerPrincipal = ComputerPrincipal.FindByIdentity(ctx, name)
            Return usr
        Catch ex As Exception
            Throw
        End Try
    End Function

    Public Function addUserToGroup(ByRef ctx As PrincipalContext, user As String, ByRef group As String) As String
        Try
            Dim usr As DirectoryServices.AccountManagement.UserPrincipal = getUserPrincipalbyUsername(userCTX, user)
            Dim grp As GroupPrincipal = getGroupPrincipalbyName(ctx, group)
            If Not grp.Members.Contains(usr) Then
                grp.Members.Add(usr)
            End If
            grp.Save()
            grp.dispose()
            grp = Nothing
            Return "ok"
        Catch ex As Exception
            Return ex.Message
            'no groups to add
        End Try
    End Function

    Public Function removeUserFromGroup(ByRef ctx As PrincipalContext, user As String, group As String) As String
        Try
            Dim usr As DirectoryServices.AccountManagement.UserPrincipal = getUserPrincipalbyUsername(userCTX, user)
            Dim grp As GroupPrincipal = getGroupPrincipalbyName(ctx, group)
            If grp.Members.Contains(userCTX, IdentityType.SamAccountName, usr.SamAccountName) Then
                grp.Members.Remove(usr)
            End If
            grp.Save()
            grp.dispose()
            grp = Nothing
            Return "ok"
        Catch ex As Exception
            Return ex.Message
            'no groups to add
        End Try
    End Function

    Public Function createUser(ByRef ctx As PrincipalContext, ByRef user As UserDetails) As createStatus
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

    Private Sub modify(ByRef ctx As PrincipalContext, ByRef user As UserDetails)
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

        usr.ProfilePath = If(user.ProfilePath.StartsWith("\\"), user.ProfilePath, "\\" & user.ProfilePath)
        usr.HomeDrive = user.HomeDrive
        usr.HomeDirectory = If(user.HomeDirectory.StartsWith("\\"), user.HomeDirectory, "\\" & user.HomeDirectory)
        usr.SetPassword(If(user.Password = "", "password", user.Password))

        usr.Description = If(user.Description = "", " ", user.Description)
        usr.MiddleName = If(user.MiddleName = "", " ", user.MiddleName)
        usr.EmployeeId = If(user.BromcomID = "", "0", user.BromcomID)

        usr.UserPrincipalName = user.Username & emailDomain

        'email enable test
        usr.EmailAddress = usr.UserPrincipalName

        usr.Save()
        usr.Dispose()
    End Sub

    Public Function getUserPrincipalexbyUsername(ByRef ctx As PrincipalContext, userName As String) As UserPrincipalex
        Try
            Dim usr As UserPrincipalex = UserPrincipalex.FindByIdentity(ctx, userName)
            Return usr
        Catch ex As Exception
            Return Nothing
        End Try
    End Function

    Public Function getUserPrincipalbyUsername(ByRef ctx As PrincipalContext, userName As String) As UserPrincipal
        Try
            Dim usr As DirectoryServices.AccountManagement.UserPrincipal = DirectoryServices.AccountManagement.UserPrincipal.FindByIdentity(ctx, userName)
            Return usr
        Catch ex As Exception
            Return Nothing
        End Try
    End Function

    Public Function getUserPrincipalsByFullName(ByRef ctx As PrincipalContext, fullname As String) As List(Of DirectoryServices.AccountManagement.UserPrincipal)
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

    Public Function getUserPrincipalByAdmission(ByRef ctx As PrincipalContext, ByRef admission As String) As DirectoryServices.AccountManagement.UserPrincipal
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
            For Each pleb In userList
                str = String.Format("{0}{1}{2}", str, vbCrLf, pleb.SamAccountName)
            Next
            Sendmail.sendmail(msg)
            Return Nothing
        End If
        If userList.Count = 0 Then
            Return Nothing
        End If
        Return userList(0)
    End Function

    Public Function getAllUsers(ByRef ctx) As List(Of UserPrincipalex)
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

    Public Function getAllComputers(ByRef ctx) As List(Of ComputerPrincipal)
        Dim computerList As New List(Of ComputerPrincipal)
        Try
            Dim searchComputer As ComputerPrincipal = New ComputerPrincipal(ctx)
            Dim searcher As PrincipalSearcher = New PrincipalSearcher(searchComputer)

            For Each computer As ComputerPrincipal In searcher.FindAll()
                computerList.Add(computer)
            Next
            searcher.Dispose()
            searchComputer.Dispose()
        Catch ex As Exception

        End Try

        Return computerList
    End Function

    Public Function getGroupsByUser(ByRef id As String) As List(Of String)
        Dim grps As New List(Of String)
        Dim usr As UserPrincipal = getUserPrincipalByID(userCTX, id)
        Dim usrGrps As PrincipalSearchResult(Of Principal) = usr.GetGroups()
        For Each grp As Principal In usrGrps
            grps.Add(grp.Name)
        Next
        Return grps
    End Function

    Public Function getManagedGroups(ByRef ctx) As List(Of GroupPrincipal)
        Dim groupList As New List(Of GroupPrincipal)
        Dim searchgrp As New GroupPrincipal(ctx)
        Dim searcher As New PrincipalSearcher(searchgrp)
        For Each grp As GroupPrincipal In searcher.FindAll()
            groupList.Add(grp)
        Next
        Return groupList
    End Function

    Public Function getManagedGroupNames(ByRef ctx) As List(Of String)
        Dim grpNames As New List(Of String)
        Dim grps As List(Of GroupPrincipal) = getManagedGroups(ctx)
        For Each grp In grps
            grpNames.Add(grp.Name)
        Next
        Return grpNames
    End Function

    Public Function getStaffGroupNames() As List(Of String)
        Dim grpNames As New List(Of String)
        Dim grps As List(Of GroupPrincipal) = getManagedGroups(staffGroupsCTX)
        For Each grp In grps
            grpNames.Add(grp.Name)
        Next
        Return grpNames
    End Function

    Public Function createGroup(ByRef ctx As PrincipalContext, ByRef groupName As String) As GroupPrincipal
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

    Public Function getGroup(ByRef ctx As PrincipalContext, ByRef groupName As String) As GroupPrincipal
        Try
            Dim usr As GroupPrincipal = GroupPrincipal.FindByIdentity(ctx, groupName)
            Return usr
        Catch ex As Exception
            Return Nothing
        End Try
    End Function


    ''' <summary>
    ''' Is the name a user?
    ''' </summary>
    ''' <param name="name">is this name a samAccountName</param>
    ''' <returns>true if name = user, false if not.</returns>
    Public Function isUser(ByRef name As String) As Boolean
        Dim myctx As PrincipalContext = ADTools.getConnection(SchoolSettings.domain, SchoolSettings.usersCTXString)
        If IsNothing(UserPrincipal.FindByIdentity(myctx, name)) Then
            myctx.Dispose()
            Return False
        Else
            myctx.Dispose()
            Return True
        End If
    End Function

    Public Function getGroupMembers(ByRef name As String) As List(Of String)
        Dim mylist As New List(Of String)
        Dim myctx As PrincipalContext = ADTools.getConnection(SchoolSettings.domain, SchoolSettings.groupsCTXString)
        Dim theGroup As GroupPrincipal = getGroup(myctx, name)
        Dim members As PrincipalCollection = theGroup.Members
        For Each member As Principal In members
            mylist.Add(member.Name)
            'If isUser(member.Name) Then
            '    mylist.Add(member.Name)
            '    'Else
            '    '    mylist = mylist.Concat(getGroupMembers(member.Name))
            'End If
        Next
        Return mylist
    End Function

#Region "ADTree"
    Public Function getUserTree(ctx As PrincipalContext) As ObjectTree
        Dim uTree As New ObjectTree
        uTree.name = "DC=internal"
        uTree.type = containerType.dn

        Dim uList As List(Of UserPrincipalex) = getAllUsers(ctx)
        For Each user As UserPrincipalex In uList
            addUserToTree(user, uTree)
        Next
        myUTree = uTree
        Return uTree
    End Function

    Public Function getGroupTree(ctx As PrincipalContext) As ObjectTree
        Dim uTree As New ObjectTree
        uTree.name = "DC=internal"
        uTree.type = containerType.dn
        Dim uList As List(Of GroupPrincipal) = getManagedGroups(ctx)
        For Each grp As GroupPrincipal In uList
            addGroupToTree(grp, uTree)
        Next
        myUTree = uTree
        Return uTree
    End Function

    Public Sub addUserToTree(ByRef user As UserPrincipalex, ByRef uTree As ObjectTree)
        'get treeNode from user's DistingushedName
        Dim locations As String() = arrayReverse(user.DistinguishedName.Split(","))
        Dim treeNode As ObjectTree = getReleventNode(locations, uTree)
        If treeNode.objectList Is Nothing Then
            treeNode.objectList = New List(Of NodeContainer)
        End If
        Dim n As New NodeContainer
        n.type = TypeOfContainer.userex
        n.userex = user
        n.user = Nothing
        n.group = Nothing
        treeNode.objectList.Add(n)
    End Sub
    Public Sub addGroupToTree(ByRef grp As GroupPrincipal, ByRef uTree As ObjectTree)
        Dim locations As String() = arrayReverse(grp.DistinguishedName.Split(","))
        Dim treeNode As ObjectTree = getReleventNode(locations, uTree)
        If treeNode.objectList Is Nothing Then
            treeNode.objectList = New List(Of NodeContainer)
        End If
        Dim n As New NodeContainer
        n.type = TypeOfContainer.group
        n.group = grp
        n.user = Nothing
        n.group = Nothing
        treeNode.objectList.Add(n)
    End Sub

    Public Function getReleventNode(ByRef locations As String(), ByRef cNode As ObjectTree) As ObjectTree
        Dim ObjectNode As ObjectTree = cNode
        For i As Integer = 2 To locations.Count - 2
            ObjectNode = findMatchingNode(locations(i), ObjectNode)
        Next
        Return ObjectNode
    End Function

    Public Function findMatchingNode(ByRef locationName As String, cNode As ObjectTree) As ObjectTree
        If cNode.children IsNot Nothing Then
            For Each node As ObjectTree In cNode.children
                If node.name.Equals(locationName) Then
                    Return node
                End If
            Next
        Else
            cNode.children = New List(Of ObjectTree)
        End If

        Dim newNode As ObjectTree = New ObjectTree
        newNode.name = locationName
        cNode.children.Add(newNode)
        Return newNode
    End Function

    Public Function getNodeByPath(ByRef path As String) As ObjectTree
        rflag = False
        Return (getNodeByPath(path, myUTree))
    End Function

    Private Function getNodeByPath(ByRef path As String, treeNode As ObjectTree) As ObjectTree
        If rflag Then Return treeNode
        If treeNode.children Is Nothing Then
            rflag = True
            Return treeNode
        End If
        For Each pathElement As String In path.Split(",")
            For Each childnode As ObjectTree In treeNode.children
                If childnode.name.Equals(pathElement) Then
                    Return getNodeByPath(path.Replace(pathElement & ",", ""), childnode)
                End If
            Next
        Next
        Return treeNode
    End Function

    ''' <summary>
    ''' Convert a path in the form as/internal/user into cn=user,dc=internal,dc=as
    ''' </summary>
    ''' <param name="path"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function getADPath(ByRef path As String) As String
        Dim backPath As String() = arrayReverse(path.Split(","))
        Dim returnString As String = String.Join(",", backPath)
        Return returnString.Substring(1)
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

    <DirectoryProperty("pager")>
    Public Property pager() As String
        Get
            If ExtensionGet("pager").Length <> 1 Then
                Return String.Empty
            End If
            Return DirectCast(ExtensionGet("pager")(0), String)
        End Get
        Set(value As String)
            ExtensionSet("pager", value)
        End Set
    End Property

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

    <DirectoryProperty("extensionAttribute5")>
    Public Property extensionAttribute5() As String
        Get
            If ExtensionGet("extensionAttribute5").Length <> 1 Then
                Return String.Empty
            End If
            Return DirectCast(ExtensionGet("extensionAttribute5")(0), String)
        End Get
        Set(value As String)
            ExtensionSet("extensionAttribute5", value)
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
    Public Shared Shadows Function FindByIdentity(context As PrincipalContext, identityValue As String) As UserPrincipalex
        Try
            Return DirectCast(FindByIdentityWithType(context, GetType(UserPrincipalex), identityValue), UserPrincipalex)
        Catch ex As Exception
            Return Nothing
        End Try

    End Function

    ' Implement the overloaded search method FindByIdentity.
    Public Shared Shadows Function FindByIdentity(context As PrincipalContext, identityType As IdentityType, identityValue As String) As UserPrincipalex
        Try
            Return DirectCast(FindByIdentityWithType(context, GetType(UserPrincipalex), identityType, identityValue), UserPrincipalex)
        Catch ex As Exception
            Return Nothing
        End Try

    End Function

    Public Function waitTillAvailable(ByRef domain As String, ByRef userdetails As UserDetails, ByRef path As String) As PrincipalContext
        Dim myctx As PrincipalContext

        myctx = ADTools.getConnection(domain, path)
        For loopcnt As Integer = 1 To 250
            If ADTools.userExists(myctx, userdetails) Then
                Thread.Sleep(2000)
                Return myctx
            End If
            Thread.Sleep(250)
        Next
        Return Nothing
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

Public Class ObjectTree
    Property type As containerType
    Property name As String
    Property parent As ObjectTree
    Property children As List(Of ObjectTree)
    Property objectList As List(Of NodeContainer)
End Class

Public Enum TypeOfContainer
    user
    userex
    group
End Enum

Public Structure NodeContainer
    Public type As TypeOfContainer
    Public userex As UserPrincipalex
    Public user As UserPrincipal
    Public group As GroupPrincipal
End Structure