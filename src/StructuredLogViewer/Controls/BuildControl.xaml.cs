﻿using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using Microsoft.Build.Logging.StructuredLogger;

namespace StructuredLogViewer.Controls
{
    public partial class BuildControl : UserControl
    {
        public BuildControl(Build build)
        {
            InitializeComponent();
            DataContext = build;
            Build = build;

            var existingTreeViewItemStyle = (Style)Application.Current.Resources[typeof(TreeViewItem)];
            var treeViewItemStyle = new Style(typeof(TreeViewItem), existingTreeViewItemStyle);
            treeViewItemStyle.Setters.Add(new Setter(TreeViewItem.IsExpandedProperty, new Binding("IsExpanded") { Mode = BindingMode.TwoWay }));
            treeViewItemStyle.Setters.Add(new Setter(TreeViewItem.IsSelectedProperty, new Binding("IsSelected") { Mode = BindingMode.TwoWay }));
            treeViewItemStyle.Setters.Add(new Setter(TreeViewItem.VisibilityProperty, new Binding("IsVisible") { Mode = BindingMode.TwoWay, Converter = new BooleanToVisibilityConverter() }));
            treeViewItemStyle.Setters.Add(new EventSetter(MouseDoubleClickEvent, (MouseButtonEventHandler)OnItemDoubleClick));
            treeViewItemStyle.Setters.Add(new EventSetter(RequestBringIntoViewEvent, (RequestBringIntoViewEventHandler)TreeViewItem_RequestBringIntoView));
            //treeViewItemStyle.Setters.Add(new Setter(FrameworkElement.ContextMenuProperty, contextMenu));

            treeView.ItemContainerStyle = treeViewItemStyle;
            treeView.KeyDown += TreeView_KeyDown;
        }

        private void TreeView_KeyDown(object sender, KeyEventArgs args)
        {
            if (args.Key == Key.Delete)
            {
                var node = treeView.SelectedItem as TreeNode;
                if (node != null)
                {
                    MoveSelectionOut(node);
                    node.IsVisible = false;
                    args.Handled = true;
                }
            }
        }

        private void MoveSelectionOut(TreeNode node)
        {
            var parent = node.Parent;
            if (parent == null)
            {
                return;
            }

            var next = node.FindNext<TreeNode>();
            if (next != null)
            {
                node.IsSelected = false;
                next.IsSelected = true;
                return;
            }

            var previous = node.FindPrevious<TreeNode>();
            if (previous != null)
            {
                node.IsSelected = false;
                previous.IsSelected = true;
            }
            else
            {
                node.IsSelected = false;
                parent.IsSelected = true;
            }
        }

        private void OnItemDoubleClick(object sender, MouseButtonEventArgs args)
        {
            TreeNode treeNode = GetNode(args);
            if (treeNode != null)
            {
                // TODO: handle double-click on node
                args.Handled = true;
            }
        }

        private static TreeNode GetNode(RoutedEventArgs args)
        {
            var treeViewItem = args.Source as TreeViewItem;
            var treeNode = treeViewItem?.DataContext as TreeNode;
            return treeNode;
        }

        public Build Build { get; set; }

        private void searchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var searchText = searchTextBox.Text;
            if (string.IsNullOrWhiteSpace(searchText))
            {
                return;
            }

            Search(searchText);
        }

        private void Search(string searchText)
        {
            var tree = treeView;
        }

        private void TreeViewItem_RequestBringIntoView(object sender, RequestBringIntoViewEventArgs args)
        {
            // prevent the annoying horizontal scrolling
            args.Handled = true;
        }
    }
}
