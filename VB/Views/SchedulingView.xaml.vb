Imports System.Windows.Controls
Imports DevExpress.Xpf.Scheduling

Namespace DXSample.Views

    Public Partial Class SchedulingView
        Inherits UserControl

        Public ReadOnly Property Scheduler As SchedulerControl
            Get
                Return Me.ChildlScheduler
            End Get
        End Property

        Public Sub New()
            Me.InitializeComponent()
        End Sub
    End Class
End Namespace
