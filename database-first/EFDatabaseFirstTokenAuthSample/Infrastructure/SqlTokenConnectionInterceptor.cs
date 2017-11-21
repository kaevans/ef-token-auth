﻿using System.Data;
using System.Data.Common;
using System.Data.Entity.Infrastructure.Interception;
using System.Data.SqlClient;

namespace EFDatabaseFirstTokenAuthSample.Infrastructure
{
    /// <summary>
    /// Intercepts the database connection opening to add the access token. 
    /// This removes the need to set the token in every calling method.
    /// </summary>
    internal class SqlTokenConnectionInterceptor : IDbConnectionInterceptor
    {

        public void Opening(DbConnection connection, DbConnectionInterceptionContext interceptionContext)
        {
            var cn = connection as SqlConnection;
            if (null == cn)
            {
                throw new System.ArgumentException("Only SQL connections are supported");
            }

            //Assume all contexts are for the current user. Should only have one context here
            //unless multiple contexts were created with the same objectcontext. Even then, this
            //should be a valid assumption given the OpenId Connect flow requires a single user.
            foreach (var ctx in interceptionContext.DbContexts)
            {
                var tokenContext = ctx as ITokenContext;
                if (null != tokenContext)
                {
                    cn.AccessToken = tokenContext.AccessToken;
                }
            }

        }



        #region NotImplemented
        public void BeganTransaction(DbConnection connection, BeginTransactionInterceptionContext interceptionContext)
        {

        }

        public void Opened(DbConnection connection, DbConnectionInterceptionContext interceptionContext)
        {


        }

        public void BeginningTransaction(DbConnection connection, BeginTransactionInterceptionContext interceptionContext)
        {

        }

        public void Closed(DbConnection connection, DbConnectionInterceptionContext interceptionContext)
        {

        }

        public void Closing(DbConnection connection, DbConnectionInterceptionContext interceptionContext)
        {

        }

        public void ConnectionStringGetting(DbConnection connection, DbConnectionInterceptionContext<string> interceptionContext)
        {

        }

        public void ConnectionStringGot(DbConnection connection, DbConnectionInterceptionContext<string> interceptionContext)
        {

        }

        public void ConnectionStringSet(DbConnection connection, DbConnectionPropertyInterceptionContext<string> interceptionContext)
        {

        }

        public void ConnectionStringSetting(DbConnection connection, DbConnectionPropertyInterceptionContext<string> interceptionContext)
        {

        }

        public void ConnectionTimeoutGetting(DbConnection connection, DbConnectionInterceptionContext<int> interceptionContext)
        {

        }

        public void ConnectionTimeoutGot(DbConnection connection, DbConnectionInterceptionContext<int> interceptionContext)
        {

        }

        public void DatabaseGetting(DbConnection connection, DbConnectionInterceptionContext<string> interceptionContext)
        {

        }

        public void DatabaseGot(DbConnection connection, DbConnectionInterceptionContext<string> interceptionContext)
        {

        }

        public void DataSourceGetting(DbConnection connection, DbConnectionInterceptionContext<string> interceptionContext)
        {

        }

        public void DataSourceGot(DbConnection connection, DbConnectionInterceptionContext<string> interceptionContext)
        {

        }

        public void Disposed(DbConnection connection, DbConnectionInterceptionContext interceptionContext)
        {

        }

        public void Disposing(DbConnection connection, DbConnectionInterceptionContext interceptionContext)
        {

        }

        public void EnlistedTransaction(DbConnection connection, EnlistTransactionInterceptionContext interceptionContext)
        {

        }

        public void EnlistingTransaction(DbConnection connection, EnlistTransactionInterceptionContext interceptionContext)
        {

        }

        public void ServerVersionGetting(DbConnection connection, DbConnectionInterceptionContext<string> interceptionContext)
        {

        }

        public void ServerVersionGot(DbConnection connection, DbConnectionInterceptionContext<string> interceptionContext)
        {

        }



        public void StateGetting(DbConnection connection, DbConnectionInterceptionContext<ConnectionState> interceptionContext)
        {

        }

        public void StateGot(DbConnection connection, DbConnectionInterceptionContext<ConnectionState> interceptionContext)
        {

        }

        #endregion
    }
}