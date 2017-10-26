﻿using System.Text;
using System.Web.UI;
using BaiRong.Core;
using BaiRong.Core.Tabs;
using SiteServer.BackgroundPages.Core;

namespace SiteServer.BackgroundPages.Controls
{
    public class NodeNaviTree : TabDrivenTemplatedWebControl
    {
        protected override void Render(HtmlTextWriter writer)
        {
            var builder = new StringBuilder();
            BuildNavigationTree(builder, GetTabs(), 0, true);
            writer.Write(builder);
        }

        /// <summary>
        /// Creates the markup for the current TabCollection
        /// </summary>
        /// <returns></returns>
        protected void BuildNavigationTree(StringBuilder builder, TabCollection tc, int parentsCount, bool isDisplay)
        {
            if (tc?.Tabs == null) return;

            foreach (var parent in tc.Tabs)
            {
                if (!TabManager.IsValid(parent, PermissionList)) continue;

                var tabCollection = NodeNaviTabManager.GetTabCollection(parent, PublishmentSystemId);

                var linkUrl = FormatLink(parent);
                if (!string.IsNullOrEmpty(linkUrl) && !StringUtils.EqualsIgnoreCase(linkUrl, PageUtils.UnclickedUrl))
                {
                    linkUrl = PageUtils.GetLoadingUrl(linkUrl);
                }
                var hasChildren = parent.Children != null && parent.Children.Length > 0;
                var openWindow = !hasChildren && StringUtils.EndsWithIgnoreCase(parent.Href, "main.aspx");

                var item = NavigationTreeItem.CreateNavigationBarItem(isDisplay, parent.Selected, parentsCount, hasChildren, openWindow, parent.Text, linkUrl, parent.Target, parent.Enabled, parent.IconUrl);

                builder.Append(item.GetTrHtml());

                if (tabCollection?.Tabs != null && tabCollection.Tabs.Length > 0)
                {
                    BuildNavigationTree(builder, tabCollection, parentsCount + 1, parent.Selected);
                }

                //if (parent.Children != null && parent.Children.Length > 0)
                //{
                //    var tc2 = NodeNaviTabManager.GetTabCollection(parent, PublishmentSystemId);
                //    BuildNavigationTree(builder, tc2, parentsCount + 1, parent.Selected);
                //}
            }
        }

        public int PublishmentSystemId { get; set; }

        private string _selected;
        public override string Selected
        {
            get { return _selected ?? (_selected = Context.Items["ControlPanelSelectedNavItem"] as string); }
            set
            {
                _selected = value;
            }
        }
    }
}
