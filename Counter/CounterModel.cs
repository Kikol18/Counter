using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Counter
{
    public class CounterModel
    {
        public string Name { get; set; }
        public int Value { get; set; }

        public CounterModel(string name)
        {
            Name = name;
            Value = 0;
        }
    }
}
