using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiftLog.Data.Tests.Utility
{
    public class DummyRepository : GenericRepository<Dummy>
    {
        public DummyRepository(Context context) : base(context)
        {
        }
    }
}

