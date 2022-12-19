using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLAccess
{
    public static class Computes
    {
        public static int ComputeStartRowIndex(int selectedIndex,int columnCount,int primaryColumnNumber)
        {
            int selectedNumber = selectedIndex + 1;
            int numberInRow = selectedNumber % columnCount;
            numberInRow= numberInRow == 0 ? columnCount : numberInRow;
            return (selectedNumber - (numberInRow - primaryColumnNumber)) - 1;
        }
    }
}
