Imports System.ComponentModel
Imports System.Drawing
Imports System.Windows.Forms
Imports AshbyTools

Public Class ASSlider
    Implements INotifyPropertyChanged
    Dim rpx As Integer
    Dim px As Integer
    Dim picmid As Integer
    Dim pMax As Integer
    Dim pMin As Integer
    Public Event PropertyChanged As PropertyChangedEventHandler _
        Implements INotifyPropertyChanged.PropertyChanged
    Public Sub New()
        InitializeComponent()
        picmid = PointerPic.Width / 2
    End Sub

    Public Property pointerX() As Integer
        Get
            ' Return Me.Width
            Return AshbyTools.Utils.interpolate(pMin, pMax, 0, Me.Width, px)
        End Get
        Set(value As Integer)
            px = value
            PointerPic.Location = New Point(px, PointerPic.Location.Y)

            PointerPic.Refresh()
            NotifyPropertyChanged("pointerX")
        End Set
    End Property

    Public Property max() As Integer
        Get
            Return pMax
        End Get
        Set(value As Integer)
            pMax = value
        End Set
    End Property

    Public Property min() As Integer
        Get
            Return pMin
        End Get
        Set(value As Integer)
            pMin = value
        End Set
    End Property

    Private Sub NotifyPropertyChanged(ByVal info As String)
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(info))
    End Sub

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
        If e.X > -1 And e.X < Me.Width + 1 Then
            pointerX = e.X
        End If

    End Sub

    Private Sub ASSlider_MouseMove(sender As Object, e As MouseEventArgs) Handles MyBase.MouseMove
        If e.Button = MouseButtons.Left Then
            If e.X > -1 And e.X < Me.Width + 1 Then
                pointerX = e.X
            End If
        End If
    End Sub
End Class