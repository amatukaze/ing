Public Module Helpers
    Public Function IsNotNull(obj As Object) As Boolean
        Return obj IsNot Nothing
    End Function

    Public Function [Not](value As Boolean) As Boolean
        Return Not value
    End Function
End Module
