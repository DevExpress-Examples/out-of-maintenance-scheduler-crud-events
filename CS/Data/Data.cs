using System;

namespace DXSample.Data {

    public class AppointmentEntity {
        public int Id { get; set; }
        public string Subject { get; set; }
        public string Description { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public int AppointmentType { get; set; }
        public string RecurrenceInfo { get; set; }
        public int ResourceId { get; set; }
        public int Label { get; set; }
    }
    public class ResourceEntity {
        public int Id { get; set; }
        public string Description { get; set; }
    }
}
