namespace Hangfire.Dashboard.BasicAuthorization
{
    using Hangfire.Dashboard;
    using Microsoft.AspNetCore.Http;
    using System;
    using System.Linq;
    using System.Net.Http.Headers;
    using System.Text;

    public class BasicAuthAuthorizationFilter : IDashboardAuthorizationFilter
    {
        private readonly BasicAuthAuthorizationFilterOptions _options;

        public BasicAuthAuthorizationFilter() : this(new BasicAuthAuthorizationFilterOptions())
        {
        }

        public BasicAuthAuthorizationFilter(BasicAuthAuthorizationFilterOptions options)
        {
            _options = options;
        }

        public bool Authorize(DashboardContext _context)
        {
            HttpContext httpContext = AspNetCoreDashboardContextExtensions.GetHttpContext(_context);
            if (_options.SslRedirect && (httpContext.Request.Scheme != "https"))
            {
                string str2 = new UriBuilder("https", httpContext.Request.Host.ToString(), 0x1bb, httpContext.Request.Path).ToString();
                httpContext.Response.StatusCode = 0x12d;
                httpContext.Response.Redirect(str2);
                return false;
            }
            if (_options.RequireSsl && !httpContext.Request.IsHttps)
            {
                return false;
            }
            string str = httpContext.Request.Headers["Authorization"];
            if (!string.IsNullOrWhiteSpace(str))
            {
                AuthenticationHeaderValue value2 = AuthenticationHeaderValue.Parse(str);
                if ("Basic".Equals(value2.Scheme, StringComparison.OrdinalIgnoreCase))
                {
                    char[] separator = new char[] { ':' };
                    string[] strArray = Encoding.UTF8.GetString(Convert.FromBase64String(value2.Parameter)).Split(separator);
                    if (strArray.Length > 1)
                    {
                        string login = strArray[0];
                        string password = strArray[1];
                        if (!string.IsNullOrWhiteSpace(login) && !string.IsNullOrWhiteSpace(password))
                        {
                            if (!Enumerable.Any(_options.Users, delegate (BasicAuthAuthorizationUser user)
                            {
                                return user.Validate(login, password, _options.LoginCaseSensitive);
                            }))
                            {
                                return Challenge(httpContext);
                            }
                            return true;
                        }
                    }
                }
            }
            return Challenge(httpContext);
        }

        private bool Challenge(HttpContext context)
        {
            context.Response.StatusCode=0x191;
            HeaderDictionaryExtensions.Append(context.Response.Headers, "WWW-Authenticate", "Basic realm=\"Hangfire Dashboard\"");
            return false;
        }
    }
}

