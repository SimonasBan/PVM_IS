using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace IS_Turizmas.Helpers.Tag
{
    [HtmlTargetElement(Attributes = ControllersAttributeName)]
    [HtmlTargetElement(Attributes = ActionsAttributeName)]
    [HtmlTargetElement(Attributes = RouteAttributeName)]
    [HtmlTargetElement(Attributes = ClassAttributeName)]
    public class ActiveLinkTagHelper : TagHelper
    {
        private const string ActionsAttributeName = "asp-active-actions";
        private const string ControllersAttributeName = "asp-active-controllers";
        private const string ClassAttributeName = "asp-active-class";
        private const string RouteAttributeName = "asp-active-route";


        [HtmlAttributeName(ControllersAttributeName)]
        public string Controllers { get; set; }

        [HtmlAttributeName(ActionsAttributeName)]
        public string Actions { get; set; }

        [HtmlAttributeName(RouteAttributeName)]
        public string Route { get; set; }

        [HtmlAttributeName(ClassAttributeName)]
        public string Class { get; set; } = "active";

        [HtmlAttributeNotBound]
        [ViewContext]
        public ViewContext ViewContext { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            RouteValueDictionary routeValues = ViewContext.RouteData.Values;
            string currentAction = routeValues["action"].ToString();
            string currentController = routeValues["controller"].ToString();


            if (string.IsNullOrEmpty(Actions))
                Actions = currentAction;

            if (string.IsNullOrEmpty(Controllers))
                Controllers = currentController;

            Actions = Regex.Replace(Actions, @"\s+", "");
            Controllers = Regex.Replace(Controllers, @"\s+", "");

            string[] acceptedActions = Actions.Split(',').Distinct().ToArray();
            string[] acceptedControllers = Controllers.Split(',').Distinct().ToArray();


            var lcComparer = new LowerCaseComparer();

            if (acceptedActions.Contains(currentAction, lcComparer) && acceptedControllers.Contains(currentController, lcComparer))
            {
                SetAttribute(output, "class", Class);
            }

            base.Process(context, output);
        }

        private void SetAttribute(TagHelperOutput output, string attributeName, string value, bool merge = true)
        {
            var v = value;
            TagHelperAttribute attribute;
            if (output.Attributes.TryGetAttribute(attributeName, out attribute))
            {
                if (merge)
                {
                    v = $"{attribute.Value} {value}";
                }
            }
            output.Attributes.SetAttribute(attributeName, v);
        }

    }
    public class LowerCaseComparer : IEqualityComparer<string>
    {
        public bool Equals(string x, string y)
        {
            return x.ToLowerInvariant().Equals(y.ToLowerInvariant());
        }

        public int GetHashCode(string obj)
        {
            return obj.GetHashCode();
        }
    }
}
