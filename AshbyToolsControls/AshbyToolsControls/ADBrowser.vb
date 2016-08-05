Imports AshbyTools
Imports System.DirectoryServices.AccountManagement
Imports System.ComponentModel
Imports System.Windows.Forms

Public Class ADBrowser
    Public Event NodeSelect(ByVal path As String)
    Public Event UserSelection(ByVal ulist As List(Of UserPrincipalex))
    Public Event UserDoubleClick(ByVal sender As Object, ByVal e As EventArgs)

    Public Sub loadAD(ByVal domain As String, ByVal ou As String)
        AdTreeView1.loadAD(getConnection(domain, ou))
    End Sub

    Private Sub AdTreeView1_NodeMouseClick(sender As Object, e As System.Windows.Forms.TreeNodeMouseClickEventArgs) Handles AdTreeView1.NodeMouseClick
        Dim path As String = AdTreeView1.GetNodeAt(e.X, e.Y).FullPath.Replace("\", ",")

        DisplayUserList(path)
    End Sub

    Private Sub DisplayUserList(path As String)
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

    Private Sub FindButton_Click(sender As Object, e As EventArgs) Handles FindButton.Click
        FindButton.Text = "WAIT"
        FindButton.Enabled = False
        FindButton.Refresh()

        Dim matchedUsers As New List(Of UserPrincipalex)

        Dim osearchString As String = FindUserBox.Text
        Dim searchString As String = osearchString.Replace("*", "")

        Dim allUsers As List(Of UserPrincipalex) = getAllUsers(userCTX)
        For Each user As UserPrincipalex In allUsers
            If Not IsNothing(user.SamAccountName) Then
                If osearchString.StartsWith("*") And osearchString.EndsWith("*") Then
                    If user.SamAccountName.ToUpper.Contains(searchString.ToUpper) Then
                        matchedUsers.Add(user)
                    End If
                ElseIf osearchString.StartsWith("*") Then
                    If user.SamAccountName.ToUpper.EndsWith(searchString.ToUpper) Then
                        matchedUsers.Add(user)
                    End If
                ElseIf osearchString.EndsWith("*") Then
                    If user.SamAccountName.ToUpper.StartsWith(searchString.ToUpper) Then
                        matchedUsers.Add(user)
                    End If
                Else
                    If user.SamAccountName.ToUpper.Equals(searchString.ToUpper) Then
                        matchedUsers.Add(user)
                    End If
                End If
            End If
        Next
        allUsers = Nothing
        Dim res As String = "Cancelled"
        If matchedUsers.Count > 1 Then
            Dim adsearch As New ADSearcherForm(matchedUsers)

            If adsearch.ShowDialog() = System.Windows.Forms.DialogResult.OK Then
                res = adsearch.getselected
            End If

            adsearch.Dispose()
            adsearch = Nothing
        Else
            res = matchedUsers(0).SamAccountName
        End If

        FindButton.Text = "Find"
        FindButton.Enabled = True
        FindButton.Refresh()
        findres(res)
    End Sub

    Private Sub findres(ByVal res As String)
        If res.Equals("Cancelled") Then Return

        Dim usr As UserPrincipal = ADTools.getUserPrincipalexbyUsername(userCTX, res)
        Dim bits As String() = usr.DistinguishedName.Split(",")
        Dim stib As String() = reverseArray(bits)

        For Each element As String In stib
            openNode(AdTreeView1.Nodes(0), element)
        Next
        Dim path As String = AdTreeView1.SelectedNode.FullPath.Replace("\", ",")
        DisplayUserList(path)
        UserListBox.SelectedIndex = UserListBox.FindString(res)
    End Sub

    Private Sub openNode(ByVal mynode As TreeNode, ByVal element As String)
        Dim tNode As TreeNode
        For Each tNode In mynode.Nodes
            If tNode.Text = element Then
                AdTreeView1.SelectedNode = tNode
                AdTreeView1.SelectedNode.Expand()
                Exit For

            End If
            openNode(tNode, element)
        Next
    End Sub
End Class
