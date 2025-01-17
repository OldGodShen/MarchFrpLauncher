﻿Imports System.ComponentModel
Imports MarchFrpLauncher.UI

Class MainWindow

#Region "自定义标题栏的交互"
#Disable Warning BC42025
    ''' <summary>
    ''' 窗口拖动
    ''' </summary>
    Private Sub TitleBar_MouseLeftButtonDown(sender As Object, e As MouseButtonEventArgs) Handles TitleBar.MouseLeftButtonDown
        Me.DragMove()
    End Sub

    ''' <summary>
    ''' 窗口关闭
    ''' </summary>
    Private Sub Bt_Close_Click(sender As Object, e As RoutedEventArgs) Handles Bt_Close.Click
        Me.Close()
    End Sub

    ''' <summary>
    ''' 窗口最小化
    ''' </summary>
    Private Sub Bt_Min_Click(sender As Object, e As RoutedEventArgs) Handles Bt_Min.Click
        Me.WindowState = WindowState.Minimized
    End Sub

    Dim clicks As Integer = 0
    ''' <summary>
    ''' 取消最大化
    ''' </summary>
    Private Sub MainWindow_SizeChanged(sender As Object, e As SizeChangedEventArgs) Handles Me.SizeChanged
        If Me.WindowState = WindowState.Maximized Then
            Me.WindowState = WindowState.Normal
        End If
    End Sub

#Enable Warning BC42025
#End Region

    Dim ac As New Assets()  'Temp用ac
    Dim Assets As AssetModel
    Dim Config As New Config()

    Private Sub _init_() Handles Me.Loaded
        Config.ReadConfig()
        Me.Assets = ac.SearchAsset(Config.Lang.Val)
        Me.LoginBox.Assets = Me.Assets
        Me.LoginBox.Visibility = Visibility.Visible
        Me.LoginBox.Config = Me.Config
        Me.LoginBox._init_()
        Me.txtTitle.Foreground = Brushes.Black
        Me.txtTitle.Text = Assets.Welcome
        Me.MainPanel.Assets = Me.Assets
        Me.MainPanel.ctm_SettingsPage.Config = Me.Config
    End Sub

    Private Sub MainWindow_Closing(sender As Object, e As CancelEventArgs) Handles Me.Closing
        End
    End Sub

    Private Sub LoginBox_LoginSucceed() Handles LoginBox.LoginSucceed
        Me.txtTitle.Foreground = Brushes.White
        Me.bd_titlefillL.Visibility = Visibility.Collapsed
        Me.MainPanel.Visibility = Visibility.Visible
        Me.MainPanel.Username = Me.LoginBox.Username
        Me.MainPanel.Nickname = Me.LoginBox.Username
        Me.MainPanel.UserToken = Me.LoginBox.UserToken
        Me.MainPanel.Config = Me.Config
        Me.MainPanel._init_()
        Me.txtTitle.Text = "MarchFrpLauncher"
    End Sub

    Private Sub MainWindow_Activated(sender As Object, e As EventArgs) Handles Me.Activated
        If Me.bd_titlefillL.Visibility = Visibility.Visible Then
            Me.txtTitle.Foreground = Brushes.Black
        Else
            Me.txtTitle.Foreground = Brushes.White
        End If
    End Sub

    Private Sub MainWindow_Deactivated(sender As Object, e As EventArgs) Handles Me.Deactivated
        Me.txtTitle.Foreground = Brushes.DarkGray
    End Sub
End Class
