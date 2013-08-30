using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace ccbs.Models
{

    public static class MyRoles
    {
        public const string coworker = "Coworker";
        public const string baikeEditor = "baikeEditor";
        public const string baikeQandA = "baikeQandA";
    }

    public class TreeNodeType
    {
        public const int DirectoryItem = 0;
        public const int ArticleItem = 1;
    }

    public class TreeViewModel
    {
        public int Type { get; set; }
        public int Id { get; set; }
        public string Title { get; set; }
        public string ImageUrl { get; set; }
        public int Number { get; set; }
        public DateTime LastUpdate { get; set; }

        public List<TreeViewModel> SubNodes;
        public TreeViewModel TopNode;

        public const int SHOW_PUBLISHED_ARTICLE = 0;
        public const int SHOW_ALL_ARTICLE = 1;

        public TreeViewModel()
        {
        }

        public static TreeViewModel BuildTreeViewModel(Article article, int showMode)
        {

            if (article == null)
            {
                return null;
            }
            if (showMode == SHOW_PUBLISHED_ARTICLE && article.Published == false)
            {
                return null;
            }

            var treeNode = new TreeViewModel();
            treeNode.Type = TreeNodeType.ArticleItem;
            treeNode.Id = article.Id;
            treeNode.Title = article.Title;
            treeNode.Number = article.Number;
            treeNode.LastUpdate = article.LastUpdate;
            treeNode.SubNodes = null;

            if (article.Published == true)
            {
                treeNode.ImageUrl = "~/Content/oceania/images/text-file-icon.png";
            }
            else
            {
                treeNode.ImageUrl = "~/Content/oceania/images/draft.png";
            }

            return treeNode;
        }

        public static TreeViewModel BuildTreeViewModel(SubDirectory root, int showMode)
        {
            if (root == null)
            {
                return null;
            }
            var treeNode = new TreeViewModel();
            treeNode.Type = TreeNodeType.DirectoryItem;
            treeNode.Id = root.Id;
            treeNode.Title = root.Name;
            treeNode.ImageUrl = "~/Content/oceania/images/folder.png";
            treeNode.LastUpdate = root.LastUpdate;
            treeNode.SubNodes = new List<TreeViewModel>();

            foreach (var dir in root.SubDirectories.OrderBy(s => s.Number))
            {
                var dirNode = BuildTreeViewModel(dir, showMode);
                if (dirNode != null)
                {
                    treeNode.SubNodes.Add(dirNode);
                    dirNode.TopNode = treeNode;
                }
            }

            foreach (var article in root.Articles.OrderBy(a => a.Number))
            {
                var articleNode = BuildTreeViewModel(article, showMode);
                if (articleNode != null)
                {
                    treeNode.SubNodes.Add(articleNode);
                    articleNode.TopNode = treeNode;
                }
            }
            return treeNode;
        }

        public List<TreeViewModel> BuildTreeHierarchy(SubDirectory root)
        {
            var treeHirarchy = new List<TreeViewModel>();
            if (root == null)
            {
                return null;
            }

            foreach (var dir in root.SubDirectories)
            {

            }
            return treeHirarchy;
        }
    }
}