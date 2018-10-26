using System;

namespace DatabaseComparer
{
    [Serializable]
    public class Trigger : SysObject
    {
        public override string Type => "TR";

        public static bool operator ==(Trigger tiOne, Trigger tiTwo)
        {
            return (tiOne.Name == tiTwo.Name) && (tiOne.Type == tiTwo.Type);
        }
        public static bool operator !=(Trigger tiOne, Trigger tiTwo)
        {
            return !(tiOne == tiTwo);
        }

    }

}
