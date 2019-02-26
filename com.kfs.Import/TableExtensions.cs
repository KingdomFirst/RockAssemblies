using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using CsvHelper;
using OfficeOpenXml;

namespace com.kfs.Import
{
    #region Spreadsheet extensions

    public static class TableExtensions
    {
        /// <summary>
        /// Transforms the CSV into a table.
        /// </summary>
        /// <param name="csv">The CSV.</param>
        /// <returns></returns>
        public static DataTable TransformTable( this CsvReader csv )
        {
            var table = new DataTable();

            csv.Read();
            foreach ( var header in csv.FieldHeaders.Where( h => !string.IsNullOrWhiteSpace( h ) ) )
            {
                table.Columns.Add( header );
            }

            do
            {
                var row = table.NewRow();
                foreach ( DataColumn column in table.Columns )
                {
                    row[column.ColumnName] = csv.GetField( column.DataType, column.ColumnName );
                    if ( column.DataType == typeof( string ) )
                    {
                        column.MaxLength = Math.Max( column.MaxLength, row[column.ColumnName].ToString().Length );
                    }
                }

                table.Rows.Add( row );
            }
            while ( csv.Read() );

            return table;
        }

        /// <summary>
        /// Transforms the Excel file into a table.
        /// </summary>
        /// <param name="package">The package.</param>
        /// <returns></returns>
        public static DataTable TransformTable( this ExcelPackage package )
        {
            if ( !package.Workbook.Worksheets.Any() )
            {
                return null;
            }

            var i = 0;
            var table = new DataTable();
            var skippedColumns = new List<int>();
            var sheet = package.Workbook.Worksheets.First();
            foreach ( var headers in sheet.Cells[1, 1, 1, sheet.Dimension.End.Column] )
            {
                if ( !string.IsNullOrWhiteSpace( headers.Text ) )
                {
                    table.Columns.Add( headers.Text );
                }
                else
                {
                    skippedColumns.Add( i );
                }

                i++;
            }

            for ( var currentRow = 2; currentRow <= sheet.Dimension.End.Row; currentRow++ )
            {
                var numSkippedColumns = 0;
                var row = sheet.Cells[currentRow, 1, currentRow, sheet.Dimension.End.Column];
                var newRow = table.NewRow();
                foreach ( var cell in row )
                {
                    var spreadsheetIndex = cell.Start.Column - 1;
                    if ( !skippedColumns.Contains( spreadsheetIndex ) )
                    {
                        var importIndex = spreadsheetIndex - numSkippedColumns;
                        newRow[importIndex] = cell.Text;
                        table.Columns[importIndex].MaxLength = Math.Max( cell.Text.Length, table.Columns[importIndex].MaxLength );
                    }
                    else
                    {
                        numSkippedColumns++;
                    }
                }

                table.Rows.Add( newRow );
            }

            return table;
        }

        /// <summary>
        /// Uploads the table to SQL.
        /// </summary>
        /// <param name="errors">The errors.</param>
        /// <param name="tableToUpload">The table to upload.</param>
        /// <returns></returns>
        public static string UploadTable( string errors, DataTable tableToUpload )
        {
            using ( var bulkCopy = new SqlBulkCopy( GetConnectionString() ) )
            {
                bulkCopy.DestinationTableName = string.Format( "[dbo].[{0}]", tableToUpload.TableName );
                try
                {
                    foreach ( var column in tableToUpload.Columns )
                    {
                        // use the original column to map headers, but trim the header for SQL
                        bulkCopy.ColumnMappings.Add( column.ToString(), StripTableName( column.ToString() ) );
                    }
                    bulkCopy.WriteToServer( tableToUpload );
                }
                catch ( System.Exception ex )
                {
                    errors = ex.Message;
                }
            }

            return errors;
        }

        /// <summary>
        /// Removes special characters from the string so that only Alpha, Numeric, '.' and '_' remain;
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static string StripTableName( string value )
        {
            var sb = new StringBuilder();
            foreach ( char c in value )
            {
                if ( ( c >= '0' && c <= '9' ) || ( c >= 'A' && c <= 'Z' ) || ( c >= 'a' && c <= 'z' ) || c == '.' || c == '_' )
                {
                    sb.Append( c );
                }
            }
            return sb.ToString();
        }

        /// <summary>
        /// Gets the column type for SQL
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="columnSize">Size of the column.</param>
        /// <param name="numericPrecision">The numeric precision.</param>
        /// <param name="numericScale">The numeric scale.</param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static string GetSQLType( object type, int columnSize, int numericPrecision, int numericScale )
        {
            switch ( type.ToString() )
            {
                case "System.String":
                    return "NVARCHAR(" + ( ( columnSize <= 0 ) ? 255 : columnSize ) + ")";

                case "System.Decimal":
                    if ( numericScale > 0 )
                        return "REAL";
                    else if ( numericPrecision > 10 )
                        return "BIGINT";
                    else
                        return "INT";

                case "System.Double":
                case "System.Single":
                    return "REAL";

                case "System.Int64":
                    return "BIGINT";

                case "System.Int16":
                case "System.Int32":
                    return "INT";

                case "System.DateTime":
                    return "DATETIME";

                default:
                    throw new Exception( type.ToString() + " not implemented." );
            }
        }

        /// <summary>
        /// SQLs the type of the get.
        /// </summary>
        /// <param name="schemaRow">The schema row.</param>
        /// <returns></returns>
        public static string GetSQLType( DataRow schemaRow )
        {
            return GetSQLType( schemaRow["DataType"],
                int.Parse( schemaRow["ColumnSize"].ToString() ),
                int.Parse( schemaRow["NumericPrecision"].ToString() ),
                int.Parse( schemaRow["NumericScale"].ToString() ) );
        }

        // Overload based on DataColumn from DataTable type
        /// <summary>
        /// SQLs the type of the get.
        /// </summary>
        /// <param name="column">The column.</param>
        /// <returns></returns>
        public static string GetSQLType( DataColumn column )
        {
            return GetSQLType( column.DataType, column.MaxLength, 10, 2 );
        }

        /// <summary>
        /// Gets the connection string.
        /// </summary>
        /// <returns></returns>
        public static string GetConnectionString()
        {
            var builder = new SqlConnectionStringBuilder( ConfigurationManager.ConnectionStrings["RockContext"].ConnectionString )
            {
                AsynchronousProcessing = true,
                MultipleActiveResultSets = true,
                ConnectTimeout = 5
            };

            return builder.ConnectionString;
        }
    }

    #endregion
}
