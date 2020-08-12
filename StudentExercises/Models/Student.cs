using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StudentExercises.Models
{
    public class Student : Person
    {
        public List<Exercise> Exercises { get; set; } = new List<Exercise>();
    }
}
