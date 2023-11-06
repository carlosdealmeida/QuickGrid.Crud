using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuickGrid.Crud
{
    public class Configuration<TItem, TController, TEntity>
    {
        public Dictionary<string, ButtonGeneric<TItem>> ActionsGrid { get; set; }
        public bool UseModal { get; set; }
        public string LinkForm { get; set; }
        public bool IsCrud { get; set; }
        public bool CanInsert { get; set; }
        public bool CanEdit { get; set; }
        public bool CanDelete { get; set; }
        public string KeyPropertyName { get; set; }
        public string SizeModal { get; set; }
        public string RolesCadastrar { get; set; }
        public string ListMethod { get; set; }
        public TItem Item { get; set; }
        public TController Controller { get; set; }
        public TEntity Entity { get; set; }
    }
}
