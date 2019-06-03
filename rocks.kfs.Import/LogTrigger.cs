// <copyright>
// Copyright 2019 by Kingdom First Solutions
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// </copyright>
//
using System;
using System.Data.SqlClient;
using System.Threading;

namespace rocks.kfs.Import
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
