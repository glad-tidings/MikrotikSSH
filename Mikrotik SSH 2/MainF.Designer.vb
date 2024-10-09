<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class MainF
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(MainF))
        Me.ViewPic = New System.Windows.Forms.PictureBox()
        CType(Me.ViewPic, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'ViewPic
        '
        Me.ViewPic.Dock = System.Windows.Forms.DockStyle.Fill
        Me.ViewPic.Image = CType(resources.GetObject("ViewPic.Image"), System.Drawing.Image)
        Me.ViewPic.Location = New System.Drawing.Point(0, 0)
        Me.ViewPic.Name = "ViewPic"
        Me.ViewPic.Size = New System.Drawing.Size(340, 340)
        Me.ViewPic.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        Me.ViewPic.TabIndex = 1
        Me.ViewPic.TabStop = False
        '
        'MainF
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(340, 340)
        Me.ControlBox = False
        Me.Controls.Add(Me.ViewPic)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "MainF"
        Me.ShowIcon = False
        Me.ShowInTaskbar = False
        Me.Text = "Form1"
        CType(Me.ViewPic, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents ViewPic As System.Windows.Forms.PictureBox

End Class
