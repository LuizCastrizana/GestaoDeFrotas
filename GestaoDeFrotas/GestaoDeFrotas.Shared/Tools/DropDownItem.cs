using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GestaoDeFrotas.Shared.Tools
{
    public class DropDownItem
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public DropDownItem(int id, string text)
        {
            this.Id = id;
            this.Text = text;
        }
    }
}