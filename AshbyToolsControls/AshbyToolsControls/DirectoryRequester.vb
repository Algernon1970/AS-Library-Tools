Imports System.Windows.Forms

Public Class DirectoryRequester
    Shared _path As String = ""

    Public Shared ReadOnly Property path()
        Get
            Return If(_path.StartsWith("\"), "\" & _path, _path)
        End Get
    End Property

    Private Sub OK_Button_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
        Me.DialogResult = System.Windows.Forms.DialogResult.OK
        Me.Close()
    End Sub

    Private Sub Cancel_Button_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
        Me.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.Close()
    End Sub

    Private Sub DirectoryRequester_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Dim tln As TreeNode = New TreeNode("Computer")
        Dim drives As ObjectModel.ReadOnlyCollection(Of IO.DriveInfo) = My.Computer.FileSystem.Drives
        For Each drive As System.IO.DriveInfo In drives
            tln.Nodes.Add(drive.Name)
        Next

        ExplorerView.Nodes.Add(tln)

    End Sub

    Private Sub ExplorerView_BeforeExpand(sender As Object, e As TreeViewCancelEventArgs) Handles ExplorerView.BeforeExpand
        Dim path As String
        For Each node As TreeNode In e.Node.Nodes
            path = node.FullPath.Replace("Computer\", "")
            Try
                Dim folders() As String = IO.Directory.GetDirectories(path)
                For Each folder As String In folders
                    node.Nodes.Add(System.IO.Path.GetFileName(folder))
                Next
            Catch ex As Exception

            End Try

        Next
        DirectoryBox.Text = e.Node.Text.Replace("\\", "\")
        _path = DirectoryBox.Text
    End Sub

    Private Sub ExplorerView_NodeMouseClick(sender As Object, e As TreeNodeMouseClickEventArgs) Handles ExplorerView.NodeMouseClick
        Dim path As String = e.Node.FullPath.Replace("Computer\", "")
        DirectoryBox.Text = path.Replace("\\", "\")
        _path = DirectoryBox.Text
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Dim unc As String = DirectoryBox.Text
        ExplorerView.Nodes.Clear()
        Dim tln As TreeNode = New TreeNode(unc)
        Try
            Dim folders() As String = IO.Directory.GetDirectories(unc)
            For Each folder As String In folders
                tln.Nodes.Add(System.IO.Path.GetFileName(folder))
            Next
        Catch ex As Exception

        End Try

        ExplorerView.Nodes.Add(tln)
    End Sub

    Private Sub NewFolderButton_Click(sender As Object, e As EventArgs) Handles NewFolderButton.Click
        Dim path As String = If(DirectoryBox.Text.StartsWith("\"), "\" & DirectoryBox.Text, DirectoryBox.Text)
        Dim fn As String = InputBox("Folder Name", "Folder Name", "New Folder")
        IO.Directory.CreateDirectory(path & "\" & fn)
        Dim myNode As TreeNode = ExplorerView.SelectedNode
        myNode.Nodes.Add(fn)
    End Sub

End Class
