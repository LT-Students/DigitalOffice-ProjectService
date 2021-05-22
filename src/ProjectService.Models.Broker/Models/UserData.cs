using System;

namespace LT.DigitalOffice.Broker.Models
{
    public class UserData
    {
        public Guid Id { get; set; }
        public string FirstName { get; }
        public string LastName { get; }
        public string MiddleName { get; }
        public bool IsActive { get; }

        public UserData(Guid id, string firstName, string middleName, string lastName, bool isActive)
        {
            Id = id;
            FirstName = firstName;
            LastName = lastName;
            MiddleName = middleName;
            IsActive = isActive;
        }
    }
}
