using System;
using System.Data.SQLite;

namespace Sakuno.KanColle.Amatsukaze
{
    public static class SQLiteDataReaderExtensions
    {
        public static bool GetBoolean(this SQLiteDataReader rpReader, string rpColumn)
        {
            var rResult = rpReader[rpColumn];

            if (rResult == DBNull.Value)
                return false;

            return Convert.ToBoolean(rResult);
        }
        public static int GetInt32(this SQLiteDataReader rpReader, string rpColumn)
        {
            var rResult = rpReader[rpColumn];

            if (rResult == DBNull.Value)
                return 0;

            return Convert.ToInt32(rResult);
        }
        public static long GetInt64(this SQLiteDataReader rpReader, string rpColumn)
        {
            var rResult = rpReader[rpColumn];

            if (rResult == DBNull.Value)
                return 0L;

            return (long)rResult;
        }
        public static double GetDouble(this SQLiteDataReader rpReader, string rpColumn)
        {
            var rResult = rpReader[rpColumn];

            if (rResult == DBNull.Value)
                return .0;

            return (double)rResult;
        }
        public static bool? GetBooleanOptional(this SQLiteDataReader rpReader, string rpColumn)
        {
            var rResult = rpReader[rpColumn];

            if (rResult == DBNull.Value)
                return null;

            return Convert.ToBoolean(rResult);
        }
        public static int? GetInt32Optional(this SQLiteDataReader rpReader, string rpColumn)
        {
            var rResult = rpReader[rpColumn];

            if (rResult == DBNull.Value)
                return null;

            return Convert.ToInt32(rResult);
        }
        public static long? GetInt64Optional(this SQLiteDataReader rpReader, string rpColumn)
        {
            var rResult = rpReader[rpColumn];

            if (rResult == DBNull.Value)
                return null;

            return (long)rResult;
        }
        public static double? GetDoubleOptional(this SQLiteDataReader rpReader, string rpColumn)
        {
            var rResult = rpReader[rpColumn];

            if (rResult == DBNull.Value)
                return null;

            return (double)rResult;
        }

        public static string GetString(this SQLiteDataReader rpReader, string rpColumn)
        {
            var rResult = rpReader[rpColumn];

            if (rResult == DBNull.Value)
                return null;

            return (string)rResult;
        }
    }
}
