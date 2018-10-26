using System;

namespace DatabaseComparer
{
    [Serializable]
    public class ColumnInfo
    {
        public string TableCatalog { get; set; }
        public string TableSchema { get; set; }
        public string TableName { get; set; }
        public string ColumnName { get; set; }
        public string ColumnDefault { get; set; }
        public string IsNullable { get; set; }
        public string DataType { get; set; }
        public int? CharacterMaximumLength { get; set; }
        public byte? NumericPrecision { get; set; }
        public int? NumericScale { get; set; }
        public short? DateTimePrecision { get; set; }
        public string CollationName { get; set; }
        public override string ToString()
        {
            return $"TableCatalog:'{TableCatalog}';"
                  + $"TableSchema:'{TableSchema}';"
                  + $"TableName:'{TableName}';"
                  + $"ColumnName:'{ColumnName}';"
                  + $"ColumnDefault:'{ColumnDefault}';"
                  + $"IsNullable:'{IsNullable}';"
                  + $"DataType:'{DataType}';"
                  + $"CharacterMaximumLength:'{CharacterMaximumLength}';"
                  + $"NumericPrecision:'{NumericPrecision}';"
                  + $"NumericScale:'{NumericScale}';"
                  + $"DateTimePrecision:'{DateTimePrecision}';"
                  + $"CollationName:'{CollationName}';";
                  }
        public static bool operator ==(ColumnInfo ciOne, ColumnInfo ciTwo)
        {
            return (ciOne.TableName == ciTwo.TableName)
                && (ciOne.ColumnName == ciTwo.ColumnName)
                && (ciOne.ColumnDefault == ciTwo.ColumnDefault)
                && (ciOne.IsNullable == ciTwo.IsNullable)
                && (ciOne.DataType == ciTwo.DataType)
                && (ciOne.CharacterMaximumLength == ciTwo.CharacterMaximumLength)
                && (ciOne.NumericPrecision == ciTwo.NumericPrecision)
                && (ciOne.NumericScale == ciTwo.NumericScale)
                && (ciOne.DateTimePrecision == ciTwo.DateTimePrecision)
                && (ciOne.CollationName == ciTwo.CollationName);
        }
        public static bool operator !=(ColumnInfo ciOne, ColumnInfo ciTwo)
        {
            return !(ciOne == ciTwo);
        }

    }
}
