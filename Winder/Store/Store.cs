using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Winder.ViewModel;

namespace Winder.Storage
{
    public class Store
    {

        private List<string> history;
        //public List<string> History => history;

        private int historyPointer = -1;

        public FileItemViewModel? CopyFile { get; set; }
        public bool IsCutPaste { get; set; }


        private Store()
        {
            history = [];
        }

        public bool CanBack => historyPointer > 0;
        public bool CanForward => historyPointer != -1 && historyPointer < history.Count - 1;

        public void addHistory(string item)
        {
            if (historyPointer != -1)
            {
                int count = history.Count - historyPointer - 1;
                if (count > 0)
                {
                    history.RemoveRange(historyPointer + 1, count);
                }
            }
            history.Add(item);
            historyPointer = history.Count - 1;
        }

        public void clearHistory()
        {
            history.Clear();
            historyPointer = -1;
        }

        public string? getBack()
        {
            if (historyPointer > 0)
            {
                historyPointer--;
                return history[historyPointer];
            }
            return null;
        }

        public string? getForward()
        {
            if (historyPointer < history.Count - 1 && historyPointer != -1)
            {
                historyPointer++;
                return history[historyPointer];
            }
            return null;
        }

        public static Store GenNewStore()
        {
            return new Store();
        }
    }
}
