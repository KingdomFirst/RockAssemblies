// <copyright>
// Copyright 2021 by Kingdom First Solutions
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// </copyright>
//
using System.Collections.Generic;
using Rock;
using Rock.Plugin;

namespace rocks.kfs.StepsToCare
{
    [MigrationNumber( 8, "1.12.3" )]
    public class LavaShortcode : Migration
    {
        public override void Up()
        {
            Sql( @"IF NOT EXISTS (SELECT Id FROM [LavaShortcode] WHERE [Guid] = 'AB79EC07-F404-43DC-8F79-9A2AF87F04C6')
BEGIN
INSERT INTO [LavaShortcode] ([Name], [Description], [Documentation], [IsSystem], [IsActive], [TagName], [Markup], [TagType], [EnabledLavaCommands], [Parameters], [Guid])
VALUES (N'KFS Note Popover', N'Use this shortcode to add person note popover/quick note icon to any entity with a person ID. Using KFS Steps to Care ""Note Templates""', N'<p><b>Usage:</b> (Adds a person note to Person Id 123, with a prefix of ""Note: ""</p>
<pre>{[ kfsnotepopover id:''123'' prefix:''Note'' ]}</pre>
<p></p>
<p><b>Parameters:</b></p><p>&nbsp; &nbsp; id: &lt;personId&gt; (required)</p>
<p>&nbsp; &nbsp; prefix: &lt;string&gt; (optional)</p>
<p>&nbsp; &nbsp; icon: &lt;font awesome icon string&gt; (default: ""fa fa-comment"")<br></p>
<p><b>Example Appearance:</b></p><p>&nbsp;&nbsp;&nbsp;&nbsp;<a href=""#""><i class=""fa fa-comment""></i></a></p>
', '0', '1', N'kfsnotepopover', N'<a tabindex=""0"" role=""button"" data-toggle=""popoverCustom"" data-id=""{{ id }}"" class=""kfs-popover""><i class=""{{ icon }}""></i></a>
{% javascript id:''kfsNotePopover'' disableanonymousfunction:''true'' %}
    $(function () {
        $(''[data-toggle=""popoverCustom""]'').each (function () {
          $(this).popover({
              //trigger: ''focus'',
              placement: ''top'',
              html:''true'',
              sanitize: false,
              content:''{% notetemplate where:''IsActive == true && Id != 6'' %}{% for template in notetemplateItems %}<a href=""#"" onclick=""addNoteAjax(\''{{ prefix }}\'',this.title,'' + $(this).data(''id'')+'');return false;"" title=""{{ template.Note }}"" class=""mx-2""><i class=""{{ template.Icon }}""></i></a>{%- endfor %}{% endnotetemplate %}''
          });
        });
    });
    
    function addNoteAjax(prefix = """", message, id) { 
        var prefixPlus = """";
        if (prefix != """") {
            prefixPlus = "": "";
        }
        $.ajax({
            type: ""POST"",
            url: ""/api/Notes"", 
            contentType:""application/json"",
            data: JSON.stringify({
                IsSystem: false,
                Text: prefix.trim()+prefixPlus+message,
                NoteTypeId: 4,
                EntityId: id,
                CreatedByPersonAliasId: {{ CurrentPerson.PrimaryAliasId }},
                ApprovalStatus: 1,
                IsAlert: false,
                Caption: """"
            }),
            success: function( data ) {
                alert( prefix+"" Added: ""+message);
            },
            error: function (e) {
                console.error(e);
            }
        });
    }
{% endjavascript %}
{% stylesheet id:''kfsNotePopoverStyles'' %}
.kfs-popover + .popover .popover-content {
    white-space: nowrap;
}
{% endstylesheet %}
', '1', N'RockEntity', N'id^|prefix^|icon^fa fa-comment', 'AB79EC07-F404-43DC-8F79-9A2AF87F04C6')
END" );
        }

        public override void Down()
        {
            Sql( @"DELETE LavaShortcode WHERE [Guid] = 'AB79EC07-F404-43DC-8F79-9A2AF87F04C6'" );
        }
    }
}                                                                                                                                                                         