<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class ADBrowser
    Inherits System.Windows.Forms.UserControl

    'UserControl overrides dispose to clean up the component list.
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
        Me.ContainerLabel = New System.Windows.Forms.Label()
        Me.UserGroupBox = New System.Windows.Forms.GroupBox()
        Me.UserListBox = New System.Windows.Forms.ListBox()
        Me.ADGroupBox = New System.Windows.Forms.GroupBox()
        Me.AdTreeView1 = New AshbyToolsControls.ADTreeView()
        Me.TableLayoutPanel1 = New System.Windows.Forms.TableLayoutPanel()
        Me.FlowLayoutPanel1 = New System.Windows.Forms.FlowLayoutPanel()
        Me.FindUserBox = New System.Windows.Forms.TextBox()
        Me.FindButton = New System.Windows.Forms.Button()
        Me.UserGroupBox.SuspendLayout()
        Me.ADGroupBox.SuspendLayout()
        Me.TableLayoutPanel1.SuspendLayout()
        Me.FlowLayoutPanel1.SuspendLayout()
        Me.SuspendLayout()
        '
        'ContainerLabel
        '
        Me.ContainerLabel.AutoSize = True
        Me.ContainerLabel.Location = New System.Drawing.Point(3, 410)
        Me.ContainerLabel.Name = "ContainerLabel"
        Me.ContainerLabel.Size = New System.Drawing.Size(0, 13)
        Me.ContainerLabel.TabIndex = 2
        Me.ContainerLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'UserGroupBox
        '
        Me.UserGroupBox.Controls.Add(Me.UserListBox)
        Me.UserGroupBox.Dock = System.Windows.Forms.DockStyle.Fill
        Me.UserGroupBox.Location = New System.Drawing.Point(235, 3)
        Me.UserGroupBox.Name = "UserGroupBox"
        Me.UserGroupBox.Size = New System.Drawing.Size(227, 404)
        Me.UserGroupBox.TabIndex = 1
        Me.UserGroupBox.TabStop = False
        Me.UserGroupBox.Text = "Users"
        '
        'UserListBox
        '
        Me.UserListBox.Dock = System.Windows.Forms.DockStyle.Fill
        Me.UserListBox.FormattingEnabled = True
        Me.UserListBox.Location = New System.Drawing.Point(3, 16)
        Me.UserListBox.Name = "UserListBox"
        Me.UserListBox.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended
        Me.UserListBox.Size = New System.Drawing.Size(221, 385)
        Me.UserListBox.TabIndex = 0
        '
        'ADGroupBox
        '
        Me.ADGroupBox.Controls.Add(Me.AdTreeView1)
        Me.ADGroupBox.Dock = System.Windows.Forms.DockStyle.Fill
        Me.ADGroupBox.Location = New System.Drawing.Point(3, 3)
        Me.ADGroupBox.Name = "ADGroupBox"
        Me.ADGroupBox.Size = New System.Drawing.Size(226, 404)
        Me.ADGroupBox.TabIndex = 0
        Me.ADGroupBox.TabStop = False
        Me.ADGroupBox.Text = "Active Directory"
        '
        'AdTreeView1
        '
        Me.AdTreeView1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.AdTreeView1.Location = New System.Drawing.Point(3, 16)
        Me.AdTreeView1.Name = "AdTreeView1"
        Me.AdTreeView1.Size = New System.Drawing.Size(220, 385)
        Me.AdTreeView1.TabIndex = 0
        '
        'TableLayoutPanel1
        '
        Me.TableLayoutPanel1.ColumnCount = 2
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.TableLayoutPanel1.Controls.Add(Me.ADGroupBox, 0, 0)
        Me.TableLayoutPanel1.Controls.Add(Me.UserGroupBox, 1, 0)
        Me.TableLayoutPanel1.Controls.Add(Me.ContainerLabel, 0, 1)
        Me.TableLayoutPanel1.Controls.Add(Me.FlowLayoutPanel1, 1, 1)
        Me.TableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TableLayoutPanel1.Location = New System.Drawing.Point(0, 0)
        Me.TableLayoutPanel1.Name = "TableLayoutPanel1"
        Me.TableLayoutPanel1.RowCount = 2
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 92.56757!))
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 7.432433!))
        Me.TableLayoutPanel1.Size = New System.Drawing.Size(465, 444)
        Me.TableLayoutPanel1.TabIndex = 0
        '
        'FlowLayoutPanel1
        '
        Me.FlowLayoutPanel1.Controls.Add(Me.FindUserBox)
        Me.FlowLayoutPanel1.Controls.Add(Me.FindButton)
        Me.FlowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.FlowLayoutPanel1.Location = New System.Drawing.Point(235, 413)
        Me.FlowLayoutPanel1.Name = "FlowLayoutPanel1"
        Me.FlowLayoutPanel1.Size = New System.Drawing.Size(227, 28)
        Me.FlowLayoutPanel1.TabIndex = 3
        '
        'FindUserBox
        '
        Me.FindUserBox.Location = New System.Drawing.Point(3, 3)
        Me.FindUserBox.Name = "FindUserBox"
        Me.FindUserBox.Size = New System.Drawing.Size(136, 20)
        Me.FindUserBox.TabIndex = 0
        '
        'FindButton
        '
        Me.FindButton.Location = New System.Drawing.Point(145, 3)
        Me.FindButton.Name = "FindButton"
        Me.FindButton.RightToLeft = System.Windows.Forms.RightToLeft.Yes
        Me.FindButton.Size = New System.Drawing.Size(75, 23)
        Me.FindButton.TabIndex = 1
        Me.FindButton.Text = "Find"
        Me.FindButton.UseVisualStyleBackColor = True
        '
        'ADBrowser
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me.TableLayoutPanel1)
        Me.Name = "ADBrowser"
        Me.Size = New System.Drawing.Size(465, 444)
        Me.UserGroupBox.ResumeLayout(False)
        Me.ADGroupBox.ResumeLayout(False)
        Me.TableLayoutPanel1.ResumeLayout(False)
        Me.TableLayoutPanel1.PerformLayout()
        Me.FlowLayoutPanel1.ResumeLayout(False)
        Me.FlowLayoutPanel1.PerformLayout()
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents ContainerLabel As System.Windows.Forms.Label
    Friend WithEvents UserGroupBox As System.Windows.Forms.GroupBox
    Friend WithEvents UserListBox As System.Windows.Forms.ListBox
    Friend WithEvents ADGroupBox As System.Windows.Forms.GroupBox
    Friend WithEvents AdTreeView1 As ADTreeView
    Friend WithEvents TableLayoutPanel1 As System.Windows.Forms.TableLayoutPanel
    Friend WithEvents FlowLayoutPanel1 As System.Windows.Forms.FlowLayoutPanel
    Friend WithEvents FindUserBox As System.Windows.Forms.TextBox
    Friend WithEvents FindButton As System.Windows.Forms.Button
End Class
