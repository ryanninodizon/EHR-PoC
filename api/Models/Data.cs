using System.Collections.Generic;

namespace Models;

    public class Patient
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }
        public string Gender { get; set; }
        public Address Address { get; set; }
        public List<MedicalHistory> MedicalHistory { get; set; }
        public List<LaboratoryTest> LaboratoryTests { get; set; }
    }

    public class Address
    {
        public string Street { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
    }

    public class MedicalHistory
    {
        public int Id { get; set; }
        public string Condition { get; set; }
        public DateTime DiagnosisDate { get; set; }
        public string PatientId { get; set; }
        public Patient Patient { get; set; }
    }

    public class LaboratoryTest
    {
        public int Id { get; set; }
        public string LabId { get; set; }
        public string TestId { get; set; }
        public string PatientId { get; set; }
        public Patient Patient { get; set; }
    }
