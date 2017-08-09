using Sitecore.Analytics.Pipelines.CreateVisits;
using Sitecore.Analytics.Pipelines.ParseReferrer;
using Sitecore.Analytics.Tracking;
using Sitecore.Diagnostics;
using System;
using System.Web;

namespace Sitecore.Support.Analytics.Pipelines.CreateVisits
{
    public class ParseReferrer : CreateVisitProcessor
    {
        public override void Process(CreateVisitArgs args)
        {
            Assert.ArgumentNotNull(args, "args");
            this.Parse(args.Request, args.Interaction);
        }

        private void Parse(HttpRequestBase request, CurrentInteraction visit)
        {
            Assert.ArgumentNotNull(request, "request");
            Uri uri;
            try
            {
                uri = request.UrlReferrer;
            }
            catch
            {
                Log.Warn(string.Concat(new object[]
                {
                    "Visit ",
                    visit.InteractionId,
                    ": referrer could not be parsed (",
                    request.Headers["Referer"],
                    ")"
                }), this);
                uri = null;
            }
            if (uri == null)
            {
                visit.Keywords = string.Empty;
                visit.ReferringSite = string.Empty;
                visit.Referrer = string.Empty;
                return;
            }

            #region Added code
            if (uri.Host == HttpContext.Current.Request.Url.Host.ToString())
            {
                visit.Keywords = string.Empty;
                visit.ReferringSite = string.Empty;
                visit.Referrer = string.Empty;
                return;
            }
            #endregion

            visit.ReferringSite = uri.Host;
            visit.Referrer = uri.ToString();
            ParseReferrerPipeline.Run(new ParseReferrerArgs
            {
                UrlReferrer = uri,
                Interaction = visit
            });
            if (visit.Keywords == null)
            {
                visit.Keywords = string.Empty;
            }
        }

    }
}