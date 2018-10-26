using System;

namespace DatabaseComparer
{
    [Serializable]
    public class TableInfo
    {
        public string Catalog { get; set; }
        public string Schema { get; set; }
        public string Name { get; set; }
        public string TableType { get; set; }
        public override string ToString()
        {
            return $"Catalog:'{Catalog}';Schema:'{Schema}';Name:'{Name}';Type:'{TableType}';";
        }
        public static bool operator ==(TableInfo tiOne, TableInfo tiTwo)
        {
            return (tiOne.Name == tiTwo.Name) && (tiOne.TableType == tiTwo.TableType);
        }
        public static bool operator !=(TableInfo tiOne, TableInfo tiTwo)
        {
            return !(tiOne == tiTwo);
        }
    }
}
