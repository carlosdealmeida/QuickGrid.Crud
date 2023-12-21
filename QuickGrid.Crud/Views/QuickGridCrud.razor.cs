using Blazored.Toast.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.QuickGrid;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Microsoft.JSInterop;
using quick_crud.Services;
using QuickGrid.Crud.Helpers;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace QuickGrid.Crud.Views
{
    public class QuickGridCrudBase<TItem, TController, TEntity>: ComponentBase
    {
        [Parameter] public Dictionary<string, ButtonGeneric<TItem>> Acoes { get; set; }
        [Parameter] public RenderFragment<TEntity>? FormTemplate { get; set; }
        [Parameter] public bool UseModal { get; set; }
        [Parameter] public string LinkForm { get; set; }
        [Parameter] public bool IsCrud { get; set; }
        [Parameter] public string TamanhoModal { get; set; }
        [Parameter] public string RolesCadastrar { get; set; }
        [Parameter] public Configuration<TItem, TController, TEntity>? Configuration { get; set; }
        [CascadingParameter] private Task<AuthenticationState> authenticationStateTask { get; set; }
        [Inject] public IHostEnvironment WebHostEnvironment { get; set; }
        [Inject] public NavigationManager navigation { get; set; }
        [Inject] public IJSRuntime JSRuntime { get; set; }
        [Inject] public IToastService toastService { get; set; }
        [Inject] public IServiceProvider ServiceProvider { get; set; }
        public IServiceBase ServiceBase { get; set; }
        public PaginationState pagination = new PaginationState { ItemsPerPage = 10 };
        public FilterGenericState filterStateBanco = new FilterGenericState();
        private FilterGenericState _previousFilterStateGrid;
        public FilterGenericState filterStateGrid = new FilterGenericState();
        public Dictionary<string, bool> columnVisibility = new Dictionary<string, bool>();
        public HashSet<string> userRoles = new();
        public string UrlEdit;
        public TEntity ItemSelecionado { get; set; }
        public IQueryable<TItem> Items { get; set; }
        public IQueryable<TItem> ItemsFiltro
        {
            get
            {
                return ApplyFilter();
            }
        }

        private IQueryable<TItem> ApplyFilter()
        {
            IQueryable<TItem> query = Items;

            if (query != null && query.Any())
            {
                query = FilterByString(query);

                query = FilterByDateTime(query);

                query = FilterByInt(query);

                query = FilterByBool(query);

                if (query != null && query.Any())
                    return query;
            }

            return Enumerable.Empty<TItem>().AsQueryable();
        }

        private IQueryable<TItem> FilterByBool(IQueryable<TItem> query)
        {
            // Filtros booleanos
            foreach (var filter in filterStateGrid.BoolFilters)
            {
                var property = typeof(TItem).GetProperty(filter.Key);
                if (property != null && property.PropertyType == typeof(bool))
                {
                    bool targetValue = filter.Value; // Pega o valor booleano
                    query = query.Where(f => (bool)property.GetValue(f) == targetValue);
                }
            }

            return query;
        }

        private IQueryable<TItem> FilterByInt(IQueryable<TItem> query)
        {
            // Filtros de inteiros
            foreach (var filter in filterStateGrid.IntFilters)
            {
                if (filter.Value.HasValue) // Verifica se o valor não é nulo
                {
                    var property = typeof(TItem).GetProperty(filter.Key);
                    if (property != null && property.PropertyType == typeof(int))
                    {
                        int targetValue = filter.Value.Value; // Pega o valor inteiro
                        query = query.Where(f => (int)property.GetValue(f) == targetValue);
                    }
                }
            }

            return query;
        }

        private IQueryable<TItem> FilterByDateTime(IQueryable<TItem> query)
        {
            // Filtros de data
            foreach (var filter in filterStateGrid.DateFilters)
            {
                var property = typeof(TItem).GetProperty(filter.Key);
                if (property != null && property.PropertyType == typeof(DateTime))
                {
                    if (filter.Value.Start.HasValue)
                    {
                        query = query.Where(f => (DateTime)property.GetValue(f) >= filter.Value.Start.Value.Date);
                    }
                    if (filter.Value.End.HasValue)
                    {
                        query = query.Where(f => (DateTime)property.GetValue(f) <= filter.Value.End.Value.Date.AddHours(23).AddMinutes(59).AddSeconds(59));
                    }
                }
            }

            return query;
        }

        private IQueryable<TItem> FilterByString(IQueryable<TItem> query)
        {
            // Filtros de string
            foreach (var filter in filterStateGrid.StringFilters)
            {
                if (!string.IsNullOrEmpty(filter.Value))
                {
                    var property = typeof(TItem).GetProperty(filter.Key);
                    if (property != null && property.PropertyType == typeof(string))
                    {
                        query = query.Where(f => (property.GetValue(f) as string).Contains(filter.Value, StringComparison.CurrentCultureIgnoreCase));
                    }
                }
            }

            return query;
        }

        public void RenderColumn(RenderTreeBuilder builder, TItem item, ButtonGeneric<TItem> acao)
        {

            if (!acao.RoleRequired.Any())
            {
                BuildAuthorizedButton(item, acao)(builder);
            }
            else if (acao.RoleRequired.Any(role => userRoles.Contains(role)))
            {
                BuildAuthorizedButton(item, acao)(builder);
            }
            else
            {
                BuildNotAuthorized();
            };
        }

        public RenderFragment BuildNotAuthorized() => builder => { };
        public void Salvar(TItem item)
        {

        }
        public RenderFragment BuildAuthorizedButton(TItem item, ButtonGeneric<TItem> acao) => builder =>
        {
            int seq = 0;
            var condicaoCorrespondente = acao.Conditions?.FirstOrDefault(cond => cond.Verify(item));
            var classCss = condicaoCorrespondente?.ClassCSS ?? acao.ClassCSS;
            var BSToggle = condicaoCorrespondente?.BSToggle ?? acao.BSToggle;
            var BSTarget = condicaoCorrespondente?.BSTarget ?? acao.BSTarget;
            var titulo = condicaoCorrespondente?.Title ?? acao.Title;
            var eventCall = condicaoCorrespondente?.ActionEvent ?? acao.ActionEvent;

            builder.OpenElement(seq++, "button");
            builder.AddAttribute(seq++, "class", classCss);
            builder.AddAttribute(seq++, "type", "button");
            builder.AddAttribute(seq++, "onclick", EventCallback.Factory.Create(this, () => eventCall.InvokeAsync(item)));

            if (!string.IsNullOrEmpty(BSToggle))
            {
                builder.AddAttribute(seq++, "data-bs-toggle", BSToggle);
            }

            if (!string.IsNullOrEmpty(BSTarget))
            {
                builder.AddAttribute(seq++, "data-bs-target", BSTarget);
            }

            builder.AddContent(seq++, ((MarkupString)titulo));
            builder.CloseElement();
        };

        protected override async Task OnInitializedAsync()
        {
            if (authenticationStateTask != null)
            {
                var authState = await authenticationStateTask;
                var user = authState.User;
                userRoles = new HashSet<string>(user.FindAll(ClaimTypes.Role).Select(c => c.Value));
            }
            else
            {
                userRoles = new HashSet<string>();
            };

            foreach (var prop in typeof(TItem).GetProperties())
            {
                foreach (Attribute attr in prop.GetCustomAttributes(true))
                {
                    if (attr is GridVisivel gridVisivel)
                    {
                        columnVisibility[prop.Name] = true;
                    }
                }
            }

            if (Configuration != null)
            {
                Acoes = Configuration.ActionsGrid;
                UseModal = Configuration.UseModal;
                LinkForm = Configuration.LinkForm;
                IsCrud = Configuration.IsCrud;
                TamanhoModal = Configuration.SizeModal;
                RolesCadastrar = Configuration.RolesCadastrar;

                if (Configuration.ServiceBaseType != null)
                {
                    ServiceBase = (IServiceBase)ServiceProvider.GetService(Configuration.ServiceBaseType);
                }
            }


            await Listar();
            await base.OnInitializedAsync();
            await InvokeAsync(StateHasChanged);
        }

        public void UpdateVisibility(string key, bool value)
        {
            if (columnVisibility.ContainsKey(key))
            {
                columnVisibility[key] = value;
            }
        }

        public async Task Listar()
        {
            Type analisar = typeof(TController);
            object controller;

            if (Configuration.ServiceBaseType != null)
            {
                controller = Activator.CreateInstance(analisar, ServiceBase);
            }
            else
            {
                controller = Activator.CreateInstance(analisar);
            }
                

            string method;

            if ((Configuration != null) && (!string.IsNullOrEmpty(Configuration.ListMethod)))
            {
                method = Configuration.ListMethod;
            }
            else
            {
                method = "GetAll";
            };

            MethodInfo metodo = controller.GetType().GetMethod(method);

            var filterExpression = filterStateBanco.GenerateFilterExpression<TEntity>();
            var parameters = new object[1] { filterExpression };
            var LstclassGeneric = (Task)metodo.Invoke(controller, parameters);
            await LstclassGeneric.ConfigureAwait(false);

            var resultProperty = LstclassGeneric.GetType().GetProperty("Result");

            var result = resultProperty.GetValue(LstclassGeneric);

            Items = (IQueryable<TItem>)result;

            await InvokeAsync(StateHasChanged);
        }

        public async void PageChangedHandler()
        {
            await Listar();
            await InvokeAsync(StateHasChanged);
        }

        public event Action OnChange;
        public void NotifyDataChanged()
        {
            OnChange?.Invoke();
        }

        protected override async Task OnParametersSetAsync()
        {
            if (_previousFilterStateGrid != filterStateGrid)
            {
                _previousFilterStateGrid = filterStateGrid;
                await InvokeAsync(StateHasChanged);
            }
        }

        public void LimparDatas(string propertyName)
        {
            filterStateBanco.SetDateFilterStart(propertyName, null);
            filterStateBanco.SetDateFilterEnd(propertyName, null);
        }

        public async Task ResetModel()
        {
            filterStateBanco = new();
            await Listar();
        }

        public async Task ExportarAsync(string fileType)
        {
            var fileName = DateTime.Now.ToString("yyyyMMddHHmmssfff");
            var path = WebHostEnvironment.ContentRootPath + "\\wwwroot";

            var filteredItems = ItemsFiltro.ToList().Select(item =>
            {
                dynamic expando = new ExpandoObject();
                var expandoDict = expando as IDictionary<string, object>;

                foreach (var entry in columnVisibility)
                {
                    if (entry.Value)
                    {
                        var value = item.GetType().GetProperty(entry.Key)?.GetValue(item, null);
                        expandoDict[entry.Key] = value;
                    }
                }

                return expando;
            }).ToList();

            switch (fileType)
            {
                case "xlsx":
                    filteredItems.ExportToXlsx(path, fileName);
                    break;
                case "csv":
                    filteredItems.ExportToCsv(path, fileName);
                    break;
                case "txt":
                    filteredItems.ExportToTxt(path, fileName);
                    break;
                case "xml":
                    filteredItems.ExportToXml(path, fileName);
                    break;
                default:
                    throw new ArgumentException("Tipo de arquivo não suportado.");
            }

            var url = $"{navigation.BaseUri}api/Export/{fileName}/{fileType}";
            await JSRuntime.InvokeVoidAsync("triggerFileDownload", fileName, url);
            toastService.ShowSuccess("Operação realizada com sucesso!");
        }

        public async Task SelecionarRegistro(bool Adicionar, TEntity Item)
        {
            Type analisar = typeof(TEntity);

            if (Adicionar)
                ItemSelecionado = (TEntity)Activator.CreateInstance(analisar);
            else
                ItemSelecionado = Item;

            await InvokeAsync(StateHasChanged);
        }
    }
}
