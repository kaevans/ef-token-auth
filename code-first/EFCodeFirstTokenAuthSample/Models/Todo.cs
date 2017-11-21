using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EFCodeFirstTokenAuthSample.Models
{
    public class Todo
    {
        public int ID { get; set; }
        public string Owner { get; set; }
        public string Description { get; set; }
    }
}