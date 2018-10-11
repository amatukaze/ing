Namespace Controls
    Public Class AirProficiencyPresenter
        Inherits UserControl

        Private ReadOnly _image As New Image
        Public Sub New()
            Content = _image
        End Sub

        Private _proficiency As Integer
        Public Property Proficiency As Integer
            Get
                Return _proficiency
            End Get
            Set(value As Integer)
                If _proficiency <> value Then
                    _proficiency = value
                    If value = 0 Then
                        _image.Source = Nothing
                    ElseIf DesignMode.DesignModeEnabled Then
                        _image.Source = New BitmapImage(New Uri($"ms-appx:///Sakuno.ING.Views.UWP.Common/Assets/Images/AirProficiency/{value}.png"))
                    Else
                        Dim stringId = "UseItemIcon_Bitmap_" & value
                        Dim source As Object = Nothing
                        If Application.Current.Resources.TryGetValue(stringId, source) Then
                            _image.Source = DirectCast(source, BitmapImage)
                        Else
                            Dim s = New BitmapImage(New Uri($"ms-appx:///Sakuno.ING.Views.UWP.Common/Assets/Images/AirProficiency/{value}.png"))
                            Application.Current.Resources.Add(stringId, s)
                            _image.Source = s
                        End If
                    End If
                End If
            End Set
        End Property
    End Class
End Namespace
