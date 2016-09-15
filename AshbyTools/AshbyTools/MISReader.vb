Imports System.Windows.Forms

Public Module MISReader
    Dim bromcomReader As Soapreader.TPReadOnlyDataServiceSoapClient = New SoapReader.TPReadOnlyDataServiceSoapClient()

    Dim bromcomdata As New DataSet
    Dim soapUser As String = "petessoaptest"
    Dim soapPass As String = "purple"

    Public currentStudentsFilter As String = "GETDATE() BETWEEN StartDate AND ISNULL(EndDate, GETDATE())"
    Public currentStaffFilter As String = "GETDATE() BETWEEN StartDate AND ISNULL(EndDate, GETDATE())"
    Public futureFilter As String = "StartDate > GETDATE()"
    Public leaversFilter As String = "EndDate < GETDATE()"

    Public Function getTable(ByVal tablename As String, ByVal filter As String) As DataTable
        Dim getTable_Table As DataTable
        getTable_Table = Nothing
        getTable_Table = Nothing
        getTable_Table = bromcomReader.getEntityData(tablename, filter, soapUser, soapPass).Tables(0)
        getTable_Table.TableName = tablename
        Return getTable_Table.Copy
    End Function

    Public Function getStaffByFilter(ByVal filter As String) As UserInfo
        Dim user As UserInfo
        Dim drs As DataRow() = getTable("Staff", filter).Select()
        If drs.Count = 0 Then
            Return Nothing
        End If
        Dim namelist As New List(Of String)
        Dim displayline As String
        For Each row As DataRow In drs
            displayline = String.Format("{1}{0}{2}{0}{3}{0}{4}", vbTab, row.Field(Of Integer)("staffid"), row.Field(Of String)("firstname"),
                                         row.Field(Of String)("lastname"), row.Field(Of Date)("startdate"))
            namelist.Add(displayline)
        Next
        Dim chooser As New ListChooser(namelist)
        Dim res As DialogResult = chooser.ShowDialog
        If res = DialogResult.OK Then
            user = getStaffInfo(chooser.getSelected(0))
        Else
            user = Nothing

        End If
        Return user
    End Function

    Public Function getStudentByFilter(ByVal filter As String) As UserInfo
        Dim user As UserInfo
        Dim drs As DataRow() = getTable("Students", filter).Select()
        If drs.Count = 0 Then
            Return Nothing
        End If
        Dim namelist As New List(Of String)
        Dim displayline As String
        For Each row As DataRow In drs
            displayline = String.Format("{1}{0}{2}{0}{3}{0}{4}", vbTab, row.Field(Of Integer)("studentid"), row.Field(Of String)("firstname"),
                                         row.Field(Of String)("lastname"), row.Field(Of Date)("startdate"))
            namelist.Add(displayline)
        Next
        Dim chooser As New ListChooser(namelist)
        Dim res As DialogResult = chooser.ShowDialog
        If res = DialogResult.OK Then
            user = getStudentInfo(chooser.getSelected(0))
        Else
            user = Nothing

        End If
        Return user
    End Function

    Public Function getStudentByID(ByVal id As String) As UserInfo
        Return getStudentByFilter(String.Format("StudentID like '{0}'", id))
    End Function

    Public Function getStaffByID(ByVal id As String) As UserInfo
        Return getStaffByFilter(String.Format("StaffID like '{0}'", id))
    End Function

    Public Function MISLookup(ByVal filter As String, ou As ouname) As UserInfo
        Dim uinfo As UserInfo
        If ou = ouname.Staff Then
            If filter.Contains("THEID") Then
                filter = filter.Replace("THEID", "STAFFID")
            End If
            uinfo = getStaffByFilter(filter)
            If IsNothing(uinfo.id) Then
                Throw New Exception("User Not Found")
            End If
        ElseIf ou = ouname.Students Then
            If filter.Contains("THEID") Then
                filter = filter.Replace("THEID", "StudentID")
            End If
            uinfo = getStudentByFilter(filter)
            If IsNothing(uinfo.id) Then
                Throw New Exception("User Not Found")
            End If
        Else
            Throw New Exception("Only Staff or Student")
        End If

            Return uinfo
    End Function

    Private Function getStaffInfo(name As String) As UserInfo
        Dim drs As DataRow() = getTable("Staff", String.Format("StaffID like '{0}'", name.Split(vbTab)(0))).Select()
        Dim user As New UserInfo
        user.forename = drs(0).Field(Of String)("Firstname")
        user.surname = drs(0).Field(Of String)("LastName")
        user.middlename = drs(0).Field(Of String)("Middlename")
        user.id = drs(0).Field(Of Integer)("StaffID").ToString
        If Not IsNothing(drs(0).Field(Of Date)("StartDate")) Then
            user.startDate = drs(0).Field(Of Object)("StartDate")
        Else
            user.startDate = New Date(0)
        End If
        If Not IsNothing(drs(0).Field(Of Object)("EndDate")) Then
            user.endDate = drs(0).Field(Of Date)("EndDate")
        Else
            user.endDate = New Date(9999, 12, 31)
        End If

        Return user
    End Function

    Private Function getStudentInfo(name As String) As UserInfo
        Dim drs As DataRow() = getTable("Students", String.Format("StudentID like '{0}'", name.Split(vbTab)(0))).Select()
        Dim user As New UserInfo
        user.forename = drs(0).Field(Of String)("Firstname")
        user.surname = drs(0).Field(Of String)("LastName")
        user.middlename = drs(0).Field(Of String)("Middlename")
        user.id = drs(0).Field(Of Integer)("StudentID").ToString
        If Not IsNothing(drs(0).Field(Of Date)("StartDate")) Then
            user.startDate = drs(0).Field(Of Object)("StartDate")
        Else
            user.startDate = New Date(0)
        End If
        If Not IsNothing(drs(0).Field(Of Object)("EndDate")) Then
            user.endDate = drs(0).Field(Of Date)("EndDate")
        Else
            user.endDate = New Date(9999, 12, 31)
        End If


        Return user
    End Function

    Public Structure UserInfo
        Public surname As String
        Public forename As String
        Public middlename As String
        Public id As String
        Public startDate As Date
        Public endDate As Date
    End Structure
End Module

Public Enum ouname
    Staff
    Students
End Enum

