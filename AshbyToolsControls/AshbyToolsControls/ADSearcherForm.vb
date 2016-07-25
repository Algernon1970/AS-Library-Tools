Imports System.DirectoryServices.AccountManagement
Imports System.Text.RegularExpressions
Imports AshbyTools

Public Class ADSearcherForm
    Public selectedItem As String = ""
    Public Sub displayUsers(ByRef ulist As List(Of UserPrincipalex))
        UserListBox.Items.Clear()

        For Each user As UserPrincipalex In ulist
            UserListBox.Items.Add(user.SamAccountName)
        Next
    End Sub

    Public Sub New(ByRef ulist As List(Of UserPrincipalex))

        ' This call is required by the designer.
        InitializeComponent()
        displayUsers(ulist)
    End Sub

    Public Function getselected() As String
        Return UserListBox.SelectedItem
    End Function

    Private Sub UserListBox_DoubleClick(sender As Object, e As EventArgs) Handles UserListBox.DoubleClick
        selectedItem = UserListBox.SelectedItem
        DialogResult = System.Windows.Forms.DialogResult.OK
        Me.Close()
    End Sub
End Class

