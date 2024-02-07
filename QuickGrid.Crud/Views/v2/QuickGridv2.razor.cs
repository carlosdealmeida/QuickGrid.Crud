using Blazored.Toast.Services;
using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Components;
using Microsoft.Fast.Components.FluentUI;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Hosting;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using QuickGrid.Crud;
using QuickGrid.Crud.Helpers;

namespace quick_crud.Views.v2
{
	public partial class QuickGridv2<TItem> : ComponentBase
	{
		[Inject] public IHostEnvironment WebHostEnvironment { get; set; }
		[Inject] public NavigationManager navigation { get; set; }
		[Inject] public IJSRuntime JSRuntime { get; set; }
		public FilterGenericState filterStateGrid = new FilterGenericState();
		public Dictionary<string, bool> columnVisibility = new Dictionary<string, bool>();
		public PaginationState pagination = new PaginationState { ItemsPerPage = 10 };
		[Parameter] public IQueryable<TItem> lstItens { get; set; }
		[Parameter] public RenderFragment? FormFiltro { get; set; }
		public IQueryable<TItem> ItensFiltro
		{
			get
			{
				return ApplyFilter();
			}
		}

		private IQueryable<TItem> ApplyFilter()
		{

			if (lstItens != null)
			{
				IQueryable<TItem> query = lstItens;

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
			else
			{
				return null;
			}
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

		public async Task ExportarAsync(string fileType)
		{
			var fileName = DateTime.Now.ToString("yyyyMMddHHmmssfff");
			var path = WebHostEnvironment.ContentRootPath + "\\wwwroot";

			Dictionary<string, string> ColunaPropriedadeNome = new();
			string tituloCol = "";

			foreach (var prop in typeof(TItem).GetProperties())
			{
				foreach (Attribute attr in prop.GetCustomAttributes(true))
				{
					if (attr is GridTituloColuna gridTituloColuna)
					{
						tituloCol = gridTituloColuna.tituloColuna;
					}
				}

				if (tituloCol == "")
				{
					tituloCol = prop.Name;
				};

				ColunaPropriedadeNome.Add(tituloCol, prop.Name);
			}

			var filteredItems = ItensFiltro.ToList().Select(item =>
			{
				dynamic expando = new ExpandoObject();
				var expandoDict = expando as IDictionary<string, object>;

				foreach (var entry in columnVisibility)
				{
					if (entry.Value)
					{
						var value = item.GetType().GetProperty(ColunaPropriedadeNome.GetValueOrDefault(entry.Key))?.GetValue(item, null);
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
			//toastService.ShowSuccess("Operação realizada com sucesso!");
		}

		protected void UpdateVisibility(string key, bool value)
		{
			if (columnVisibility.ContainsKey(key))
			{
				columnVisibility[key] = value;
			}
		}

		protected override void OnInitialized()
		{
			foreach (var prop in typeof(TItem).GetProperties())
			{
				string tituloCol = "";

				foreach (Attribute attr in prop.GetCustomAttributes(true))
				{
					if (attr is GridTituloColuna gridTituloColuna)
					{
						tituloCol = gridTituloColuna.tituloColuna;
					}

					if (attr is GridVisivel gridVisivel)
					{
						columnVisibility[tituloCol] = true;
					}
				}

				if (tituloCol == "")
				{
					tituloCol = prop.Name;
				};
			}
		}

		public async void SetQtdPorPagina(ChangeEventArgs e)
		{
			pagination.ItemsPerPage = int.Parse(e.Value.ToString());
			await InvokeAsync(StateHasChanged);
		}
	}
}