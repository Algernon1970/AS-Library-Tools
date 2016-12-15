Imports System.Drawing
Imports System.Windows.Forms

Public Class ASSlider
    Dim px As Integer
    Dim picmid As Integer
    Public Sub New()
        InitializeComponent()
        picmid = PointerPic.Width / 2
    End Sub

    Public Property pointerX() As Integer
        Get
            Return px
        End Get
        Set(value As Integer)
            px = value
            PointerPic.Location = New Point(px, PointerPic.Location.Y)
            PointerPic.Refresh()
        End Set
    End Property


    Protected Overrides Sub OnPaint(e As PaintEventArgs)

        MyBase.OnPaint(e)
        Dim g As Graphics = e.Graphics
        Dim width As Integer = Me.Width
        Dim height As Integer = Me.Height
        Dim spacing As Integer = Me.Width / 10
        For x = 0 To Me.Width Step spacing
            g.DrawLine(New Pen(Color.Black, 1), New Point(x, 0), New Point(x, height / 2))
        Next

    End Sub

    Private Sub ASSlider_MouseDown(sender As Object, e As MouseEventArgs) Handles MyBase.MouseDown
        pointerX = e.X
    End Sub
End Class