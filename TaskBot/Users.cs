using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TaskBot
{
    public class Users
    {
        public string name { get; set; } = "";


        public int taskCount { get; set; }
        public List<Tasks> tasks { get; set; }

        public Users()
        {
            tasks = new List<Tasks>();
        }
    }
}
