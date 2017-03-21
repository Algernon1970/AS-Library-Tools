Imports System.Windows.Forms

Public Module MISReader
    Dim bromcomReader As SoapReader.TPReadOnlyDataServiceSoapClient = New SoapReader.TPReadOnlyDataServiceSoapClient()

    Dim bromcomdata As New DataSet
    Dim soapUser As String = "petessoaptest"
    Dim soapPass As String = "purple"

    Dim studentInfoTable As DataTable

    Public currentStudentsFilter As String = "GETDATE() BETWEEN StartDate AND ISNULL(EndDate, GETDATE())"
    Public currentStaffFilter As String = "GETDATE() BETWEEN StartDate AND ISNULL(EndDate, GETDATE())"
    Public futureFilter As String = "StartDate > GETDATE()"
    Public leaversFilter As String = "EndDate < GETDATE()"

    Public Sub dropStudentInfoTable()
        studentInfoTable = Nothing
    End Sub

    ''' <summary>
    ''' Get a Datatable from Bromcom based on Tablename and Filter
    ''' </summary>
    ''' <param name="tablename">The name of a table to return</param>
    ''' <param name="filter">an SQL filter to limit the records in the table.</param>
    ''' <returns>The datatable containing the data requested by the filter.</returns>
    Public Function getTable(ByVal tablename As String, ByVal filter As String) As DataTable
        Try
            Dim getTable_Table As DataTable
            getTable_Table = Nothing
            getTable_Table = Nothing
            getTable_Table = bromcomReader.getEntityData(tablename, filter, soapUser, soapPass).Tables(0)
            getTable_Table.TableName = tablename
            Return getTable_Table.Copy
        Catch ex As Exception
            Throw New MISException("Not Found : " & filter)
        End Try

    End Function

    ''' <summary>
    ''' Get a UserInfo object representing a member of staff from the Staff Table in Bromcom
    ''' If there is more than 1 user matching the given filter, prompt the user for the correct user.
    ''' </summary>
    ''' <param name="filter">SQL command to find the requested users.</param>
    ''' <returns>A Userinfo object containg the bromcom information for a specific user.</returns>
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

    ''' <summary>
    ''' Get a UserInfo object representing a member of Student from the Student Table in Bromcom
    ''' If there is more than 1 user matching the given filter, prompt the user for the correct user.
    ''' </summary>
    ''' <param name="filter">SQL command to find the requested users.</param>
    ''' <returns>A Userinfo object containg the bromcom information for a specific user.</returns>
    Public Function getStudentByFilter(ByVal filter As String) As UserInfo
        Dim user As UserInfo
        Dim drs As DataRow() = getTable("Students", filter).Select()
        If drs.Count = 0 Then
            Return Nothing
        End If
        If drs.Count = 1 Then
            Return getStudentInfo(drs(0).Field(Of Integer)("studentid"))
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

    ''' <summary>
    ''' Get a UserInfo object for a specified Student
    ''' </summary>
    ''' <param name="id">The Bromcom ID of the user.  (unique)</param>
    ''' <returns>Userinfo object representing the student</returns>
    Public Function getStudentByID(ByVal id As String) As UserInfo
        Return getStudentByFilter(String.Format("StudentID like '{0}'", id))
    End Function

    ''' <summary>
    ''' Get a UserInfo object for a specified Staff member
    ''' </summary>
    ''' <param name="id">The Bromcom ID of the user.  (unique)</param>
    ''' <returns>Userinfo object representing the Staff member</returns>
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

    Public Function getStaffInfo(name As String) As UserInfo
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

    Public Function getStudentInfo(name As String) As UserInfo
        Dim drs As DataRow() = getTable("Students", String.Format("StudentID like '{0}'", name.Split(vbTab)(0))).Select()
        Dim user As New UserInfo
        user.UPN = drs(0).Field(Of String)("UPN")
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

    Public Function getStaffTutorGroup(ByVal staffID As String) As String
        Dim ceFilter As String = String.Format("staffid like '{0}' and (CollectionRoleTypeDescription like 'Main Tutor' or CollectionRoleTypeDescription like 'Additional Tutor' or CollectionRoleTypeDescription like 'Head of House') and enddate is null", staffID)
        Dim CET As DataTable = getTable("CollectionExecutives", ceFilter)
        Dim cidr As DataRow() = CET.Select()
        If IsNothing(cidr) Then
            Return ("Nothing")
        End If
        If cidr.Count > 1 Then
            Return String.Format("Found {0} records.", cidr.Count)
        ElseIf cidr.Count = 0 Then
            Return "Got No Records"
        Else
            Dim cidFilter As String = String.Format("CollectionID like {0}", cidr(0).Field(Of Integer)("CollectionID"))
            Dim cRow As DataRow() = getTable("Collections", cidFilter).Select()
            Return String.Format("{0}", cRow(0).Field(Of String)("CollectionName"))
        End If

    End Function

    Public Function getHouseFromGroup(ByRef hs As String) As String
        If hs.ToLower.StartsWith("got") Then Return "Undefined"
        If hs.ToLower.StartsWith("a") Then Return "Ashe"
        If hs.ToLower.StartsWith("b") Then Return "Bullen"
        If hs.ToLower.StartsWith("c") Then Return "Crewe"
        If hs.ToLower.StartsWith("e") Then Return "Erdington"
        If hs.ToLower.StartsWith("f") Then Return "Ferrers"
        If hs.ToLower.StartsWith("g") Then Return "Gylby"
        If hs.ToLower.StartsWith("h") Then Return "Hastings"
        If hs.ToLower.StartsWith("l") Then Return "Loudoun"
        Return "NONE"
    End Function

    Public Function getStaffClassList(ByVal staffID As String) As List(Of String)
        Dim classList As New List(Of String)
        Dim cRow As DataRow()

        Dim ceFilter As String = String.Format("staffid like '{0}' and (CollectionRoleTypeDescription like 'Teacher' or CollectionRoleTypeDescription like 'Main Teacher' or CollectionRoleTypeDescription like 'Additional Teacher') and (enddate is null or enddate > getdate())", staffID)
        Dim cedrs As DataRow() = getTable("CollectionExecutives", ceFilter).Select()
        For Each cedr As DataRow In cedrs
            cRow = getTable("Collections", String.Format("CollectionID like {0}", cedr.Field(Of Integer)("CollectionID"))).Select()
            classList.Add(cRow(0).Field(Of String)("CollectionName"))
        Next
        Return classList
    End Function

    Private Function getStudentRows(ByVal sid As String) As DataRow()
        Dim dt As DataRow()
        If IsNothing(sid) Or sid = 0 Then Return Nothing

        If IsNothing(studentInfoTable) Then
            studentInfoTable = getStudentInfoTables()
        End If
        dt = studentInfoTable.Select(String.Format("studentid like '{0}'", sid))
        Return dt
    End Function

    Public Function getStudentObject(ByVal sid As String) As studentObject
        Dim drs As DataRow() = getStudentRows(sid)
        Dim so As New studentObject
        If IsNothing(drs) Then
            so.userInfo.id = sid
            so.userInfo.forename = "Not"
            so.userInfo.surname = "Found"
        Else
            If drs.Length = 0 Then
                so.userInfo.id = sid
                so.userInfo.forename = "Not"
                so.userInfo.surname = "Found"
            Else
                so.userInfo.id = sid
                so.userInfo.forename = drs(0).Field(Of String)("forename")
                so.userInfo.surname = drs(0).Field(Of String)("surname")
                'so.userInfo.middlename = drs(0).Field(Of String)("middlename")
                'so.userInfo.startDate = drs(0).Field(Of String)("startdate")
                'so.userInfo.endDate = drs(0).Field(Of String)("enddate")
            End If
            For Each row As DataRow In drs
                so.classlist.Add(row.Field(Of String)("classname"))
            Next
        End If

        Return so
    End Function


    Private Function getStudentInfoTables() As DataTable
        Dim tempTable As DataTable
        tempTable = getTable("Students", currentStudentsFilter)
        tempTable.TableName = "currentStudents"
        bromcomdata.Tables.Add(tempTable)

        tempTable = getTable("SubjectClasses", currentStudentsFilter)
        tempTable.TableName = "SubjectClasses"
        bromcomdata.Tables.Add(tempTable)

        tempTable = getTable("CollectionAssociates", currentStudentsFilter)
        tempTable.TableName = "CollectionAssociates"
        bromcomdata.Tables.Add(tempTable)

        tempTable = getTable("VLEUsers", "")
        tempTable.TableName = "VleUsers"
        bromcomdata.Tables.Add(tempTable)


        Dim table As DataTable
        Dim students As DataTable = bromcomdata.Tables("CurrentStudents")
        Dim classes As DataTable = bromcomdata.Tables("SubjectClasses")
        Dim collection As DataTable = bromcomdata.Tables("CollectionAssociates")
        Dim query As IEnumerable = From link In collection
                                   Join student In students
                       On link.Field(Of Integer)("PersonID") Equals student.Field(Of Integer)("StudentID")
                                   Join classDetail In classes
                       On link.Field(Of Integer)("CollectionID") Equals classDetail.Field(Of Integer)("ClassID")
                                   Select New With {.forename = student.Field(Of String)("PreferredFirstName"),
                                        .classname = classDetail.Field(Of String)("ClassDescription"),
                                        .surname = student.Field(Of String)("PreferredLastName"),
                                        .studentID = "" & student.Field(Of Integer)("StudentID"),
                                        .admission = student.Field(Of String)("AdmissionNumber"),
                                        .tutor = student.Field(Of String)("TutorGroup"),
                                        .subject = classDetail.Field(Of String)("SubjectDescription"),
                                        .yearGroup = student.Field(Of String)("YearGroup"),
                                        .vleuser = student.Field(Of String)("ChronologicalYearGroup")
                                       }
        table = convertToTable(query)
        Return table
    End Function

    Private Function convertToTable(ByVal info As IEnumerable) As DataTable
        Dim table As New DataTable("TempTable")
        Dim row As DataRow
        Dim columns() = {"AdmissionNo", "StudentID", "Forename", "Surname", "ClassName", "Subject", "TutorGroup", "YearGroup", "VleUser"}
        For Each colName As String In columns
            table.Columns.Add(colName)
        Next

        For Each entry In info
            row = table.NewRow()
            row("Forename") = entry.forename
            row("Surname") = entry.surname
            row("AdmissionNo") = entry.admission
            row("StudentID") = entry.studentID
            row("ClassName") = entry.classname
            row("Subject") = entry.subject
            row("TutorGroup") = entry.tutor
            row("YearGroup") = entry.yearGroup
            row("VleUser") = entry.vleuser
            table.Rows.Add(row)
        Next
        info = Nothing
        row = Nothing
        Return table
    End Function

    Public Function getSubjectFromClass(ByVal className As String) As String
        Try
            Dim sdrs As DataRow() = getTable("Subjectclasses", String.Format("ClassDescription like '{0}' and enddate > getdate()", className)).Select()
            If sdrs Is Nothing Then
                Throw New MISException("No description for " & className)
            End If
            If sdrs.Count > 0 Then
                Return sdrs(0).Field(Of String)("SubjectDescription")
            Else
                Return "nothing"
            End If

        Catch ex As Exception
            Throw New MISException("getSubjectFromClass " & className)
            Return "nothing"
        End Try
    End Function

    Public Function getSubjectsFromStaff(ByVal staffID As String) As List(Of String)
        Dim subjectList As New List(Of String)
        Dim clist As List(Of String) = getStaffClassList(staffID)
        Dim subject As String = "nothing"
        For Each className As String In clist
            Try
                subject = getSubjectFromClass(className)
            Catch ex As Exception
                subject = "nothing"
            End Try

            If Not subject.Equals("nothing") Then
                If Not subjectList.Contains(subject) Then
                    subjectList.Add(subject)
                End If
            End If
        Next

        Return subjectList
    End Function

    ''' <summary>
    ''' Remove illegal characters from Bromcom Groups, so they can be used as AD Groups.
    ''' </summary>
    ''' <param name="name">The name of the group</param>
    ''' <returns>A Sanitized version of the group name</returns>
    Public Function sanitizeGroupName(ByRef name As String) As String
        If name Is Nothing Then Return "None"
        If name.Contains("&") Then
            name = name.Replace("&", "and")
        End If
        If name.Contains("(") Then
            Dim tempname() As String = name.Split(" ")
            name = tempname(0)
        End If
        Return name
    End Function

    Public Structure UserInfo
        Public surname As String
        Public forename As String
        Public middlename As String
        Public id As String
        Public startDate As Date
        Public endDate As Date
        Public UPN As String
    End Structure
End Module

Public Enum ouname
    Staff
    Students
End Enum

Public Class studentObject
    Public userInfo As UserInfo
    Public classlist As New List(Of String)
End Class

<Serializable()>
Public Class MISException : Inherits System.Exception
    Public Sub New()
        MyBase.New()
    End Sub

    Public Sub New(ByVal message As String)
        MyBase.New(message)
    End Sub

    Public Sub New(ByVal message As String, ByVal innerException As System.Exception)
        MyBase.New(message, innerException)
    End Sub
End Class

