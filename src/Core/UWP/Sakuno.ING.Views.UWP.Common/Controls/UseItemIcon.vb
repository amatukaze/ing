Imports Sakuno.ING.Game.Models.Knowledge

Namespace Controls
    Public Class UseItemIcon
        Inherits UserControl

        Private ReadOnly _image As Image
        Public Sub New()
            _image = New Image()
            Content = _image
        End Sub

        Public Shared ReadOnly IdProperty As DependencyProperty =
            DependencyProperty.Register(NameOf(Id), GetType(KnownUseItem), GetType(UseItemIcon),
                                        New PropertyMetadata(New KnownUseItem, Sub(d, e) CType(d, UseItemIcon).OnIdChanged(CType(e.NewValue, KnownUseItem))))

        Public Property Id As KnownUseItem
            Get
                Return CType(GetValue(IdProperty), KnownUseItem)
            End Get
            Set(value As KnownUseItem)
                SetValue(IdProperty, value)
            End Set
        End Property

        Private Sub OnIdChanged(value As KnownUseItem)
            Dim intId As Integer = value
            If DesignMode.DesignModeEnabled Then
                _image.Source = New BitmapImage(New Uri($"ms-appx:///Sakuno.ING.Views.UWP.Common/Assets/Images/UseItem/{intId}.png"))
            Else
                Dim stringId = "UseItemIcon_Bitmap_" & intId
                Dim source As Object = Nothing
                If Application.Current.Resources.TryGetValue(stringId, source) Then
                    _image.Source = DirectCast(source, BitmapImage)
                Else
                    Dim s = New BitmapImage(New Uri($"ms-appx:///Sakuno.ING.Views.UWP.Common/Assets/Images/UseItem/{intId}.png"))
                    Application.Current.Resources.Add(stringId, s)
                    _image.Source = s
                End If
            End If
        End Sub
    End Class
End Namespace
