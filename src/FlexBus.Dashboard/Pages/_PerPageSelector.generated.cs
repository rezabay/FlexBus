﻿#pragma warning disable 1591
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace FlexBus.Dashboard.Pages
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    
    #line 2 "..\..\Pages\_PerPageSelector.cshtml"
    using FlexBus.Dashboard.Resources;
    
    #line default
    #line hidden
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("RazorGenerator", "2.0.0.0")]
    internal partial class PerPageSelector : FlexBus.Dashboard.RazorPage
    {
#line hidden

        public override void Execute()
        {


WriteLiteral("\r\n");



WriteLiteral("\r\n<div class=\"btn-group pull-right paginator\">\r\n");


            
            #line 6 "..\..\Pages\_PerPageSelector.cshtml"
     foreach (var count in new[] {10, 20, 50, 100, 500})
    {

            
            #line default
            #line hidden
WriteLiteral("        <a class=\"btn btn-sm btn-default ");


            
            #line 8 "..\..\Pages\_PerPageSelector.cshtml"
                                     Write(count == _pager.RecordsPerPage ? "active" : null);

            
            #line default
            #line hidden
WriteLiteral("\"\r\n           href=\"");


            
            #line 9 "..\..\Pages\_PerPageSelector.cshtml"
            Write(_pager.RecordsPerPageUrl(count));

            
            #line default
            #line hidden
WriteLiteral("\">\r\n            ");


            
            #line 10 "..\..\Pages\_PerPageSelector.cshtml"
       Write(count);

            
            #line default
            #line hidden
WriteLiteral("</a>\r\n");


            
            #line 11 "..\..\Pages\_PerPageSelector.cshtml"
    }

            
            #line default
            #line hidden
WriteLiteral("</div>\r\n<div class=\"btn-toolbar-spacer pull-right\"></div>\r\n<div class=\"btn-toolba" +
"r-label btn-toolbar-label-sm pull-right\">\r\n    ");


            
            #line 15 "..\..\Pages\_PerPageSelector.cshtml"
Write(Strings.PerPageSelector_ItemsPerPage);

            
            #line default
            #line hidden
WriteLiteral(":\r\n</div>");


        }
    }
}
#pragma warning restore 1591