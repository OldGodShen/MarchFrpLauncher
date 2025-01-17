﻿Imports MarchFrpLauncher.UI
Imports System.IO
Imports System.Threading

Public Class LoginBox

    Public Assets As AssetModel

    Public Config As Config

    Dim loginResult As UserControl

    Public Event LoginSucceed()

    Public Username As String
    Public UserToken As String
    Public Webapi As String

    Private Sub SearchHeadImg(email As String)
        If File.Exists(Gravatar.FolderPath + "\" + email + ".png") Then
            Try
                Dim imgBitmap As New System.Drawing.Bitmap(Gravatar.FolderPath + "\" + email + ".png")
                Me.img_head.Source = UI.Image.BitmapToImageSource(imgBitmap)
            Catch
            End Try
        End If
    End Sub

    Public Sub _init_()
        Me.tb_Username.Text = Config.Username.Val
        Me.tb_Username.Text = Assets.Username
        Me.tb_Username.Foreground = Brushes.Gray
        Me.lb_pwdNotice.Foreground = Brushes.Gray
        Me.lb_pwdNotice.Content = Assets.Password
        Me.tempfrm.Show()
        Me.tempfrm.Visible = False
        If Not Config.Username.Val = "" Then
            Me.tb_Username.Text = Config.Username.Val
            Me.tb_Username.Foreground = Brushes.Black
            SearchHeadImg(Config.Username.Val)
            Me.tb_Password.Focus()
        Else
            Me.tb_Username.Focus()
        End If


        Me.Apisel.Items.Add("OGFrp官方Api")
        Me.Apisel.Items.Add("OpenFrp官方Api")
        Me.Apisel.Items.Add("木韩Frp官方Api")
        Me.Apisel.Items.Add("LightPowerFrp官方Api")
        Me.Apisel.Items.Add("自定义Api")
        If Config.Apiweb.Val = "api.ogfrp.cn" Then
            Me.Apisel.Text = "OGFrp官方Api"
            Me.Apiwebbox.Text = Config.Apiweb.Val
            Me.Apiwebbox.IsReadOnly = True
        ElseIf Config.Apiweb.Val = "api.openfrp.net" Then
            Me.Apisel.Text = "OpenFrp官方Api"
            Me.Apiwebbox.Text = Config.Apiweb.Val
            Me.Apiwebbox.IsReadOnly = True
        ElseIf Config.Apiweb.Val = "frp.mcbebbs.cn/api" Then
            Me.Apisel.Text = "木韩Frp官方Api"
            Me.Apiwebbox.Text = Config.Apiweb.Val
            Me.Apiwebbox.IsReadOnly = True
        ElseIf Config.Apiweb.Val = "frp.lightpower.top/api" Then
            Me.Apisel.Text = "LightPowerFrp官方Api"
            Me.Apiwebbox.Text = Config.Apiweb.Val
            Me.Apiwebbox.IsReadOnly = True
        Else
            Me.Apisel.Text = "自定义Api"
            Me.Apiwebbox.Text = Config.Apiweb.Val
            Me.Apiwebbox.IsReadOnly = False
        End If
    End Sub

    Dim tempfrm As New Forms.Form With {
        .WindowState = Forms.FormWindowState.Minimized,
        .ShowInTaskbar = False,
        .Left = Integer.MaxValue,
        .Top = Integer.MaxValue,
        .FormBorderStyle = Forms.FormBorderStyle.None,
        .Width = 0,
        .Height = 0
    }

    Dim Net As New Net

    ''' <summary>
    ''' 已获取的用户头像
    ''' </summary>
    Public UserImage As System.Drawing.Bitmap

    ''' <summary>
    ''' 当成功获取用户头像后触发
    ''' </summary>
    Public Event GotUserImage()

    ''' <summary>
    ''' 登录（返回token）
    ''' </summary>
    Private Sub login()
        Dim Apitoken As String
        If Config.Apiweb.Val = "api.openfrp.net" Then
            Apitoken = "&apitoken=LTW9yDKEfYJnzrqv"
        Else
            Apitoken = ""
        End If
        Try
            tempfrm.Invoke(
                Sub()
                    Me.UserToken = Net.GetAccessToken(Me.tb_Username.Text, Me.tb_Password.Password, Config.Apiweb.Val, Apitoken)
                    Me.Username = Me.tb_Username.Text
                    RaiseEvent LoginSucceed()
                    Me.Visibility = Visibility.Hidden
                End Sub)
        Catch ex As Exception
            tempfrm.Invoke(
                Sub()
                    Me.lb_info.Content = Assets.LoginFailed
                    Me.tb_Username.IsEnabled = True
                    Me.tb_Password.IsEnabled = True
                    Me.bt_login.IsEnabled = True
                End Sub)
        End Try
    End Sub

    Private Sub bt_login_Click() Handles bt_login.Click
        Me.Config.Username.Val = Me.tb_Username.Text
        Config.WriteConfig()
        If Me.tb_Username.Text = vbNullString Or Me.tb_Password.Password = vbNullString Then
            Me.lb_info.Content = Assets.LoginFailed
        Else
            With Me
                Me.tb_Username.IsEnabled = False
                Me.tb_Password.IsEnabled = False
                Me.bt_login.IsEnabled = False
            End With
            Me.lb_info.Content = Assets.Logining
            Dim LoginThread As New Thread(AddressOf login)
            LoginThread.Start()
        End If
    End Sub

    Private Sub tb_Password_KeyDown(sender As Object, e As KeyEventArgs) Handles tb_Password.KeyDown
        If e.Key = Key.Enter Then
            Me.bt_login_Click()
        End If
    End Sub

    Private Sub usrnme_foc() Handles tb_Username.GotFocus
        If Me.tb_Username.Text = Assets.Username Then
            Me.tb_Username.Foreground = Brushes.Black
            Me.tb_Username.Text = ""
        End If
    End Sub

    Private Sub usrnme_lfc() Handles tb_Username.LostFocus
        If Me.tb_Username.Text = "" Then
            Me.tb_Username.Foreground = Brushes.Gray
            Me.tb_Username.Text = Assets.Username
        End If
    End Sub

    Private Sub pswd_foc() Handles lb_pwdNotice.MouseDown, tb_Password.GotFocus
        Me.lb_pwdNotice.Visibility = Visibility.Hidden
        Me.tb_Password.Focus()
    End Sub

    Private Sub pswd_lfc() Handles tb_Password.LostFocus
        If Me.tb_Password.Password = vbNullString Then
            Me.lb_pwdNotice.Visibility = Visibility.Visible
        End If
    End Sub

    Private Sub pswd_msetr() Handles lb_pwdNotice.MouseMove
        If Me.tb_Password.IsEnabled Then
            Me.lb_pwdNotice.BorderBrush = Me.tb_Password.SelectionBrush
        End If
    End Sub

    Private Sub pswd_mslv() Handles lb_pwdNotice.MouseLeave
        Me.lb_pwdNotice.BorderBrush = Brushes.Transparent
    End Sub

    Private Sub Apisel_SelectionChanged_1(sender As Object, e As SelectionChangedEventArgs) Handles Apisel.SelectionChanged
        Select Case Me.Apisel.SelectedItem
            Case "OGFrp官方Api"
                Config.Apiweb.Val = "api.ogfrp.cn"
                Me.Apiwebbox.Text = Config.Apiweb.Val
                Me.Apiwebbox.IsReadOnly = True
            Case "OpenFrp官方Api"
                Config.Apiweb.Val = "api.openfrp.net"
                Me.Apiwebbox.Text = Config.Apiweb.Val
                Me.Apiwebbox.IsReadOnly = True
            Case "木韩Frp官方Api"
                Config.Apiweb.Val = "frp.mcbebbs.cn/api"
                Me.Apiwebbox.Text = Config.Apiweb.Val
                Me.Apiwebbox.IsReadOnly = True
            Case "LightPowerFrp官方Api"
                Config.Apiweb.Val = "frp.lightpower.top/api"
                Me.Apiwebbox.Text = Config.Apiweb.Val
                Me.Apiwebbox.IsReadOnly = True
            Case "自定义Api"
                Me.Apiwebbox.Text = ""
                Me.Apiwebbox.IsReadOnly = False
        End Select
        Config.WriteConfig()

    End Sub

    Private Sub Apiwebbox_TextChanged(sender As Object, e As TextChangedEventArgs) Handles Apiwebbox.TextChanged
        Config.Apiweb.Val = Me.Apiwebbox.Text
        Config.WriteConfig()

    End Sub
End Class
