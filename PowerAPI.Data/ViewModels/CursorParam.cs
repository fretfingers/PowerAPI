using System;
using System.Collections.Generic;
using System.Text;

namespace PowerAPI.Data.ViewModels
{
    public class CursorParam
    {
        public int Count { get; set; } = 50;
        public int Cursor { get; set; } = 0;
      //  public dynamic totalSize { get; set; }
    }
}
