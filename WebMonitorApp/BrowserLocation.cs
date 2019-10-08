using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Automation;
using System.Windows.Automation.Providers;
using System.Reflection;
using System.Runtime.InteropServices;
using System.IO;


namespace WebMonitorApp
{
    class BrowserLocation
    {
        public static string GetChromeUrl(Process process)
        {
            if (process == null)
                throw new ArgumentNullException("process");

            if (process.MainWindowHandle == IntPtr.Zero)
                return null;

            AutomationElement element;
            try
            {
                element = AutomationElement.FromHandle(process.MainWindowHandle);
            }
            catch (Exception)
            {
                element = null;
            }
            if (element == null)
                return null;
            AutomationElement edit = element.FindFirst(TreeScope.Subtree, new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.Edit));
            if (edit == null)
                return null;
            return ((ValuePattern)edit.GetCurrentPattern(ValuePattern.Pattern)).Current.Value as string;


        }

        public static string GetInternetExplorerUrl(Process process)
        {
            if (process == null)
                throw new ArgumentNullException("process");

            if (process.MainWindowHandle == IntPtr.Zero)
                return null;

            AutomationElement element = AutomationElement.FromHandle(process.MainWindowHandle);
            if (element == null)
                return null;

            AutomationElement rebar = element.FindFirst(TreeScope.Children, new PropertyCondition(AutomationElement.ClassNameProperty, "ReBarWindow32"));
            if (rebar == null)
                return null;

            AutomationElement edit = rebar.FindFirst(TreeScope.Subtree, new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.Edit));

            return ((ValuePattern)edit.GetCurrentPattern(ValuePattern.Pattern)).Current.Value as string;
        }


        public static string GetFirefoxUrl(Process process)
        {
            if (process == null)
                throw new ArgumentNullException("process");

            if (process.MainWindowHandle == IntPtr.Zero)
                return null;

            AutomationElement element = AutomationElement.FromHandle(process.MainWindowHandle);
            if (element == null)
                return null;
            var visibleCondition = new PropertyCondition(AutomationElement.IsInvokePatternAvailableProperty, false);
            var controlCondition = new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.Group);
            var condition = new AndCondition(visibleCondition, controlCondition);
            AutomationElement group = element.FindFirst(TreeScope.Subtree, condition);
            if (group == null)
                return null;
            AutomationElement container = group.FindFirst(TreeScope.Children, new PropertyCondition(AutomationElement.IsOffscreenProperty, false));
            if (container == null)
                return null;
            AutomationElement doc = container.FindFirst(TreeScope.Subtree, new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.Document));
            if (doc == null)
                return null;

            return ((ValuePattern)doc.GetCurrentPattern(ValuePattern.Pattern)).Current.Value as string;
        }
    }
}
