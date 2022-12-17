Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports DevExpress.Mvvm
Imports DevExpress.Mvvm.UI.Interactivity
Imports DevExpress.Xpf.Scheduling
Imports DevExpress.XtraScheduler

Namespace DXSample.Behaviors

    Public Class SchedulerControlEditingBehavior
        Inherits Behavior(Of SchedulerControl)

        Private isUpdating As Boolean = False

        Private Sub AssociatedObject_AppointmentEditing(ByVal sender As Object, ByVal e As AppointmentEditingEventArgs)
            If isUpdating Then Return
            Dim newAppointments As List(Of AppointmentItem) = New List(Of AppointmentItem)()
            For Each editAppt In e.EditAppointments
                Dim sourceAppt As AppointmentItem = e.SourceAppointments(Enumerable.ToList(e.EditAppointments).IndexOf(editAppt))
                If SkipAppt(sourceAppt) Then Continue For
                editAppt.RecurrenceInfo.Start = GetStartForNewPattern(sourceAppt, editAppt)
                editAppt.Start = GetStartForNewPattern(sourceAppt, editAppt)
                Dim oldAppt = AssociatedObject.CopyAppointment(sourceAppt)
                oldAppt.RecurrenceInfo.Range = RecurrenceRange.EndByDate
                oldAppt.RecurrenceInfo.End = GetEndForOldPattern(sourceAppt)
                newAppointments.Add(oldAppt)
            Next

            AssociatedObject.AddAppointments(newAppointments)
        End Sub

        Private Sub AssociatedObject_AppointmentRemoving(ByVal sender As Object, ByVal e As AppointmentRemovingEventArgs)
            Dim collection As List(Of AppointmentItem) = New List(Of AppointmentItem)()
            Dim updatedAppointments As List(Of AppointmentItem) = New List(Of AppointmentItem)()
            For Each editAppt In e.Appointments
                If SkipAppt(editAppt) Then Continue For
                e.CanceledAppointments.Add(editAppt)
                collection.Add(editAppt)
                Dim modifiedAppt = AssociatedObject.CopyAppointment(editAppt)
                modifiedAppt.RecurrenceInfo.Range = RecurrenceRange.EndByDate
                modifiedAppt.RecurrenceInfo.End = GetEndForOldPattern(editAppt)
                updatedAppointments.Add(modifiedAppt)
            Next

            isUpdating = True
            AssociatedObject.EditAppointments(collection, updatedAppointments)
            isUpdating = False
        End Sub

        Private Function SkipAppt(ByVal editAppt As AppointmentItem) As Boolean
            If editAppt.Type <> AppointmentType.Pattern OrElse editAppt.Start > Date.Now Then Return True
            Dim rec = editAppt.RecurrenceInfo
            If rec.Range = RecurrenceRange.EndByDate AndAlso rec.End <= Date.Now Then Return True
            If rec.Range = RecurrenceRange.OccurrenceCount AndAlso editAppt.QueryEnd <= Date.Now Then Return True
            Return False
        End Function

        Private Function GetEndForOldPattern(ByVal appt As AppointmentItem) As Date
            Dim occ As AppointmentItem = AssociatedObject.GetOccurrencesAndExceptions(appt, New DateTimeRange(Date.Today, GetMinTimeSpan(appt))).OrderBy(Function(x) x.Start).FirstOrDefault()
            If occ Is Nothing OrElse occ.Start > Date.Now Then Return Date.Today.AddDays(-1)
            Return Date.Today
        End Function

        Private Function GetStartForNewPattern(ByVal sourceAppt As AppointmentItem, ByVal editAppt As AppointmentItem) As Date
            Dim occ As AppointmentItem = AssociatedObject.GetOccurrencesAndExceptions(sourceAppt, New DateTimeRange(Date.Now, GetMinTimeSpan(sourceAppt))).OrderBy(Function(x) x.Start).FirstOrDefault()
            Return New DateTime(occ.Start.Year, occ.Start.Month, occ.Start.Day, editAppt.Start.Hour, editAppt.Start.Minute, editAppt.Start.Second)
        End Function

        Private Function GetMinTimeSpan(ByVal appointmentItem As AppointmentItem) As TimeSpan
            Select Case appointmentItem.RecurrenceInfo.Type
                Case RecurrenceType.Daily
                    Return TimeSpan.FromDays(1)
                Case RecurrenceType.Weekly
                    Return TimeSpan.FromDays(7 * appointmentItem.RecurrenceInfo.Periodicity)
                Case RecurrenceType.Monthly
                    Return TimeSpan.FromDays(31 * appointmentItem.RecurrenceInfo.Periodicity)
                Case RecurrenceType.Yearly
                    Return TimeSpan.FromDays(365 * appointmentItem.RecurrenceInfo.Periodicity)
                Case RecurrenceType.Minutely
                    Return TimeSpan.FromMinutes(appointmentItem.RecurrenceInfo.Periodicity)
                Case RecurrenceType.Hourly
                    Return TimeSpan.FromHours(appointmentItem.RecurrenceInfo.Periodicity)
                Case Else
                    Return TimeSpan.FromDays(365 * appointmentItem.RecurrenceInfo.Periodicity)
            End Select
        End Function

        Protected Overrides Sub OnAttached()
            MyBase.OnAttached()
            AddHandler AssociatedObject.AppointmentRemoving, AddressOf AssociatedObject_AppointmentRemoving
            AddHandler AssociatedObject.AppointmentEditing, AddressOf AssociatedObject_AppointmentEditing
        End Sub

        Protected Overrides Sub OnDetaching()
            RemoveHandler AssociatedObject.AppointmentRemoving, AddressOf AssociatedObject_AppointmentRemoving
            RemoveHandler AssociatedObject.AppointmentEditing, AddressOf AssociatedObject_AppointmentEditing
            MyBase.OnDetaching()
        End Sub
    End Class
End Namespace
