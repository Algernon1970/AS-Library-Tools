Public Class ListChooser
    Public Sub New(ByRef grps As List(Of String))

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        For Each grp As String In grps
            GroupChooserListBox.Items.Add(grp)
        Next
    End Sub

    Public Function getSelected() As List(Of String)
        Dim _selected As New List(Of String)
        For Each thing As String In GroupChooserListBox.SelectedItems
            _selected.Add(thing)
        Next
        Return _selected
    End Function

    Private Sub TextBox1_TextChanged(sender As Object, e As EventArgs) Handles TextBox1.TextChanged
        GroupChooserListBox.TopIndex = GroupChooserListBox.FindString(TextBox1.Text)
    End Sub
End Class