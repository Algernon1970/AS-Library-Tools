Module Utils
    Public Function reverseArray(ByVal inp() As String) As String()
        Dim outArray(inp.Count) As String
        For i As Integer = 0 To inp.Count - 1
            outArray(inp.Count - i) = inp(i)
        Next
        Return outArray
    End Function
End Module
