using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuickGrid.Crud
{
    [System.AttributeUsage(System.AttributeTargets.Property)]
    public class GridVisivel : System.Attribute
    {
        public bool visivel { get; set; }

        public GridVisivel(bool visivel)
        {
            this.visivel = visivel;
        }
    }
    [System.AttributeUsage(System.AttributeTargets.Property)]
    public class GridDisponivel : System.Attribute
    {
        public bool disponivel { get; set; }

        public GridDisponivel(bool disponivel)
        {
            this.disponivel = disponivel;
        }
    }

    [System.AttributeUsage(System.AttributeTargets.Property)]
    public class GridPodeFiltrar : System.Attribute
    {
        public bool podeFiltrar { get; set; }

        public GridPodeFiltrar(bool podeFiltrar)
        {
            this.podeFiltrar = podeFiltrar;
        }
    }

    [System.AttributeUsage(System.AttributeTargets.Property)]
    public class GridPodeOrdenar : System.Attribute
    {
        public bool podeOrdenar { get; set; }

        public GridPodeOrdenar(bool podeOrdenar)
        {
            this.podeOrdenar = podeOrdenar;
        }
    }

    [System.AttributeUsage(System.AttributeTargets.Property)]
    public class GridTipoFormatacao : System.Attribute
    {
        public string tipoFormatacao { get; set; }

        public GridTipoFormatacao(string tipoFormatacao)
        {
            this.tipoFormatacao = tipoFormatacao;
        }
    }

    [System.AttributeUsage(System.AttributeTargets.Property)]
    public class GridTituloColuna : System.Attribute
    {
        public string tituloColuna { get; set; }

        public GridTituloColuna(string tituloColuna)
        {
            this.tituloColuna = tituloColuna;
        }
    }

    [System.AttributeUsage(System.AttributeTargets.Property)]
    public class GridTemplateColumn : System.Attribute
    {
        public bool ehTemplateColumn { get; set; }

        public GridTemplateColumn(bool ehTemplateColumn)
        {
            this.ehTemplateColumn = ehTemplateColumn;
        }
    }
}
