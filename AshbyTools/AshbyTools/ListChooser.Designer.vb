<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class ListChooser
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
        Me.GroupChooserListBox = New System.Windows.Forms.ListBox()
        Me.TableLayoutPanel1 = New System.Windows.Forms.TableLayoutPanel()
        Me.FlowLayoutPanel1 = New System.Windows.Forms.FlowLayoutPanel()
        Me.GroupOKButton = New System.Windows.Forms.Button()
        Me.GroupCancelButton = New System.Windows.Forms.Button()
        Me.TextBox1 = New System.Windows.Forms.TextBox()
        Me.TableLayoutPanel1.SuspendLayout()
        Me.FlowLayoutPanel1.SuspendLayout()
        Me.SuspendLayout()
        '
        'GroupChooserListBox
        '
        Me.GroupChooserListBox.Dock = System.Windows.Forms.DockStyle.Fill
        Me.GroupChooserListBox.FormattingEnabled = True
        Me.GroupChooserListBox.Location = New System.Drawing.Point(3, 3)
        Me.GroupChooserListBox.Name = "GroupChooserListBox"
        Me.GroupChooserListBox.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended
        Me.GroupChooserListBox.Size = New System.Drawing.Size(278, 221)
        Me.GroupChooserListBox.Sorted = True
        Me.GroupChooserListBox.TabIndex = 0
        '
        'TableLayoutPanel1
        '
        Me.TableLayoutPanel1.ColumnCount = 1
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.TableLayoutPanel1.Controls.Add(Me.GroupChooserListBox, 0, 0)
        Me.TableLayoutPanel1.Controls.Add(Me.FlowLayoutPanel1, 0, 1)
        Me.TableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TableLayoutPanel1.Location = New System.Drawing.Point(0, 0)
        Me.TableLayoutPanel1.Name = "TableLayoutPanel1"
        Me.TableLayoutPanel1.RowCount = 2
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 35.0!))
        Me.TableLayoutPanel1.Size = New System.Drawing.Size(284, 262)
        Me.TableLayoutPanel1.TabIndex = 2
        '
        'FlowLayoutPanel1
        '
        Me.FlowLayoutPanel1.Controls.Add(Me.GroupOKButton)
        Me.FlowLayoutPanel1.Controls.Add(Me.GroupCancelButton)
        Me.FlowLayoutPanel1.Controls.Add(Me.TextBox1)
        Me.FlowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.FlowLayoutPanel1.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft
        Me.FlowLayoutPanel1.Location = New System.Drawing.Point(3, 230)
        Me.FlowLayoutPanel1.Name = "FlowLayoutPanel1"
        Me.FlowLayoutPanel1.Size = New System.Drawing.Size(278, 29)
        Me.FlowLayoutPanel1.TabIndex = 1
        '
        'GroupOKButton
        '
        Me.GroupOKButton.DialogResult = System.Windows.Forms.DialogResult.OK
        Me.GroupOKButton.Location = New System.Drawing.Point(200, 3)
        Me.GroupOKButton.Name = "GroupOKButton"
        Me.GroupOKButton.Size = New System.Drawing.Size(75, 23)
        Me.GroupOKButton.TabIndex = 0
        Me.GroupOKButton.Text = "OK"
        Me.GroupOKButton.UseVisualStyleBackColor = True
        '
        'GroupCancelButton
        '
        Me.GroupCancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.GroupCancelButton.Location = New System.Drawing.Point(119, 3)
        Me.GroupCancelButton.Name = "GroupCancelButton"
        Me.GroupCancelButton.Size = New System.Drawing.Size(75, 23)
        Me.GroupCancelButton.TabIndex = 1
        Me.GroupCancelButton.Text = "Cancel"
        Me.GroupCancelButton.UseVisualStyleBackColor = True
        '
        'TextBox1
        '
        Me.TextBox1.Location = New System.Drawing.Point(9, 3)
        Me.TextBox1.Name = "TextBox1"
        Me.TextBox1.Size = New System.Drawing.Size(104, 20)
        Me.TextBox1.TabIndex = 2
        '
        'ListChooser
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(284, 262)
        Me.Controls.Add(Me.TableLayoutPanel1)
        Me.Name = "ListChooser"
        Me.Text = "ListChooser"
        Me.TableLayoutPanel1.ResumeLayout(False)
        Me.FlowLayoutPanel1.ResumeLayout(False)
        Me.FlowLayoutPanel1.PerformLayout()
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents GroupChooserListBox As System.Windows.Forms.ListBox
    Friend WithEvents TableLayoutPanel1 As System.Windows.Forms.TableLayoutPanel
    Friend WithEvents FlowLayoutPanel1 As System.Windows.Forms.FlowLayoutPanel
    Friend WithEvents GroupOKButton As System.Windows.Forms.Button
    Friend WithEvents GroupCancelButton As System.Windows.Forms.Button
    Friend WithEvents TextBox1 As System.Windows.Forms.TextBox
End Class
