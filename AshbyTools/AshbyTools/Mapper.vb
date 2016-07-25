Imports System.Runtime.InteropServices

Public Module Mapper

    Public Declare Auto Function WNetAddConnection2 Lib "mpr.dll" Alias "WNetAddConnection2" (ByRef lpNetResource As NETRESOURCE,
       <MarshalAs(UnmanagedType.LPTStr)> ByVal lpPassword As String,
       <MarshalAs(UnmanagedType.LPTStr)> ByVal lpUserName As String,
       ByVal dwFlags As Integer) As Integer

    Public Declare Auto Function WNetCancelConnection2 Lib "mpr.dll" Alias "WNetCancelConnection2" _
      (<MarshalAs(UnmanagedType.LPTStr)> ByVal lpName As String,
       ByVal dwFlags As Integer,
       ByVal fForce As Integer) _
       As Integer

    <StructLayout(LayoutKind.Sequential)> Public Structure NETRESOURCE
        Public dwScope As Integer
        Public dwType As Integer
        Public dwDisplayType As Integer
        Public dwUsage As Integer
        <MarshalAs(UnmanagedType.LPTStr)> Public lpLocalName As String
        <MarshalAs(UnmanagedType.LPTStr)> Public lpRemoteName As String
        <MarshalAs(UnmanagedType.LPTStr)> Public lpComment As String
        <MarshalAs(UnmanagedType.LPTStr)> Public lpProvider As String
    End Structure

    Public Function mapdrive(ByVal drive As String, ByVal path As String) As Boolean
        Dim nr As New NETRESOURCE
        nr.lpRemoteName = path
        nr.lpLocalName = drive
        nr.lpProvider = Nothing
        nr.dwType = &H1
        Dim res As Integer = Mapper.WNetAddConnection2(nr, Nothing, Nothing, 0)
        If res = 0 Then
            Return True
        Else
            Return False
        End If
    End Function

    Public Sub unmapdrive(ByVal drive As String)
        Mapper.WNetCancelConnection2(drive, 0, 1)
    End Sub

End Module
