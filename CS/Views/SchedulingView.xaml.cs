using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using DevExpress.Mvvm.UI.Interactivity;
using DevExpress.Xpf.Scheduling;

namespace DXSample.Views {
    public partial class SchedulingView : UserControl {
        public SchedulerControl Scheduler { get { return this.ChildlScheduler; } }
        public SchedulingView() {
            InitializeComponent();            
        }
    }
}
