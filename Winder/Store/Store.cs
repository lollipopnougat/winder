using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Winder.Storage
{
    public class Store
    {

        private List<string> history;
        public List<string> History => history;
        private Store()
        {
            history = [];

        }

        public static Store GenNewStore()
        {
            return new Store();
        }
    }
}
