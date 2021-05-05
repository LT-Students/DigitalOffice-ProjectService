using System;

namespace LT.DigitalOffice.Broker.Models
{
    public class UserData
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MiddleName { get; set; }
        public bool IsActive { get; set; }

        public static UserData Create(Guid id, string firstName, string middleName, string lastName, bool isActive)
        {
            return new UserData
            {
                Id = id,
                FirstName = firstName,
                LastName = lastName,
                MiddleName = middleName,
                IsActive = isActive
            };
        }
    }
}
