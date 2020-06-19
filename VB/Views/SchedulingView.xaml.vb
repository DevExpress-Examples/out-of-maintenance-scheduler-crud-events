Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Windows.Controls
Imports DevExpress.Mvvm.UI.Interactivity
Imports DevExpress.Xpf.Scheduling

Namespace DXSample.Views
	Partial Public Class SchedulingView
		Inherits UserControl

		Public ReadOnly Property Scheduler() As SchedulerControl
			Get
				Return Me.ChildlScheduler
			End Get
		End Property
		Public Sub New()
			InitializeComponent()
		End Sub
	End Class
End Namespace
