Imports System.IO
Imports Microsoft.EntityFrameworkCore
Imports Sakuno.ING.Composition
Imports Sakuno.ING.Game.Logger
Imports Sakuno.ING.Game.Logger.Entities
Imports Sakuno.ING.Shell

<Export(GetType(LogMigrationVM), SingleInstance:=False)>
Public Class LogMigrationVM
    Inherits BindableObject

    Private ReadOnly logger As Logger
    Private ReadOnly shell As IShell
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
            Return TypeOf SelectedMigrator Is ILogProvider(Of ShipCreation)
        End Get
    End Property
    Public Property SelectShipCreation As Boolean

    Public ReadOnly Property SupportEquipmentCreation As Boolean
        Get
            Return TypeOf SelectedMigrator Is ILogProvider(Of EquipmentCreation)
        End Get
    End Property
    Public Property SelectEquipmentCreation As Boolean

    Public ReadOnly Property SupportExpeditionCompletion As Boolean
        Get
            Return TypeOf SelectedMigrator Is ILogProvider(Of ExpeditionCompletion)
        End Get
    End Property
    Public Property SelectExpeditionCompletion As Boolean

    Public Sub New(logger As Logger, migrators As ILogMigrator(), shell As IShell)
        Me.logger = logger
        Me.shell = shell
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
    Public Property DateFrom As DateTime = DateTime.Now
    Public Property DateTo As DateTime = DateTime.Now

    Public Async Sub PickPath()
        Dim fs As FileSystemInfo
        If SelectedMigrator.RequireFolder Then
            fs = Await shell.PickFolderAsync()
        Else
            fs = Await shell.OpenFileAsync()
        End If
        If fs IsNot Nothing Then SelectedPath = fs
    End Sub

    Private Async Function TryMigrate(Of T As {Class, ITimedEntity})(db As DbSet(Of T), provider As ILogProvider(Of T)) As Task
        If provider Is Nothing Then Return

        Dim source = (Await provider.GetLogsAsync(SelectedPath, TimeSpan.FromHours(TimeZoneOffset))).AsEnumerable()
        If Ranged Then
            Dim timeZone = TimeSpan.FromHours(TimeZoneOffset)
            Dim timeFrom = DateTime.SpecifyKind(DateFrom, DateTimeKind.Utc).Subtract(timeZone)
            Dim timeTo = DateTime.SpecifyKind(DateTo, DateTimeKind.Utc).Subtract(timeZone)
            source = From e In source Where e.TimeStamp > timeFrom AndAlso e.TimeStamp < timeTo
        End If

        Dim index As New HashSet(Of Long)(From e In db Select e.TimeStamp.ToUnixTimeSeconds())
        Await db.AddRangeAsync(From e In source Where Not index.Contains(e.TimeStamp.ToUnixTimeSeconds()))
    End Function

    Public Async Sub DoMigration()
        Dim ex As Exception = Nothing
        Try
            Using context = logger.CreateContext()
                Await TryMigrate(context.ShipCreationTable, TryCast(SelectedMigrator, ILogProvider(Of ShipCreation)))
                Await TryMigrate(context.EquipmentCreationTable, TryCast(SelectedMigrator, ILogProvider(Of EquipmentCreation)))
                Await TryMigrate(context.ExpeditionCompletionTable, TryCast(SelectedMigrator, ILogProvider(Of ExpeditionCompletion)))
                Await context.SaveChangesAsync()
            End Using
        Catch e As Exception
            ex = e
        End Try

        If ex Is Nothing Then
            Await shell.ShowMessageAsync("Log migration completed successfully.", "Migration Completed")
        Else
            Await shell.ShowMessageAsync(ex.ToString(), "Migration Failed")
        End If
    End Sub
End Class
