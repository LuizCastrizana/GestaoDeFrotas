using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Text;
using GestaoDeFrotas.Models;

namespace GestaoDeFrotas
{
    public class LeitorCSV
    {
        private readonly string _caminhoArquivo;

        public LeitorCSV(string caminhoArquivo)
        {
            this._caminhoArquivo = caminhoArquivo;
        }

        public IEnumerable<Estado> LerEstados()
        {
            List<Estado> listaEstados = new List<Estado>();
            var reader = new StreamReader(File.OpenRead(_caminhoArquivo), new UTF8Encoding());
            reader.ReadLine();
            while (!reader.EndOfStream)
            {
                Estado estado = new Estado();
                var linha = reader.ReadLine();
                var valores = linha.Split(';');
                estado.CodigoIbge = valores[0];
                estado.ID = Convert.ToInt32(valores[0]);
                estado.NomeEstado = valores[1].ToUpper();
                estado.UF = valores[2].ToUpper();
                listaEstados.Add(estado);
            }

            return listaEstados;
        }

        public IEnumerable<Municipio> LerMunicipios()
        {
            List<Municipio> listaMunicipios = new List<Municipio>();
            var reader = new StreamReader(File.OpenRead(_caminhoArquivo), new UTF8Encoding());
            reader.ReadLine();
            while (!reader.EndOfStream)
            {
                Municipio municipio = new Municipio();
                var linha = reader.ReadLine();
                var valores = linha.Split(';');
                municipio.CodigoIbge = valores[2];
                municipio.NomeMunicipio = valores[4].ToUpper();
                municipio.Estado.ID = Convert.ToInt32(valores[2].Substring(0, 2));
                listaMunicipios.Add(municipio);
            }

            return listaMunicipios;
        }
    }
}