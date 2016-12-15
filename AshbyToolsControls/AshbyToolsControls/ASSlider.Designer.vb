<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class ASSlider
    Inherits System.Windows.Forms.UserControl

    'UserControl overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()>
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
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Me.PointerPic = New System.Windows.Forms.PictureBox()
        CType(Me.PointerPic, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'PointerPic
        '
        Me.PointerPic.Image = Global.AshbyToolsControls.My.Resources.Resources.greenball
        Me.PointerPic.Location = New System.Drawing.Point(71, 23)
        Me.PointerPic.Name = "PointerPic"
        Me.PointerPic.Size = New System.Drawing.Size(10, 17)
        Me.PointerPic.TabIndex = 0
        Me.PointerPic.TabStop = False
        '
        'ASSlider
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me.PointerPic)
        Me.Name = "ASSlider"
        Me.Size = New System.Drawing.Size(150, 43)
        CType(Me.PointerPic, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents PointerPic As System.Windows.Forms.PictureBox
End Class
