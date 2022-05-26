using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using AppMinhasCompras.Model;
using SQLite;

namespace AppMinhasCompras.Helper
{
    /**
     * 
     */ 
    public class SQLiteDatabaseHelper
    {
        /**
         * 
         */ 
        readonly SQLiteAsyncConnection _conn;


        /**
         * 
         */ 
        public SQLiteDatabaseHelper(string path)
        {
            _conn = new SQLiteAsyncConnection(path);
            _conn.CreateTableAsync<Produto>().Wait();
        }


        /**
         * 
         */ 
        public Task<int> Insert(Produto p)
        {
            return _conn.InsertAsync(p);
        }


        /**
         * 
         */ 
        public Task<List<Produto>> Update(Produto p)
        {
            string sql = "UPDATE Produto SET Descricao=?, Quantidade=?, Preco=? WHERE id= ? ";
            return _conn.QueryAsync<Produto>(sql, p.Descricao, p.Quantidade, p.Preco, p.Id);
        }


        /**
         * 
         */ 
        public Task<List<Produto>> GetAll()
        {
            return _conn.Table<Produto>().ToListAsync();
        }


        /**
         * 
         */
        public Task<int> Delete(int id)
        {
            return _conn.Table<Produto>().DeleteAsync(i => i.Id == id);
        }

        /**
         * 
         */
        public Task<List<Produto>> Search(string q)
        {
            string sql = "SELECT * FROM Produto WHERE Descricao LIKE '%" + q + "%' ";

            return _conn.QueryAsync<Produto>(sql);
        }
    }
}
