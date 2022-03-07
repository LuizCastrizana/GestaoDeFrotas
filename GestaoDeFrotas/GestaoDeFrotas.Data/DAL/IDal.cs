using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestaoDeFrotas.Data.DAL
{
    interface IDal<T>
    {
        void Create(T obj);
        T Read(int id);
        void Update(T obj);
        void UpdateStatus(int id, bool status);
    }
}
