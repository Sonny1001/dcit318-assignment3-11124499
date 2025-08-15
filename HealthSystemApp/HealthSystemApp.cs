using System;
using System.Collections.Generic;
using System.Linq;

namespace HealthSystemApp
{
    // a) Generic Repository
    public class Repository<T>
    {
        private readonly List<T> items = new();
        public void Add(T item) => items.Add(item);
        public List<T> GetAll() => new List<T>(items);
        public T? GetById(Func<T, bool> predicate) => items.FirstOrDefault(predicate);
        public bool Remove(Func<T, bool> predicate)
        {
            var toRemove = items.FirstOrDefault(predicate);
            if (toRemove is null) return false;
            return items.Remove(toRemove);
        }
    }

    // b) Patient
    public class Patient
    {
        public int Id;
        public string Name;
        public int Age;
        public string Gender;

        public Patient(int id, string name, int age, string gender)
        {
            Id = id; Name = name; Age = age; Gender = gender;
        }

        public override string ToString() => $"Patient {{ Id={Id}, Name={Name}, Age={Age}, Gender={Gender} }}";
    }

    // c) Prescription
    public class Prescription
    {
        public int Id;
        public int PatientId;
        public string MedicationName;
        public DateTime DateIssued;

        public Prescription(int id, int patientId, string medicationName, DateTime dateIssued)
        {
            Id = id; PatientId = patientId; MedicationName = medicationName; DateIssued = dateIssued;
        }

        public override string ToString() => $"Prescription {{ Id={Id}, PatientId={PatientId}, Medication={MedicationName}, DateIssued={DateIssued:d} }}";
    }

    public class HealthSystemApp
    {
        private readonly Repository<Patient> _patientRepo = new();
        private readonly Repository<Prescription> _prescriptionRepo = new();
        private Dictionary<int, List<Prescription>> _prescriptionMap = new();

        // d,e) Build dictionary mapping
        public List<Prescription> GetPrescriptionsByPatientId(int patientId)
        {
            return _prescriptionMap.TryGetValue(patientId, out var list) ? list : new List<Prescription>();
        }

        // g) Methods
        public void SeedData()
        {
            _patientRepo.Add(new Patient(1, "Kingsford Asante", 28, "Male"));
            _patientRepo.Add(new Patient(2, "Kofi Mensah", 43, "Male"));
            _patientRepo.Add(new Patient(3, "Cynthia Agyei", 35, "Female"));

            _prescriptionRepo.Add(new Prescription(100, 1, "Amoxicillin 400mg", DateTime.Today.AddDays(-10)));
            _prescriptionRepo.Add(new Prescription(101, 1, "Ibuprofen 200mg", DateTime.Today.AddDays(-5)));
            _prescriptionRepo.Add(new Prescription(102, 2, "Metformin 500mg", DateTime.Today.AddDays(-2)));
            _prescriptionRepo.Add(new Prescription(103, 3, "Loratadine 20mg", DateTime.Today.AddDays(-1)));
            _prescriptionRepo.Add(new Prescription(104, 2, "Atorvastatin 10mg", DateTime.Today));
        }

        public void BuildPrescriptionMap()
        {
            _prescriptionMap = _prescriptionRepo.GetAll()
                .GroupBy(p => p.PatientId)
                .ToDictionary(g => g.Key, g => g.ToList());
        }

        public void PrintAllPatients()
        {
            Console.WriteLine("=== All Patients ===");
            foreach (var p in _patientRepo.GetAll())
                Console.WriteLine(p);
        }

        public void PrintPrescriptionsForPatient(int id)
        {
            Console.WriteLine($"=== Prescriptions for PatientId={id} ===");
            foreach (var pr in GetPrescriptionsByPatientId(id))
                Console.WriteLine(pr);
        }

        // Main flow
        public static void Main()
        {
            var app = new HealthSystemApp();
            app.SeedData();
            app.BuildPrescriptionMap();
            app.PrintAllPatients();
            app.PrintPrescriptionsForPatient(2); // Select one patient to display
        }
    }
}