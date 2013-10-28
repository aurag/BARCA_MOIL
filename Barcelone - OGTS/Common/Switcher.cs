using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using Barcelone___OGTS.View;

namespace Barcelone___OGTS.Common
{
    public static class Switcher
    {
        public static MainWindow pageSwitcher;
        public static Stack<UserControl> NavigationStack = new Stack<UserControl>();
        public static Hashtable ApplicationState = new Hashtable();

        private static UserControl _currentPage = null;

        #region Navigation Methods
        public static void Switch(UserControl newPage)
        {
            pageSwitcher.Navigate(newPage);
            if (_currentPage != null)
                NavigationStack.Push(_currentPage);
            _currentPage = newPage;
        }

        public static void Switch(UserControl newPage, object state)
        {
            pageSwitcher.Navigate(newPage, state);
        }

        public static void SwitchBack()
        {
            UserControl u = null;

            if (NavigationStack.Count > 0)
            {
                u = NavigationStack.Pop();
                pageSwitcher.Navigate(u);
                _currentPage = u;
            }
            else
                MessageBox.Show("vous ne pouvez pas naviguer en arrière!");
        }
        #endregion

        #region Application State Handling
        public static void AddObject(string key, object value)
        {
            if (!ApplicationState.Contains(key))
                Switcher.ApplicationState.Add(key, value);
            else
                Switcher.ApplicationState[key] = value;
        }
        #endregion
    }
}
