﻿#pragma checksum "..\..\SummaryView.xaml" "{8829d00f-11b8-4213-878b-770e8597ac16}" "19372E20C71EAB97102F0394B12758DC497BF889817E5975DFA8D9F82AE433E4"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Shell;
using WindowsPerformanceMonitor;
using WindowsPerformanceMonitor.Graphs;


namespace WindowsPerformanceMonitor {
    
    
    /// <summary>
    /// SummaryView
    /// </summary>
    public partial class SummaryView : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 22 "..\..\SummaryView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock t1;
        
        #line default
        #line hidden
        
        
        #line 23 "..\..\SummaryView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock t2;
        
        #line default
        #line hidden
        
        
        #line 24 "..\..\SummaryView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock t3;
        
        #line default
        #line hidden
        
        
        #line 25 "..\..\SummaryView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal WindowsPerformanceMonitor.Graphs.LiveLineGraph liveGraph1;
        
        #line default
        #line hidden
        
        
        #line 26 "..\..\SummaryView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal WindowsPerformanceMonitor.Graphs.LiveLineGraph liveGraph2;
        
        #line default
        #line hidden
        
        
        #line 27 "..\..\SummaryView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal WindowsPerformanceMonitor.Graphs.LiveLineGraph liveGraph3;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/WindowsPerformanceMonitor;component/summaryview.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\SummaryView.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal System.Delegate _CreateDelegate(System.Type delegateType, string handler) {
            return System.Delegate.CreateDelegate(delegateType, this, handler);
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            
            #line 9 "..\..\SummaryView.xaml"
            ((WindowsPerformanceMonitor.SummaryView)(target)).Closing += new System.ComponentModel.CancelEventHandler(this.SummaryView_Closing);
            
            #line default
            #line hidden
            return;
            case 2:
            this.t1 = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 3:
            this.t2 = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 4:
            this.t3 = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 5:
            this.liveGraph1 = ((WindowsPerformanceMonitor.Graphs.LiveLineGraph)(target));
            return;
            case 6:
            this.liveGraph2 = ((WindowsPerformanceMonitor.Graphs.LiveLineGraph)(target));
            return;
            case 7:
            this.liveGraph3 = ((WindowsPerformanceMonitor.Graphs.LiveLineGraph)(target));
            return;
            case 8:
            
            #line 28 "..\..\SummaryView.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.DetailedView_Click);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}

