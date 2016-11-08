﻿Imports Oracle.LinuxCompatibility.MySQL
Imports SMRUCC.WebCloud.DataCenter.mysql

Public Class SubscriptionMgr

    Public ReadOnly Property MySQL As MySQL
    Public ReadOnly Property AppId As Integer

    Sub New(app%, cnn As ConnectionUri)
        MySQL = New MySQL(cnn)
        AppId = app
    End Sub

    ''' <summary>
    ''' 添加订阅用户
    ''' </summary>
    ''' <param name="email$"></param>
    Public Sub Add(email$)
        Dim user As New subscription With {
            .app = AppId,
            .email = email,
            .hash = $"{AppId}-{LCase(email)}".MD5
        }
        Call MySQL.ExecInsert(user)
    End Sub

    Public Sub Active(hash$)
        Call __changeStatus(1, hash)
    End Sub

    Private Sub __changeStatus(active&, hash$)
        Dim SQL$ = String.Format(SQLGetUserByHash, hash)
        Dim user As subscription =
            MySQL.ExecuteScalar(Of subscription)(SQL)

        If Not user Is Nothing Then
            user.active = active
            Call MySQL.ExecUpdate(user)
        End If
    End Sub

    Const SQLGetUserByHash$ = "SELECT * FROM smrucc_webcloud.subscription where lower(hash) = lower('{0}');"

    Public Sub Remove(hash$)
        Call __changeStatus(0, hash)
    End Sub
End Class
