using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsonDataGenerate
{
    public class Field
    {
        public string FieldName { get; set; }
        public Type FieldType { get; set; }
        public int Min { get; set; }
        public int Max { get; set; }

    }
}
