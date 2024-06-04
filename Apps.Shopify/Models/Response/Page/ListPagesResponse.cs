using Blackbird.Applications.Sdk.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apps.Shopify.Models.Response.Page
{
    public class ListPagesResponse
    {
        public IEnumerable<Page> Pages { get; set; }
    }

    public class Page
    {
        [Display("Resource ID")]
        public string ResourceId { get; set; }

        public string Title { get; set; }
    }
}
