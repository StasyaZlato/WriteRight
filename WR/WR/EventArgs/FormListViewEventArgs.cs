using System;
using System.Collections.Generic;

namespace WR.CustomEventArgs
{
    public class FormListViewEventArgs : EventArgs
    {
        public List<string[]> collection;

        public FormListViewEventArgs(List<string[]> col)
        {
            collection = col;
        }
    }
}