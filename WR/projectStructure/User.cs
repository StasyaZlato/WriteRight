using System;

namespace ProjectStructure
{
    [Serializable]
    public class User
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public User() { }

        public User(string fn, string ln)
        {
            FirstName = fn;
            LastName = ln;
        }
    }
}