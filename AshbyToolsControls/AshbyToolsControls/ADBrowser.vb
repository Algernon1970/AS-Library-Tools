Imports AshbyTools
Imports System.DirectoryServices.AccountManagement

Public Class ADBrowser
    Public Event NodeSelect(ByVal path As String)
    Public Event UserSelection(ByVal ulist As List(Of UserPrincipalex))
    Public Event UserDoubleClick(ByVal sender As Object, ByVal e As EventArgs)

    Public Sub loadAD(ByVal domain As String, ByVal ou As String)
        AdTreeView1.loadAD(getConnection(domain, ou))
    End Sub

    Private Sub AdTreeView1_NodeMouseClick(sender As Object, e As System.Windows.Forms.TreeNodeMouseClickEventArgs) Handles AdTreeView1.NodeMouseClick
        Dim path As String = AdTreeView1.GetNodeAt(e.X, e.Y).FullPath.Replace("\", ",")
        ContainerLabel.Text = path
        Dim listit As List(Of UserPrincipalex) = getNodeByPath(path).userList
        UserListBox.Items.Clear()
        Try
            UserListBox.Sorted = True
            For Each user As UserPrincipalex In listit
                UserListBox.Items.Add(user)
            Next

            RaiseEvent NodeSelect(path)
        Catch ex As Exception

        End Try
    End Sub

    Private Sub UserListBox_SelectedIndexChanged(sender As Object, e As EventArgs) Handles UserListBox.SelectedIndexChanged
        Dim users As List(Of UserPrincipalex) = getSelectedUsers()
        RaiseEvent UserSelection(users)
    End Sub

    Public Function GetSelectedOU() As String
        Return ContainerLabel.Text
    End Function

    Public Function getSelectedUsers() As List(Of UserPrincipalex)
        Dim users As New List(Of UserPrincipalex)

        For Each user As UserPrincipalex In UserListBox.SelectedItems
            users.Add(user)
        Next
        Return users
    End Function

    Private Sub UserListBox_DoubleClick(sender As Object, e As EventArgs) Handles UserListBox.DoubleClick
        RaiseEvent UserDoubleClick(sender, e)
    End Sub
End Class
