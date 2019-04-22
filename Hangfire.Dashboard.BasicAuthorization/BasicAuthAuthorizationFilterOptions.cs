namespace Hangfire.Dashboard.BasicAuthorization
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;

    public class BasicAuthAuthorizationFilterOptions
    {
        public BasicAuthAuthorizationFilterOptions()
        {
            SslRedirect = true;
            RequireSsl = true;
            LoginCaseSensitive = true;
            Users = new BasicAuthAuthorizationUser[0];
        }

        public bool LoginCaseSensitive { get; set; }

        public bool RequireSsl { get; set; }

        public bool SslRedirect { get; set; }

        public IEnumerable<BasicAuthAuthorizationUser> Users { get; set; }
    }
}

