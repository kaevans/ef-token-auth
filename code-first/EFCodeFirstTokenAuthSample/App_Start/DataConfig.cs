using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using EFCodeFirstTokenAuthSample.Infrastructure;
using EFCodeFirstTokenAuthSample.Models;

namespace EFCodeFirstTokenAuthSample
{
    public class DataConfig
    {
        public static void RegisterDataInitializers()
        {
            //If this is dev/test, you can use the EFADALInitializer to drop/create
            //the database based on model changes. Don't do this in production.
            //Database.SetInitializer(new EFADALInitializer());


            //Using this method, the database is assumed to already exist.
            Database.SetInitializer<ADALContext>(null);
        }
    }
}