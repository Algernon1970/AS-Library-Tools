<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class ADSearcherForm
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.UserListBox = New System.Windows.Forms.ListBox()
        Me.SuspendLayout()
        '
        'UserListBox
        '
        Me.UserListBox.Dock = System.Windows.Forms.DockStyle.Fill
        Me.UserListBox.FormattingEnabled = True
        Me.UserListBox.Location = New System.Drawing.Point(0, 0)
        Me.UserListBox.Name = "UserListBox"
        Me.UserListBox.Size = New System.Drawing.Size(284, 262)
        Me.UserListBox.TabIndex = 3
        '
        'ADSearcherForm
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(284, 262)
        Me.Controls.Add(Me.UserListBox)
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "ADSearcherForm"
        Me.ShowIcon = False
        Me.ShowInTaskbar = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = "ADSearcherForm"
        Me.TopMost = True
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents UserListBox As System.Windows.Forms.ListBox
End Class
