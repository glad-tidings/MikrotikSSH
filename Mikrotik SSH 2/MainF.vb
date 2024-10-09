Public Class MainF

    Private Sub MainF_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        If Command() <> "" Then

            Dim mikrotik As New MK_ROS("176.9.185.245", 8728)
            Dim TMP As String = ""
            If Not mikrotik.Login("admin", "gale@joon#65") Then
                mikrotik.Close()
                Return
            End If

            mikrotik.Send("/ppp/active/print")
            mikrotik.Send("?name=" & Command())
            mikrotik.Send("=.proplist=.id", True)

            For Each h As String In mikrotik.Read()
                'Text1.Text &= h & vbCrLf
                If InStr(h, "e=.id=") <> 0 Then
                    TMP = Microsoft.VisualBasic.Right(h, Len(h) - InStr(h, "e=.id="))
                End If
            Next

            mikrotik.Send("/ppp/active/remove")
            mikrotik.Send(TMP, True)

            For Each h As String In mikrotik.Read()
                'Text1.Text &= h
                'MsgBox(h)
            Next

            mikrotik.Close()
            Application.Exit()

        End If
    End Sub

    Private Sub ViewPic_Click(sender As System.Object, e As System.EventArgs) Handles ViewPic.Click
        Application.Exit()
    End Sub
End Class
