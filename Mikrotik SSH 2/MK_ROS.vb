Imports System.Net.Sockets
Imports System.IO
Imports System.Text

Class MK_ROS
    Private connection As Stream
    Private con As TcpClient

    Public Sub New(ByVal ip As String, ByVal port As String)
        con = New TcpClient()
        con.Connect(ip, port)
        connection = DirectCast(con.GetStream(), Stream)
    End Sub
    Public Sub Close()
        connection.Close()
        con.Close()
    End Sub
    Public Function Login(ByVal username As String, ByVal password As String) As Boolean
        Send("/login", True)
        Dim hash As String = Read()(0).Split(New String() {"ret="}, StringSplitOptions.None)(1)
        Send("/login")
        Send("=name=" & username)
        Send("=response=00" & EncodePassword(password, hash), True)
        If Read()(0) = "!done" Then
            Return True
        Else
            Return False
        End If
    End Function
    Public Sub Send(ByVal co As String)
        Dim bajty As Byte() = Encoding.ASCII.GetBytes(co.ToCharArray())
        Dim velikost As Byte() = EncodeLength(bajty.Length)

        connection.Write(velikost, 0, velikost.Length)
        connection.Write(bajty, 0, bajty.Length)
    End Sub
    Public Sub Send(ByVal co As String, ByVal endsentence As Boolean)
        Dim bajty As Byte() = Encoding.ASCII.GetBytes(co.ToCharArray())
        Dim velikost As Byte() = EncodeLength(bajty.Length)
        connection.Write(velikost, 0, velikost.Length)
        connection.Write(bajty, 0, bajty.Length)
        connection.WriteByte(0)
    End Sub
    Public Function Read() As List(Of String)
        Dim output As New List(Of String)()
        Dim o As String = ""
        Dim tmp As Byte() = New Byte(3) {}
        Dim count As Long
        While True
            'MsgBox(CByte(connection.ReadByte()))
            tmp(3) = CByte(connection.ReadByte())
            'if(tmp[3] == 220) tmp[3] = (byte)connection.ReadByte(); it sometimes happend to me that
            'mikrotik send 220 as some kind of "bonus" between words, this fixed things, not sure about it though
            If tmp(3) = 0 Then
                output.Add(o)
                If o.Substring(0, 5) = "!done" Then
                    Exit While
                Else
                    o = ""
                    Continue While
                End If
            Else
                If tmp(3) < &H80 Then
                    count = tmp(3)
                Else
                    If tmp(3) < &HC0 Then
                        Dim tmpi As Integer = BitConverter.ToInt32(New Byte() {CByte(connection.ReadByte()), tmp(3), 0, 0}, 0)
                        count = tmpi Xor &H8000
                    Else
                        If tmp(3) < &HE0 Then
                            tmp(2) = CByte(connection.ReadByte())
                            Dim tmpi As Integer = BitConverter.ToInt32(New Byte() {CByte(connection.ReadByte()), tmp(2), tmp(3), 0}, 0)
                            count = tmpi Xor &HC00000
                        Else
                            If tmp(3) < &HF0 Then
                                tmp(2) = CByte(connection.ReadByte())
                                tmp(1) = CByte(connection.ReadByte())
                                Dim tmpi As Integer = BitConverter.ToInt32(New Byte() {CByte(connection.ReadByte()), tmp(1), tmp(2), tmp(3)}, 0)
                                count = tmpi Xor &HE0000000
                            Else
                                If tmp(3) = &HF0 Then
                                    tmp(3) = CByte(connection.ReadByte())
                                    tmp(2) = CByte(connection.ReadByte())
                                    tmp(1) = CByte(connection.ReadByte())
                                    tmp(0) = CByte(connection.ReadByte())
                                    count = BitConverter.ToInt32(tmp, 0)
                                Else
                                    'Error in packet reception, unknown length
                                    Exit While
                                End If
                            End If
                        End If
                    End If
                End If
            End If

            For i As Integer = 0 To count - 1
                o += ChrW(connection.ReadByte())
            Next
        End While
        Return output
    End Function
    Private Function EncodeLength(ByVal delka As Integer) As Byte()
        If delka < &H80 Then
            Dim tmp As Byte() = BitConverter.GetBytes(delka)
            Return New Byte(0) {tmp(0)}
        End If
        If delka < &H4000 Then
            Dim tmp As Byte() = BitConverter.GetBytes(delka Or &H8000)
            Return New Byte(1) {tmp(1), tmp(0)}
        End If
        If delka < &H200000 Then
            Dim tmp As Byte() = BitConverter.GetBytes(delka Or &HC00000)
            Return New Byte(2) {tmp(2), tmp(1), tmp(0)}
        End If
        If delka < &H10000000 Then
            Dim tmp As Byte() = BitConverter.GetBytes(delka Or &HE0000000)
            Return New Byte(3) {tmp(3), tmp(2), tmp(1), tmp(0)}
        Else
            Dim tmp As Byte() = BitConverter.GetBytes(delka)
            Return New Byte(4) {&HF0, tmp(3), tmp(2), tmp(1), tmp(0)}
        End If
    End Function

    Public Function EncodePassword(ByVal Password As String, ByVal hash As String) As String
        Dim hash_byte As Byte() = New Byte(hash.Length / 2 - 1) {}
        For i As Integer = 0 To hash.Length - 2 Step 2
            hash_byte(i / 2) = [Byte].Parse(hash.Substring(i, 2), System.Globalization.NumberStyles.HexNumber)
        Next
        Dim heslo As Byte() = New Byte(1 + Password.Length + (hash_byte.Length - 1)) {}
        heslo(0) = 0
        Encoding.ASCII.GetBytes(Password.ToCharArray()).CopyTo(heslo, 1)
        hash_byte.CopyTo(heslo, 1 + Password.Length)

        Dim hotovo As Byte()
        Dim md5 As System.Security.Cryptography.MD5

        md5 = New System.Security.Cryptography.MD5CryptoServiceProvider()

        hotovo = md5.ComputeHash(heslo)

        'Convert encoded bytes back to a 'readable' string
        Dim navrat As String = ""
        For Each h As Byte In hotovo
            navrat += h.ToString("x2")
        Next
        Return navrat
    End Function
End Class