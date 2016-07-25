Imports System
Imports System.IO
Imports System.Collections

Public Module DictionarySerializer
    Public Sub saveDict(ByVal fname As String, ByVal ext As String, ByRef dict As Dictionary(Of String, String))
        Dim fs As IO.FileStream = New IO.FileStream(String.Format("{0}{1}", fname, ext), IO.FileMode.OpenOrCreate)
        Dim bf As New Runtime.Serialization.Formatters.Binary.BinaryFormatter()
        bf.Serialize(fs, dict)
        fs.Close()
    End Sub

    Public Function loadDict(ByVal fname As String, ByVal ext As String) As Dictionary(Of String, String)
        Dim fsRead As New IO.FileStream(String.Format("{0}{1}", fname, ext), IO.FileMode.Open)
        Dim bf As New Runtime.Serialization.Formatters.Binary.BinaryFormatter()
        Dim theDict As Dictionary(Of String, String) = bf.Deserialize(fsRead)
        fsRead.Close()
        Return theDict
    End Function

    Public Function templateList(ByVal location As String, ByVal ext As String) As String()
        Dim theList As String() = Directory.GetFiles(location, "*" & ext)
        For x As Integer = 0 To theList.Count - 1
            theList(x) = theList(x).Replace(location, "")
            theList(x) = theList(x).Replace(ext, "")
        Next
        Return theList
    End Function
End Module
