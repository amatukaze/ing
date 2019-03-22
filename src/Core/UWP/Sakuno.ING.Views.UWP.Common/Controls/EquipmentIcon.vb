Namespace Controls
    Public Class EquipmentIcon
        Inherits UserControl

        Private ReadOnly _image As New Image
        Public Sub New()
            Content = _image
        End Sub

        Private _id As Integer
        Public Property Id As Integer
            Get
                Return _id
            End Get
            Set(value As Integer)
                If _id <> value Then
                    _id = value
                    If DesignMode.DesignModeEnabled Then
                        _image.Source = New BitmapImage(New Uri($"ms-appx:///Sakuno.ING.Views.UWP.Common/Assets/Images/Equipment/{value}.png"))
                    Else
                        Dim stringId = "EquipmentIcon_Bitmap_" & value
                        Dim source As Object = Nothing
                        If Application.Current.Resources.TryGetValue(stringId, source) Then
                            _image.Source = DirectCast(source, BitmapImage)
                        Else
                            Dim s = New BitmapImage(New Uri($"ms-appx:///Sakuno.ING.Views.UWP.Common/Assets/Images/Equipment/{value}.png"))
                            Application.Current.Resources.Add(stringId, s)
                            _image.Source = s
                        End If
                    End If
                End If
            End Set
        End Property
    End Class
End Namespace
