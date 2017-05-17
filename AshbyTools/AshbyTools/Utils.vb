Imports System.IO
Imports System.Security

Public Module Utils
    Public Function reverseArray(ByVal inp() As String) As String()
        Dim outArray(inp.Count) As String
        For i As Integer = 0 To inp.Count - 1
            outArray(inp.Count - i) = inp(i)
        Next
        Return outArray
    End Function

    Public Sub deleteDir(ByVal path As String)
        Dim dirlist As String() = IO.Directory.GetDirectories(path)
        Dim filelist As String() = IO.Directory.GetFiles(path)
        For Each filename As String In filelist
            Dim oFileinfo As New FileInfo(filename)
            If (oFileinfo.Attributes And FileAttributes.ReadOnly) > 0 Then
                oFileinfo.Attributes = oFileinfo.Attributes Xor FileAttributes.ReadOnly
            End If
            File.Delete(filename)
        Next
        For Each dir As String In dirlist
            deleteDir(dir)
        Next
        Dim oDirInfo As New DirectoryInfo(path)
        If oDirInfo.Attributes And FileAttributes.ReadOnly > 0 Then
            oDirInfo.Attributes = FileAttributes.Normal
        End If
        Directory.Delete(path)
    End Sub

    Public Function convertToSecureString(str As String) As SecureString
        Dim secureStr = New SecureString()
        If str.Length > 0 Then
            For Each c In str.ToCharArray()
                secureStr.AppendChar(c)
            Next
        End If
        Return secureStr
    End Function

End Module
