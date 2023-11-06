using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuickGrid.Crud
{
    public class ButtonGenericBase<T>
    {
        public EventCallback<T> ActionEvent { get; set; }
        public string ClassCSS { get; set; }
        public string Title { get; set; }
        public string BSToggle { get; set; }
        public string BSTarget { get; set; }
        public HashSet<string> RoleRequired { get; set; }
    }

    public class ButtonGeneric<T> : ButtonGenericBase<T>
    {
        public List<ConditionButton<T>> Conditions { get; set; }
    }

    public class ConditionButton<T> : ButtonGenericBase<T>
    {
        public Func<T, bool> Verify { get; set; }
    }
}
