using System;
using System.Collections.Generic;
using System.Linq;
using DevExpress.Mvvm;
using DevExpress.Mvvm.UI.Interactivity;
using DevExpress.Xpf.Scheduling;
using DevExpress.XtraScheduler;

namespace DXSample.Behaviors {
    public class SchedulerControlEditingBehavior : Behavior<SchedulerControl> {
        bool isUpdating = false;
        private void AssociatedObject_AppointmentEditing(object sender, AppointmentEditingEventArgs e) {
            if (isUpdating) return;
            List<AppointmentItem> newAppointments = new List<AppointmentItem>();
            foreach (var editAppt in e.EditAppointments) {
                AppointmentItem sourceAppt = e.SourceAppointments[e.EditAppointments.ToList().IndexOf(editAppt)];
                if(SkipAppt(sourceAppt)) continue;
                editAppt.RecurrenceInfo.Start = GetStartForNewPattern(sourceAppt, editAppt);
                editAppt.Start = GetStartForNewPattern(sourceAppt, editAppt);
                var oldAppt = AssociatedObject.CopyAppointment(sourceAppt);
                oldAppt.RecurrenceInfo.Range = RecurrenceRange.EndByDate;
                oldAppt.RecurrenceInfo.End = GetEndForOldPattern(sourceAppt);
                newAppointments.Add(oldAppt);
            }
            AssociatedObject.AddAppointments(newAppointments);            
        }
        private void AssociatedObject_AppointmentRemoving(object sender, AppointmentRemovingEventArgs e) {
            List<AppointmentItem> collection = new List<AppointmentItem>();
            List<AppointmentItem> updatedAppointments = new List<AppointmentItem>();
            foreach (var editAppt in e.Appointments) {
                if (SkipAppt(editAppt)) continue;
                e.CanceledAppointments.Add(editAppt);
                collection.Add(editAppt);
                var modifiedAppt = AssociatedObject.CopyAppointment(editAppt);
                modifiedAppt.RecurrenceInfo.Range = RecurrenceRange.EndByDate;
                modifiedAppt.RecurrenceInfo.End = GetEndForOldPattern(editAppt);
                updatedAppointments.Add(modifiedAppt);
            }            
            isUpdating = true;
            AssociatedObject.EditAppointments(collection, updatedAppointments);
            isUpdating = false;
        }
        private bool SkipAppt(AppointmentItem editAppt) {
            if (editAppt.Type != AppointmentType.Pattern || editAppt.Start > DateTime.Now)
                return true;
            var rec = editAppt.RecurrenceInfo;
            if (rec.Range == RecurrenceRange.EndByDate && rec.End <= DateTime.Now)
                return true;
            if (rec.Range == RecurrenceRange.OccurrenceCount && editAppt.QueryEnd <= DateTime.Now)
                return true;
            return false;
        }       
        DateTime GetEndForOldPattern(AppointmentItem appt) {
            AppointmentItem occ = AssociatedObject
                .GetOccurrencesAndExceptions(appt, new DateTimeRange(DateTime.Today, GetMinTimeSpan(appt)))
                .OrderBy(x => x.Start)
                .FirstOrDefault(); 
            if (occ == null || occ.Start > DateTime.Now)
                return DateTime.Today.AddDays(-1);
            return DateTime.Today;
        }
        DateTime GetStartForNewPattern(AppointmentItem sourceAppt, AppointmentItem editAppt) {
            AppointmentItem occ = AssociatedObject
                .GetOccurrencesAndExceptions(sourceAppt, new DateTimeRange(DateTime.Now, GetMinTimeSpan(sourceAppt)))
                .OrderBy(x => x.Start)
                .FirstOrDefault();                     
            return new DateTime(occ.Start.Year, occ.Start.Month, occ.Start.Day, editAppt.Start.Hour, editAppt.Start.Minute, editAppt.Start.Second);
        }
        TimeSpan GetMinTimeSpan(AppointmentItem appointmentItem) {
            switch (appointmentItem.RecurrenceInfo.Type) {
                case RecurrenceType.Daily:
                    return TimeSpan.FromDays(1);
                case RecurrenceType.Weekly:
                    return TimeSpan.FromDays(7 * appointmentItem.RecurrenceInfo.Periodicity);
                case RecurrenceType.Monthly:
                    return TimeSpan.FromDays(31 * appointmentItem.RecurrenceInfo.Periodicity);
                case RecurrenceType.Yearly:
                    return TimeSpan.FromDays(365 * appointmentItem.RecurrenceInfo.Periodicity);
                case RecurrenceType.Minutely:
                    return TimeSpan.FromMinutes(appointmentItem.RecurrenceInfo.Periodicity);
                case RecurrenceType.Hourly:
                    return TimeSpan.FromHours(appointmentItem.RecurrenceInfo.Periodicity);
                default:
                    return TimeSpan.FromDays(365 * appointmentItem.RecurrenceInfo.Periodicity);
            }
        }

        protected override void OnAttached() {
            base.OnAttached();
            AssociatedObject.AppointmentRemoving += AssociatedObject_AppointmentRemoving;
            AssociatedObject.AppointmentEditing += AssociatedObject_AppointmentEditing;
        }
        protected override void OnDetaching() {
            AssociatedObject.AppointmentRemoving -= AssociatedObject_AppointmentRemoving;
            AssociatedObject.AppointmentEditing -= AssociatedObject_AppointmentEditing;
            base.OnDetaching();
        }
    }
}
