﻿#pragma checksum "..\..\Logging.xaml" "{8829d00f-11b8-4213-878b-770e8597ac16}" "594A2DD0DB55C0F40115A96B136D87398B3B6AC369E2D6FB2E62B92BFB135EFD"
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
using WindowsPerformanceMonitor.Graphs;


namespace WindowsPerformanceMonitor {
    
    
    /// <summary>
    /// Logging
    /// </summary>
    public partial class Logging : System.Windows.Controls.UserControl, System.Windows.Markup.IComponentConnector {
        
        
        #line 21 "..\..\Logging.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal WindowsPerformanceMonitor.Graphs.LiveLineGraph liveGraph;
        
        #line default
        #line hidden
        
        
        #line 32 "..\..\Logging.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.GridView listView_gridView;
        
        #line default
        #line hidden
        
        
        #line 89 "..\..\Logging.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ComboBox comboBox1;
        
        #line default
        #line hidden
        
        
        #line 96 "..\..\Logging.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.StackPanel radioButtons;
        
        #line default
        #line hidden
        
        
        #line 97 "..\..\Logging.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.CheckBox cbAll;
        
        #line default
        #line hidden
        
        
        #line 98 "..\..\Logging.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.CheckBox cpu;
        
        #line default
        #line hidden
        
        
        #line 99 "..\..\Logging.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.CheckBox gpu;
        
        #line default
        #line hidden
        
        
        #line 100 "..\..\Logging.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.CheckBox memory;
        
        #line default
        #line hidden
        
        
        #line 101 "..\..\Logging.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.CheckBox disk;
        
        #line default
        #line hidden
        
        
        #line 102 "..\..\Logging.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.CheckBox network;
        
        #line default
        #line hidden
        
        
        #line 103 "..\..\Logging.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.CheckBox gpuTemp;
        
        #line default
        #line hidden
        
        
        #line 104 "..\..\Logging.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.CheckBox cpuTemp;
        
        #line default
        #line hidden
        
        
        #line 112 "..\..\Logging.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button StartRecordingButton;
        
        #line default
        #line hidden
        
        
        #line 113 "..\..\Logging.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button StopRecordingButton;
        
        #line default
        #line hidden
        
        
        #line 124 "..\..\Logging.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ListView logList;
        
        #line default
        #line hidden
        
        
        #line 127 "..\..\Logging.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.GridView storedLogs_ListView;
        
        #line default
        #line hidden
        
        
        #line 139 "..\..\Logging.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button PauseButton;
        
        #line default
        #line hidden
        
        
        #line 142 "..\..\Logging.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button StepBack;
        
        #line default
        #line hidden
        
        
        #line 143 "..\..\Logging.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button StepForward;
        
        #line default
        #line hidden
        
        
        #line 147 "..\..\Logging.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ProgressBar pBar;
        
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
            System.Uri resourceLocater = new System.Uri("/WindowsPerformanceMonitor;component/logging.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\Logging.xaml"
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
            
            #line 5 "..\..\Logging.xaml"
            ((WindowsPerformanceMonitor.Logging)(target)).Loaded += new System.Windows.RoutedEventHandler(this.OnControlLoaded);
            
            #line default
            #line hidden
            return;
            case 2:
            this.liveGraph = ((WindowsPerformanceMonitor.Graphs.LiveLineGraph)(target));
            return;
            case 3:
            this.listView_gridView = ((System.Windows.Controls.GridView)(target));
            return;
            case 4:
            this.comboBox1 = ((System.Windows.Controls.ComboBox)(target));
            
            #line 91 "..\..\Logging.xaml"
            this.comboBox1.SelectionChanged += new System.Windows.Controls.SelectionChangedEventHandler(this.ComboBox_SelectionChanged);
            
            #line default
            #line hidden
            return;
            case 5:
            this.radioButtons = ((System.Windows.Controls.StackPanel)(target));
            return;
            case 6:
            this.cbAll = ((System.Windows.Controls.CheckBox)(target));
            
            #line 97 "..\..\Logging.xaml"
            this.cbAll.Checked += new System.Windows.RoutedEventHandler(this.CBAllChanged);
            
            #line default
            #line hidden
            
            #line 97 "..\..\Logging.xaml"
            this.cbAll.Unchecked += new System.Windows.RoutedEventHandler(this.CBAllChanged);
            
            #line default
            #line hidden
            return;
            case 7:
            this.cpu = ((System.Windows.Controls.CheckBox)(target));
            
            #line 98 "..\..\Logging.xaml"
            this.cpu.Checked += new System.Windows.RoutedEventHandler(this.CheckBoxChanged);
            
            #line default
            #line hidden
            
            #line 98 "..\..\Logging.xaml"
            this.cpu.Unchecked += new System.Windows.RoutedEventHandler(this.CheckBoxChanged);
            
            #line default
            #line hidden
            return;
            case 8:
            this.gpu = ((System.Windows.Controls.CheckBox)(target));
            
            #line 99 "..\..\Logging.xaml"
            this.gpu.Checked += new System.Windows.RoutedEventHandler(this.CheckBoxChanged);
            
            #line default
            #line hidden
            
            #line 99 "..\..\Logging.xaml"
            this.gpu.Unchecked += new System.Windows.RoutedEventHandler(this.CheckBoxChanged);
            
            #line default
            #line hidden
            return;
            case 9:
            this.memory = ((System.Windows.Controls.CheckBox)(target));
            
            #line 100 "..\..\Logging.xaml"
            this.memory.Checked += new System.Windows.RoutedEventHandler(this.CheckBoxChanged);
            
            #line default
            #line hidden
            
            #line 100 "..\..\Logging.xaml"
            this.memory.Unchecked += new System.Windows.RoutedEventHandler(this.CheckBoxChanged);
            
            #line default
            #line hidden
            return;
            case 10:
            this.disk = ((System.Windows.Controls.CheckBox)(target));
            
            #line 101 "..\..\Logging.xaml"
            this.disk.Checked += new System.Windows.RoutedEventHandler(this.CheckBoxChanged);
            
            #line default
            #line hidden
            
            #line 101 "..\..\Logging.xaml"
            this.disk.Unchecked += new System.Windows.RoutedEventHandler(this.CheckBoxChanged);
            
            #line default
            #line hidden
            return;
            case 11:
            this.network = ((System.Windows.Controls.CheckBox)(target));
            
            #line 102 "..\..\Logging.xaml"
            this.network.Checked += new System.Windows.RoutedEventHandler(this.CheckBoxChanged);
            
            #line default
            #line hidden
            
            #line 102 "..\..\Logging.xaml"
            this.network.Unchecked += new System.Windows.RoutedEventHandler(this.CheckBoxChanged);
            
            #line default
            #line hidden
            return;
            case 12:
            this.gpuTemp = ((System.Windows.Controls.CheckBox)(target));
            
            #line 103 "..\..\Logging.xaml"
            this.gpuTemp.Checked += new System.Windows.RoutedEventHandler(this.CheckBoxChanged);
            
            #line default
            #line hidden
            
            #line 103 "..\..\Logging.xaml"
            this.gpuTemp.Unchecked += new System.Windows.RoutedEventHandler(this.CheckBoxChanged);
            
            #line default
            #line hidden
            return;
            case 13:
            this.cpuTemp = ((System.Windows.Controls.CheckBox)(target));
            
            #line 104 "..\..\Logging.xaml"
            this.cpuTemp.Checked += new System.Windows.RoutedEventHandler(this.CheckBoxChanged);
            
            #line default
            #line hidden
            
            #line 104 "..\..\Logging.xaml"
            this.cpuTemp.Unchecked += new System.Windows.RoutedEventHandler(this.CheckBoxChanged);
            
            #line default
            #line hidden
            return;
            case 14:
            this.StartRecordingButton = ((System.Windows.Controls.Button)(target));
            
            #line 112 "..\..\Logging.xaml"
            this.StartRecordingButton.Click += new System.Windows.RoutedEventHandler(this.StartLog_Click);
            
            #line default
            #line hidden
            return;
            case 15:
            this.StopRecordingButton = ((System.Windows.Controls.Button)(target));
            
            #line 113 "..\..\Logging.xaml"
            this.StopRecordingButton.Click += new System.Windows.RoutedEventHandler(this.StopLog_Click);
            
            #line default
            #line hidden
            return;
            case 16:
            this.logList = ((System.Windows.Controls.ListView)(target));
            
            #line 122 "..\..\Logging.xaml"
            this.logList.PreviewMouseLeftButtonUp += new System.Windows.Input.MouseButtonEventHandler(this.logList_Click);
            
            #line default
            #line hidden
            return;
            case 17:
            this.storedLogs_ListView = ((System.Windows.Controls.GridView)(target));
            return;
            case 18:
            
            #line 138 "..\..\Logging.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.PlayLog_Click);
            
            #line default
            #line hidden
            return;
            case 19:
            this.PauseButton = ((System.Windows.Controls.Button)(target));
            
            #line 139 "..\..\Logging.xaml"
            this.PauseButton.Click += new System.Windows.RoutedEventHandler(this.PauseLog_Click);
            
            #line default
            #line hidden
            return;
            case 20:
            this.StepBack = ((System.Windows.Controls.Button)(target));
            
            #line 142 "..\..\Logging.xaml"
            this.StepBack.Click += new System.Windows.RoutedEventHandler(this.Step_Back);
            
            #line default
            #line hidden
            return;
            case 21:
            this.StepForward = ((System.Windows.Controls.Button)(target));
            
            #line 143 "..\..\Logging.xaml"
            this.StepForward.Click += new System.Windows.RoutedEventHandler(this.Step_Forward);
            
            #line default
            #line hidden
            return;
            case 22:
            this.pBar = ((System.Windows.Controls.ProgressBar)(target));
            return;
            }
            this._contentLoaded = true;
        }
    }
}

