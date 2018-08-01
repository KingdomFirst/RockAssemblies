using System;
using System.Data.SqlClient;
using System.Threading;

namespace com.kfs.Import
{
    #region Async Triggers

    /// <summary>
    /// Async Trigger to log progress of SQL procedures
    /// </summary>
    /// <seealso cref="System.IDisposable" />
    public class LogTrigger : System.Object, IDisposable
    {
        public SqlCommand command;
        public SqlConnection connection;
        public bool isExecuting = true;

        protected Thread bgThread;

        public LogTrigger()
        {
        }

        public LogTrigger( SqlConnection conn, SqlCommand comm )
        {
            connection = conn;
            command = comm;
        }

        public delegate void InfoMessage( object sender, string Message );

        public event InfoMessage LogEvent;

        public void Dispose()
        {
            try
            {
                if ( connection != null ) connection.Dispose();
                if ( command != null ) command.Dispose();
            }
            catch { }
        }

        public void ExecSql()
        {
            if ( connection == null || command == null )
            {
                return;
            }

            if ( LogEvent != null )
            {
                connection.FireInfoMessageEventOnUserErrors = true;
                connection.InfoMessage += new SqlInfoMessageEventHandler( SqlEventTrigger );
            }

            connection.Open();
            command.Connection = connection;
            command.CommandTimeout = 0;
            command.ExecuteNonQuery();
            connection.Close();

            isExecuting = false;
        }

        public void SqlEventTrigger( object sender, SqlInfoMessageEventArgs e )
        {
            LogEvent?.Invoke( sender, e.Message );
        }

        public void Join()
        {
            if ( bgThread != null && bgThread.IsAlive )
            {
                bgThread.Join();
            }
        }

        public void Start()
        {
            bgThread = new Thread( new ThreadStart( ExecSql ) );
            bgThread.Start();
        }

        public void Stop()
        {
            if ( bgThread != null && bgThread.IsAlive )
            {
                bgThread.Abort();
            }

            Join();
        }
    }

    #endregion
}
