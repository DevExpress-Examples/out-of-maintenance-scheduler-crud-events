Imports System.Collections.ObjectModel
Imports DevExpress.Mvvm
Imports DXSample.Data

Namespace DXSample.ViewModels

    Public Class SchedulingViewModel
        Inherits ViewModelBase

        Public Property Appts As ObservableCollection(Of AppointmentEntity)
            Get
                Return GetValue(Of ObservableCollection(Of AppointmentEntity))()
            End Get

            Set(ByVal value As ObservableCollection(Of AppointmentEntity))
                SetValue(value)
            End Set
        End Property

        Public Property Calendars As ObservableCollection(Of ResourceEntity)
            Get
                Return GetValue(Of ObservableCollection(Of ResourceEntity))()
            End Get

            Set(ByVal value As ObservableCollection(Of ResourceEntity))
                SetValue(value)
            End Set
        End Property

        Public Sub New()
            Appts = DataHelper.GetAppointments()
            Calendars = DataHelper.GetResources()
        End Sub
    End Class
End Namespace
