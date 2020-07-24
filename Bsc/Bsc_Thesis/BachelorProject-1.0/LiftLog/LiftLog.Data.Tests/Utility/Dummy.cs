using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiftLog.Data.Interfaces;

namespace LiftLog.Data.Tests.Utility
{
    public class Dummy : IEntity
    {
        public int Id { get; set; }

        public int DummyVariable { get; set; }
    }
}
