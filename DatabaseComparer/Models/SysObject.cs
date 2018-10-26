using System;

namespace DatabaseComparer
{
    [Serializable]
    public abstract class SysObject
    {     
        public string Name { get; set; }
        public abstract string Type { get;  }
        public string Definition { get; set; }

        public override string ToString()
        {
            return $"Name:'{Name}';Type:'{Type}';Definition:'{Definition}';";
        }        
    }
}
