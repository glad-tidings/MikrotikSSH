Public Class Mikrotik
    Dim tcpStream As IO.Stream
    Dim tcpCon As New Net.Sockets.TcpClient

    Public Sub New(ByVal ipOrDns As String, Optional ByVal port As Integer = -1)
        Dim ips = Net.Dns.GetHostEntry(ipOrDns)

        tcpCon.Connect(ips.AddressList(0), If(port = -1, 8728, port))
        tcpStream = tcpCon.GetStream()
    End Sub

    Public Sub New(ByVal endP As System.Net.IPEndPoint)
        tcpCon.Connect(endP)
        tcpStream = tcpCon.GetStream()
    End Sub

    Public Sub Close()
        tcpStream.Close()
        tcpCon.Close()
    End Sub

    Public Function Login(ByVal user As String, ByVal pass As String) As Boolean
        Send("/login", True)
        Dim hash = Read()(0).Split(New String() {"ret="}, StringSplitOptions.None)(1)
        Send("/login")
        Send("=name=" + user)
        Send("=response=00" + EncodePassword(pass, hash), True)
        Dim res = Read()
        If (res(0) = "!done") Then Return True Else Return False
    End Function

    Function EncodePassword(ByVal pass As String, ByVal challange As String) As String
        Dim hash_byte(challange.Length / 2 - 1) As Byte
        For i = 0 To challange.Length - 2 Step 2
            hash_byte(i / 2) = Byte.Parse(challange.Substring(i, 2), Globalization.NumberStyles.HexNumber)
        Next
        Dim response(pass.Length + hash_byte.Length) As Byte
        response(0) = 0
        Text.Encoding.ASCII.GetBytes(pass.ToCharArray()).CopyTo(response, 1)
        hash_byte.CopyTo(response, 1 + pass.Length)


        Dim md5 = New System.Security.Cryptography.MD5CryptoServiceProvider()

        Dim hash = md5.ComputeHash(response)

        Dim hashStr As New Text.StringBuilder()
        For Each h In hash
            hashStr.Append(h.ToString("x2"))
        Next
        Return hashStr.ToString()
    End Function

    Public Sub Send(ByVal command As String, Optional ByVal EndSentence As Boolean = False)
        Dim bytes = System.Text.Encoding.ASCII.GetBytes(command.ToCharArray())
        Dim size = EncodeLength(bytes.Length)

        tcpStream.Write(size, 0, size.Length)
        tcpStream.Write(bytes, 0, bytes.Length)
        If EndSentence Then tcpStream.WriteByte(0)
    End Sub

    Public Function Read() As List(Of String)
        Dim output As New List(Of String)
        Dim o = ""
        Dim tmp(4) As Byte
        Dim count As Long

        While True
            tmp(3) = tcpStream.ReadByte()
            Select Case tmp(3)
                Case 0
                    output.Add(o)
                    If o.Substring(0, 5) = "!done" Then
                        Exit While
                    Else
                        o = ""
                        Continue While
                    End If
                Case Is < &H80
                    count = tmp(3)
                Case Is < &HC0
                    count = BitConverter.ToInt32(New Byte() {tcpStream.ReadByte(), tmp(3), 0, 0}, 0) ^ &H8000
                Case Is < &HE0
                    tmp(2) = tcpStream.ReadByte()
                    count = BitConverter.ToInt32(New Byte() {tcpStream.ReadByte(), tmp(2), tmp(3), 0}, 0) ^ &HC00000
                Case Is < &HF0
                    tmp(2) = tcpStream.ReadByte()
                    tmp(1) = tcpStream.ReadByte()
                    count = BitConverter.ToInt32(New Byte() {tcpStream.ReadByte(), tmp(1), tmp(2), tmp(3)}, 0) ^ &HE0000000
                Case &HF0
                    tmp(3) = tcpStream.ReadByte()
                    tmp(2) = tcpStream.ReadByte()
                    tmp(1) = tcpStream.ReadByte()
                    tmp(0) = tcpStream.ReadByte()
                    count = BitConverter.ToInt32(tmp, 0)
                Case Else
                    Exit While   'err
            End Select

            For i = 0 To count - 1
                o += ChrW(tcpStream.ReadByte())
            Next
        End While
        Return output
    End Function

    Function EncodeLength(ByVal l As Integer) As Byte()
        If l < &H80 Then
            Dim tmp = BitConverter.GetBytes(l)
            Return New Byte() {tmp(0)}
        ElseIf l < &H4000 Then
            Dim tmp = BitConverter.GetBytes(l Or &H8000)
            Return New Byte() {tmp(1), tmp(0)}
        ElseIf l < &H200000 Then
            Dim tmp = BitConverter.GetBytes(l Or &HC00000)
            Return New Byte() {tmp(2), tmp(1), tmp(0)}
        ElseIf l < &H10000000 Then
            Dim tmp = BitConverter.GetBytes(l Or &HE0000000)
            Return New Byte() {tmp(3), tmp(2), tmp(1), tmp(0)}
        Else
            Dim tmp = BitConverter.GetBytes(l)
            Return New Byte() {&HF0, tmp(3), tmp(2), tmp(1), tmp(0)}
        End If
    End Function
End Class