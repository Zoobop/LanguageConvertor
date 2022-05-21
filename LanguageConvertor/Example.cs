using System.Collections.Concurrent;
using System.Collections.Generic;

namespace ConversionOutput
{
    public class Example : ISomeInterface, IBuilder, AbstractClass
    {
        private String firstName;
        private String lastName;
        private int age;
        private String job;
    
        public Example(String firstName, String lastName, int age, String job)
        {
            this.firstName = firstName;
            this.lastName = lastName;
            this.age = age;
            this.job = job;
        }

        public String GetFirstName()
        {
            return firstName;
        }
        
        public String GetLastName()
        {
            return lastName;
        }

        public int GetAge()
        {
            return age;
        }

        private String GetJob()
        {
            return job;
        }

        protected override string FormatAge()
        {
            return $"{age}";
        }
    }
}