Imports System.IO
Imports Sakuno.ING.Composition
Imports Sakuno.ING.Game.Logger
Imports Sakuno.ING.Shell

<Export(GetType(LogMigrationVM), SingleInstance:=False)>
Public Class LogMigrationVM
    Inherits BindableObject

    Private ReadOnly logger As Logger
    Private ReadOnly fsPicker As IFileSystemPickerService
    Public ReadOnly Property Migrators As IBindableCollection(Of ILogMigrator)

    Private _selectedMigrator As ILogMigrator
    Public Property SelectedMigrator As ILogMigrator
        Get
            Return _selectedMigrator
        End Get
        Set(value As ILogMigrator)
            If _selectedMigrator IsNot value Then
                _selectedMigrator = value
                SelectedPath = Nothing
                NotifyPropertyChanged()
                NotifyPropertyChanged(NameOf(SupportShipCreation))
                NotifyPropertyChanged(NameOf(SupportEquipmentCreation))
                NotifyPropertyChanged(NameOf(SupportExpeditionCompletion))
            End If
        End Set
    End Property

    Public ReadOnly Property Ready As Boolean
        Get
            Return SelectedMigrator IsNot Nothing AndAlso
                SelectedPath IsNot Nothing AndAlso
                SelectedPath.Exists
        End Get
    End Property

    Private _selectedFs As FileSystemInfo
    Public Property SelectedPath As FileSystemInfo
        Get
            Return _selectedFs
        End Get
        Private Set(value As FileSystemInfo)
            [Set](_selectedFs, value)
            NotifyPropertyChanged(NameOf(Ready))
        End Set
    End Property

    Public ReadOnly Property SupportShipCreation As Boolean
        Get
            Return If(SelectedMigrator?.SupportedTypes.HasFlag(LogType.ShipCreation), False)
        End Get
    End Property
    Public Property SelectShipCreation As Boolean

    Public ReadOnly Property SupportEquipmentCreation As Boolean
        Get
            Return If(SelectedMigrator?.SupportedTypes.HasFlag(LogType.EquipmentCreation), False)
        End Get
    End Property
    Public Property SelectEquipmentCreation As Boolean

    Public ReadOnly Property SupportExpeditionCompletion As Boolean
        Get
            Return If(SelectedMigrator?.SupportedTypes.HasFlag(LogType.ExpeditionCompletion), False)
        End Get
    End Property
    Public Property SelectExpeditionCompletion As Boolean

    Public Sub New(logger As Logger, migrators As ILogMigrator(), fsPicker As IFileSystemPickerService)
        Me.logger = logger
        Me.fsPicker = fsPicker
        Me.Migrators = migrators.AsBindable()
    End Sub

    Private _ranged As Boolean
    Public Property Ranged As Boolean
        Get
            Return _ranged
        End Get
        Set(value As Boolean)
            [Set](_ranged, value)
        End Set
    End Property
    Public Property TimeZoneOffset As Double
    Public Property DateFrom As DateTime
    Public Property DateTo As DateTime

    Public Async Sub PickPath()
        Dim fs As FileSystemInfo
        If SelectedMigrator.RequireFolder Then
            fs = Await fsPicker.PickFolderAsync()
        Else
            fs = Await fsPicker.OpenFileAsync()
        End If
        If fs IsNot Nothing Then SelectedPath = fs
    End Sub

    Public Async Sub DoMigration()
        Dim types As LogType = 0
        If SelectShipCreation AndAlso SupportShipCreation Then types = types Or LogType.ShipCreation
        If SelectEquipmentCreation AndAlso SupportEquipmentCreation Then types = types Or LogType.EquipmentCreation
        If SelectExpeditionCompletion AndAlso SupportExpeditionCompletion Then types = types Or LogType.ExpeditionCompletion

        Dim offset = TimeSpan.FromHours(TimeZoneOffset)
        Dim range As TimeRange? = Nothing
        Dim from = DateTime.SpecifyKind(DateFrom, DateTimeKind.Utc).Subtract(offset)
        Dim [to] = DateTime.SpecifyKind(DateTo, DateTimeKind.Utc).Subtract(offset)
        If Ranged Then range = New TimeRange(from, [to])

        Try
            Using context = logger.CreateContext()
                Await SelectedMigrator.MigrateAsync(SelectedPath, context, types, offset, range)
                Await context.SaveChangesAsync()
            End Using
        Catch
            ' Prompt
        End Try
    End Sub
End Class
