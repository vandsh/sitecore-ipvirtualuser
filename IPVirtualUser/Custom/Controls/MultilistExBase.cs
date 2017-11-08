using System.Collections.Generic;
using System.Web.UI;
using Sitecore;
using Sitecore.Diagnostics;
using Sitecore.Globalization;
using Sitecore.Resources;
using Sitecore.Shell.Applications.ContentEditor;

namespace IPVirtualUser.Custom.Controls
{
    public abstract class MultilistExBase : MultilistEx
    {
        /// <summary>
        /// Return here your unselected items. First value is the ID you will store into your field, the second one is the display text.
        /// </summary>
        /// <returns>The unselected items</returns>
        protected abstract IEnumerable<KeyValuePair<string, string>> GetNonSelectedItems();

        /// <summary>
        /// Return here your selected items. First value is the ID you will store into your field, the second one is the display text.
        /// </summary>
        /// <returns>The selected items</returns>
        protected abstract IEnumerable<KeyValuePair<string, string>> GetSelectedItems();

        /// <summary>
        /// By overideing this method, you can initialise some variables here.
        /// </summary>
        protected virtual void InitRendering()
        {
        }

        protected override void DoRender(HtmlTextWriter output)
        {
            Assert.ArgumentNotNull(output, "output");

            ServerProperties["ID"] = ID;

            InitRendering();

            var text = string.Empty;
            if (ReadOnly)
            {
                text = " disabled=\"disabled\"";
            }

            output.Write(string.Concat("<input id=\"", ID, "_Value\" type=\"hidden\" value=\"", StringUtil.EscapeQuote(Value), "\" />"));
            output.Write("<table" + GetControlAttributes() + ">");
            output.Write("<tr>");
            output.Write("<td class=\"scContentControlMultilistCaption\" width=\"50%\">" + Translate.Text("All") + "</td>");
            output.Write("<td width=\"20\">" + Images.GetSpacer(20, 1) + "</td>");
            output.Write("<td class=\"scContentControlMultilistCaption\" width=\"50%\">" + Translate.Text("Selected") + "</td>");
            output.Write("<td width=\"20\">" + Images.GetSpacer(20, 1) + "</td>");
            output.Write("</tr>");
            output.Write("<tr>");
            output.Write("<td valign=\"top\" height=\"100%\">");
            output.Write(string.Concat("<select id=\"", ID, "_unselected\" class=\"scContentControlMultilistBox\" multiple=\"multiple\" size=\"10\"", text, " ondblclick=\"javascript:scContent.multilistMoveRight('", ID, "')\" onchange=\"javascript:document.getElementById('", ID, "_all_help').innerHTML=this.selectedIndex>=0?this.options[this.selectedIndex].innerHTML:''\" >"));

            // Bind the selected values?
            foreach (var field in GetNonSelectedItems())
            {
                output.Write(string.Concat("<option value=\"", field.Key, "\">", field.Value, "</option>"));
            }

            output.Write("</select>");
            output.Write("</td>");
            output.Write("<td valign=\"top\">");
            RenderButton(output, "Core/16x16/arrow_blue_right.png", "javascript:scContent.multilistMoveRight('" + ID + "')");
            output.Write("<br />");
            RenderButton(output, "Core/16x16/arrow_blue_left.png", "javascript:scContent.multilistMoveLeft('" + ID + "')");
            output.Write("</td>");
            output.Write("<td valign=\"top\" height=\"100%\">");
            output.Write(string.Concat("<select id=\"", ID, "_selected\" class=\"scContentControlMultilistBox\" multiple=\"multiple\" size=\"10\"", text, " ondblclick=\"javascript:scContent.multilistMoveLeft('", ID, "')\" onchange=\"javascript:document.getElementById('", ID, "_selected_help').innerHTML=this.selectedIndex>=0?this.options[this.selectedIndex].innerHTML:''\">"));

            // Bind the available items list
            foreach (var field in GetSelectedItems())
            {
                output.Write(string.Concat("<option value=\"", field.Key, "\">", field.Value, "</option>"));
            }

            output.Write("</select>");
            output.Write("</td>");
            output.Write("<td valign=\"top\">");
            RenderButton(output, "Core/16x16/arrow_blue_up.png", "javascript:scContent.multilistMoveUp('" + ID + "')");
            output.Write("<br />");
            RenderButton(output, "Core/16x16/arrow_blue_down.png", "javascript:scContent.multilistMoveDown('" + ID + "')");
            output.Write("</td>");
            output.Write("</tr>");
            output.Write("<tr>");
            output.Write("<td valign=\"top\">");
            output.Write("<div style=\"border:1px solid #999999;font:8pt tahoma;padding:2px;margin:4px 0px 4px 0px;height:14px\" id=\"" + ID + "_all_help\"></div>");
            output.Write("</td>");
            output.Write("<td></td>");
            output.Write("<td valign=\"top\">");
            output.Write("<div style=\"border:1px solid #999999;font:8pt tahoma;padding:2px;margin:4px 0px 4px 0px;height:14px\" id=\"" + ID + "_selected_help\"></div>");
            output.Write("</td>");
            output.Write("<td></td>");
            output.Write("</tr>");
            output.Write("</table>");
        }        

        private void RenderButton(HtmlTextWriter output, string icon, string click)
        {
            Assert.ArgumentNotNull(output, "output");
            Assert.ArgumentNotNull(icon, "icon");
            Assert.ArgumentNotNull(click, "click");

            var imageBuilder = new ImageBuilder
            {
                Src = icon,
                Width = 16,
                Height = 16,
                Margin = "2px"
            };

            if (!ReadOnly)
            {
                imageBuilder.OnClick = click;
            }

            output.Write(imageBuilder.ToString());
        }
    }
}