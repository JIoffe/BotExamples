using System;

namespace BIADTemplate.Model
{
    [Serializable]
    public class Profile
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Occupation { get; set; }
        public string HomeTown { get; set; }
    }
}