Imports System

Namespace DXSample.Data

	Public Class AppointmentEntity
		Public Property Id() As Integer
		Public Property Subject() As String
		Public Property Description() As String
		Public Property Start() As DateTime
		Public Property [End]() As DateTime
		Public Property AppointmentType() As Integer
		Public Property RecurrenceInfo() As String
		Public Property ResourceId() As Integer
		Public Property Label() As Integer
	End Class
	Public Class ResourceEntity
		Public Property Id() As Integer
		Public Property Description() As String
	End Class
End Namespace
