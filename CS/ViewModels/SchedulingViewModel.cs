using System;
using System.Collections.ObjectModel;
using DevExpress.Mvvm;
using DXSample.Data;

namespace DXSample.ViewModels {
    public class SchedulingViewModel : ViewModelBase {
        public ObservableCollection<AppointmentEntity> Appts {
            get { return GetValue<ObservableCollection<AppointmentEntity>>(); }
            set { SetValue(value); }
        }
        public ObservableCollection<ResourceEntity> Calendars {
            get { return GetValue<ObservableCollection<ResourceEntity>>(); }
            set { SetValue(value); }
        }        
        public SchedulingViewModel() {
            Appts = DataHelper.GetAppointments();
            Calendars = DataHelper.GetResources();
        }        
    }
}