Module Module1

    Sub Main()
        Dim mk = New Mikrotik("176.9.185.245", 8728)
        If Not mk.Login("admin", "gale@joon#65") Then
            Console.WriteLine("Cant log in")
            mk.Close()
            Console.ReadLine()
            Return
        End If

        mk.Send("/system/resource/getall", True)
        For Each row In mk.Read()
            Console.WriteLine(row)
        Next
        Console.ReadLine()
    End Sub

End Module
