using System;

namespace DatabaseComparer
{
    [Serializable]
    public class ForeignKey: SysObject
    {
        public override string Type => "F";
    }
}
