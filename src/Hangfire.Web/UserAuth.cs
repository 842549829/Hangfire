using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Holder.ERP.Job.Web
{
    public class UserAuth
    {
        public string UserName { get; set; }

        public string Password { get; set; }
    }

    public class HangfireOption
    {
        public IEnumerable<UserAuth> Users { get; set; }
    }
}
