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
    
    #line 2 "..\..\Pages\PublishedPage.cshtml"
    using System;
    
    #line default
    #line hidden
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    
    #line 3 "..\..\Pages\PublishedPage.cshtml"
    using FlexBus.Dashboard;
    
    #line default
    #line hidden
    
    #line 4 "..\..\Pages\PublishedPage.cshtml"
    using FlexBus.Dashboard.Pages;
    
    #line default
    #line hidden
    
    #line 5 "..\..\Pages\PublishedPage.cshtml"
    using FlexBus.Dashboard.Resources;
    
    #line default
    #line hidden
    
    #line 6 "..\..\Pages\PublishedPage.cshtml"
    using FlexBus.Messages;
    
    #line default
    #line hidden
    
    #line 7 "..\..\Pages\PublishedPage.cshtml"
    using FlexBus.Monitoring;
    
    #line default
    #line hidden
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("RazorGenerator", "2.0.0.0")]
    internal partial class PublishedPage : FlexBus.Dashboard.RazorPage
    {
#line hidden

        public override void Execute()
        {


WriteLiteral("\r\n");









            
            #line 9 "..\..\Pages\PublishedPage.cshtml"
  
    Layout = new LayoutPage(Strings.PublishedMessagesPage_Title);

    int from, perPage;

    int.TryParse(Query("from"), out from);
    int.TryParse(Query("count"), out perPage);
    var name = Query("name");
    var content = Query("content");

    var monitor = Storage.GetMonitoringApi();
    var pager = new Pager(from, perPage, GetTotal(monitor));
    var queryDto = new MessageQueryDto
    {
        MessageType = MessageType.Publish,
        Name = name,
        Content = content,
        StatusName = Name,
        CurrentPage = pager.CurrentPage - 1,
        PageSize = pager.RecordsPerPage
    };
    var succeededMessages = monitor.Messages(queryDto);


            
            #line default
            #line hidden
WriteLiteral("\r\n<div class=\"row\">\r\n    <div class=\"col-md-3\">\r\n        ");


            
            #line 35 "..\..\Pages\PublishedPage.cshtml"
   Write(Html.MessagesSidebar(MessageType.Publish));

            
            #line default
            #line hidden
WriteLiteral("\r\n    </div>\r\n    <div class=\"col-md-9\">\r\n        <h1 class=\"page-header\">");


            
            #line 38 "..\..\Pages\PublishedPage.cshtml"
                           Write(Strings.PublishedPage_Title);

            
            #line default
            #line hidden
WriteLiteral("</h1>\r\n\r\n");


            
            #line 40 "..\..\Pages\PublishedPage.cshtml"
         if (succeededMessages.Count == 0)
        {

            
            #line default
            #line hidden
WriteLiteral("            <div class=\"alert alert-info\">\r\n                ");


            
            #line 43 "..\..\Pages\PublishedPage.cshtml"
           Write(Strings.MessagesPage_NoMessages);

            
            #line default
            #line hidden
WriteLiteral("\r\n            </div>\r\n");


            
            #line 45 "..\..\Pages\PublishedPage.cshtml"
        }
        else
        {

            
            #line default
            #line hidden
WriteLiteral(@"            <div class=""js-jobs-list"">
                <div class=""btn-toolbar btn-toolbar-top"">
                    <form class=""row"">
                        <div class=""col-md-3"">
                            <input type=""text"" class=""form-control"" name=""name"" value=""");


            
            #line 52 "..\..\Pages\PublishedPage.cshtml"
                                                                                  Write(Query("name"));

            
            #line default
            #line hidden
WriteLiteral("\" placeholder=\"");


            
            #line 52 "..\..\Pages\PublishedPage.cshtml"
                                                                                                               Write(Strings.MessagesPage_Query_MessageName);

            
            #line default
            #line hidden
WriteLiteral("\"/>\r\n                        </div>\r\n                        <div class=\"col-md-5" +
"\">\r\n                            <div class=\"input-group\">\r\n                     " +
"           <input type=\"text\" class=\"form-control\" name=\"content\" value=\"");


            
            #line 56 "..\..\Pages\PublishedPage.cshtml"
                                                                                         Write(Query("content"));

            
            #line default
            #line hidden
WriteLiteral("\" placeholder=\"");


            
            #line 56 "..\..\Pages\PublishedPage.cshtml"
                                                                                                                         Write(Strings.MessagesPage_Query_MessageBody);

            
            #line default
            #line hidden
WriteLiteral("\"/>\r\n                                <span class=\"input-group-btn\">\r\n            " +
"                        <button class=\"btn btn-info\">");


            
            #line 58 "..\..\Pages\PublishedPage.cshtml"
                                                            Write(Strings.MessagesPage_Query_Button);

            
            #line default
            #line hidden
WriteLiteral(@"</button>
                                </span>
                            </div>
                        </div>
                    </form>
                </div>
                <div class=""btn-toolbar btn-toolbar-top"">
                    <button class=""js-jobs-list-command btn btn-sm btn-primary""
                            data-url=""");


            
            #line 66 "..\..\Pages\PublishedPage.cshtml"
                                 Write(Url.To("/published/requeue"));

            
            #line default
            #line hidden
WriteLiteral("\"\r\n                            data-loading-text=\"");


            
            #line 67 "..\..\Pages\PublishedPage.cshtml"
                                          Write(Strings.Common_Enqueueing);

            
            #line default
            #line hidden
WriteLiteral("\"\r\n                            disabled=\"disabled\">\r\n                        <spa" +
"n class=\"glyphicon glyphicon-repeat\"></span>\r\n                        ");


            
            #line 70 "..\..\Pages\PublishedPage.cshtml"
                   Write(Strings.Common_RequeueMessages);

            
            #line default
            #line hidden
WriteLiteral("\r\n                    </button>\r\n\r\n                    ");


            
            #line 73 "..\..\Pages\PublishedPage.cshtml"
               Write(Html.PerPageSelector(pager));

            
            #line default
            #line hidden
WriteLiteral(@"
                </div>

                <div class=""table-responsive"">
                    <table class=""table"">
                        <thead>
                        <tr>
                            <th style=""width: 30px;"">
                                <input type=""checkbox"" class=""js-jobs-list-select-all""/>
                            </th>
                            <th style=""width: 175px"">");


            
            #line 83 "..\..\Pages\PublishedPage.cshtml"
                                                Write(Strings.Common_Id);

            
            #line default
            #line hidden
WriteLiteral("</th>\r\n                            <th>");


            
            #line 84 "..\..\Pages\PublishedPage.cshtml"
                           Write(Strings.Common_Version);

            
            #line default
            #line hidden
WriteLiteral("</th>\r\n                            <th>");


            
            #line 85 "..\..\Pages\PublishedPage.cshtml"
                           Write(Strings.MessagesPage_Table_Name);

            
            #line default
            #line hidden
WriteLiteral("</th>\r\n                            <th class=\"min-width\">");


            
            #line 86 "..\..\Pages\PublishedPage.cshtml"
                                             Write(Strings.MessagesPage_Table_Retries);

            
            #line default
            #line hidden
WriteLiteral("</th>\r\n");


            
            #line 87 "..\..\Pages\PublishedPage.cshtml"
                             if (string.Equals(Name, "Processing", StringComparison.CurrentCultureIgnoreCase))
                            {

            
            #line default
            #line hidden
WriteLiteral("                                <th>");


            
            #line 89 "..\..\Pages\PublishedPage.cshtml"
                               Write(Strings.MessagesPage_Table_State);

            
            #line default
            #line hidden
WriteLiteral("</th>\r\n");


            
            #line 90 "..\..\Pages\PublishedPage.cshtml"
                            }

            
            #line default
            #line hidden
WriteLiteral("                            <th class=\"align-right\">");


            
            #line 91 "..\..\Pages\PublishedPage.cshtml"
                                               Write(Strings.MessagesPage_Table_AddedAt);

            
            #line default
            #line hidden
WriteLiteral("</th>\r\n                        </tr>\r\n                        </thead>\r\n         " +
"               <tbody>\r\n");


            
            #line 95 "..\..\Pages\PublishedPage.cshtml"
                         foreach (var message in succeededMessages)
                        {

            
            #line default
            #line hidden
WriteLiteral("                            <tr class=\"js-jobs-list-row hover\">\r\n                " +
"                <td>\r\n                                    <input type=\"checkbox\"" +
" class=\"js-jobs-list-checkbox\" name=\"messages[]\" value=\"");


            
            #line 99 "..\..\Pages\PublishedPage.cshtml"
                                                                                                             Write(message.Id);

            
            #line default
            #line hidden
WriteLiteral("\" />\r\n                                </td>\r\n                                <td " +
"class=\"word-break\">\r\n                                    <a href=\"javascript:;\" " +
"data-url=\'");


            
            #line 102 "..\..\Pages\PublishedPage.cshtml"
                                                                 Write(Url.To("/published/message/") + message.Id);

            
            #line default
            #line hidden
WriteLiteral("\' class=\"openModal\">#");


            
            #line 102 "..\..\Pages\PublishedPage.cshtml"
                                                                                                                                  Write(message.Id);

            
            #line default
            #line hidden
WriteLiteral("</a>\r\n                                </td>\r\n                                <td>" +
"\r\n                                    ");


            
            #line 105 "..\..\Pages\PublishedPage.cshtml"
                               Write(message.Version);

            
            #line default
            #line hidden
WriteLiteral("\r\n                                </td>\r\n                                <td>\r\n  " +
"                                  ");


            
            #line 108 "..\..\Pages\PublishedPage.cshtml"
                               Write(message.Name);

            
            #line default
            #line hidden
WriteLiteral("\r\n                                </td>\r\n                                <td>\r\n  " +
"                                  ");


            
            #line 111 "..\..\Pages\PublishedPage.cshtml"
                               Write(message.Retries);

            
            #line default
            #line hidden
WriteLiteral("\r\n                                </td>\r\n");


            
            #line 113 "..\..\Pages\PublishedPage.cshtml"
                                 if (string.Equals(Name, "Processing", StringComparison.CurrentCultureIgnoreCase))
                                {

            
            #line default
            #line hidden
WriteLiteral("                                    <td>\r\n                                       " +
" ");


            
            #line 116 "..\..\Pages\PublishedPage.cshtml"
                                   Write(message.StatusName);

            
            #line default
            #line hidden
WriteLiteral("\r\n                                    </td>\r\n");


            
            #line 118 "..\..\Pages\PublishedPage.cshtml"
                                }

            
            #line default
            #line hidden
WriteLiteral("                                <td class=\"align-right\">\r\n                       " +
"             ");


            
            #line 120 "..\..\Pages\PublishedPage.cshtml"
                               Write(Html.RelativeTime(message.Added));

            
            #line default
            #line hidden
WriteLiteral("\r\n                                </td>\r\n\r\n                            </tr>\r\n");


            
            #line 124 "..\..\Pages\PublishedPage.cshtml"
                        }

            
            #line default
            #line hidden
WriteLiteral("                        </tbody>\r\n                    </table>\r\n                <" +
"/div>\r\n                ");


            
            #line 128 "..\..\Pages\PublishedPage.cshtml"
           Write(Html.Paginator(pager));

            
            #line default
            #line hidden
WriteLiteral("\r\n            </div>\r\n");


            
            #line 130 "..\..\Pages\PublishedPage.cshtml"


            
            #line default
            #line hidden
WriteLiteral(@"            <div>
                <div class=""modal fade"" tabindex=""-1"" role=""dialog"">
                    <div class=""modal-dialog"" role=""document"">
                        <div class=""modal-content"">
                            <div class=""modal-header"">
                                <button type=""button"" class=""close"" data-dismiss=""modal"" aria-label=""Close"">
                                    <span aria-hidden=""true"">&times;</span>
                                </button>
                                <h4 class=""modal-title"">Message Content</h4>
                            </div>
                            <div id=""jsonContent"" style=""max-height: 500px; overflow-y: auto;"" class=""modal-body"">
                            </div>
                            <div class=""modal-footer"">
                                <button type=""button"" class=""btn btn-sm btn-primary"" id=""formatBtn"" onclick="""">");


            
            #line 144 "..\..\Pages\PublishedPage.cshtml"
                                                                                                          Write(Strings.MessagesPage_Modal_Format);

            
            #line default
            #line hidden
WriteLiteral("</button>\r\n                                <button type=\"button\" class=\"btn btn-s" +
"m btn-primary\" id=\"rawBtn\" onclick=\"\">");


            
            #line 145 "..\..\Pages\PublishedPage.cshtml"
                                                                                                       Write(Strings.MessagesPage_Modal_Raw);

            
            #line default
            #line hidden
WriteLiteral("</button>\r\n                                <button type=\"button\" class=\"btn btn-s" +
"m btn-primary\" id=\"expandBtn\" onclick=\"\">");


            
            #line 146 "..\..\Pages\PublishedPage.cshtml"
                                                                                                          Write(Strings.MessagesPage_Modal_Expand);

            
            #line default
            #line hidden
WriteLiteral("</button>\r\n                                <button type=\"button\" class=\"btn btn-s" +
"m btn-primary\" id=\"collapseBtn\" onclick=\"\">");


            
            #line 147 "..\..\Pages\PublishedPage.cshtml"
                                                                                                            Write(Strings.MessagesPage_Model_Collaspse);

            
            #line default
            #line hidden
WriteLiteral("</button>\r\n                            </div>\r\n                        </div><!--" +
" /.modal-content -->\r\n                    </div><!-- /.modal-dialog -->\r\n       " +
"         </div><!-- /.modal -->\r\n            </div>\r\n");


            
            #line 153 "..\..\Pages\PublishedPage.cshtml"
        }

            
            #line default
            #line hidden
WriteLiteral("    </div>\r\n</div>");


        }
    }
}
#pragma warning restore 1591