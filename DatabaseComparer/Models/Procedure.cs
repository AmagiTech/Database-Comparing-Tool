using System;

namespace DatabaseComparer
{
    [Serializable]
    public class Procedure : SysObject
    {
        public override string Type => "P";
    }
}
