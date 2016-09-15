Imports System.IO

Public Module DictionaryTools
    Public Sub saveDictionary(ByVal filename As String, ByRef dictionary As Dictionary(Of String, String))
        Dim fs As New FileStream(filename, FileMode.OpenOrCreate)
        Dim bf As New Runtime.Serialization.Formatters.Binary.BinaryFormatter()
        bf.Serialize(fs, dictionary)
        fs.Close()
    End Sub

    Public Function loadDictionary(ByVal filename As String) As Dictionary(Of String, String)
        Dim fs As New FileStream(filename, FileMode.Open)
        Dim bf As New Runtime.Serialization.Formatters.Binary.BinaryFormatter()
        Dim theDictionary As Dictionary(Of String, String) = bf.Deserialize(fs)
        fs.Close()
        Return theDictionary
    End Function
End Module
