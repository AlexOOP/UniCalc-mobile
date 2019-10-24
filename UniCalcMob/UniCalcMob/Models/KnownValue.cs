using System;
using System.Collections.Generic;
using System.Text;

namespace UniCalcMob.Models
{
    public class KnownValue
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public override string ToString()
        {
          return this.Name;
        }

    }
}
