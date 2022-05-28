/**
 * Arquivo Model para criação da Tabela e Transporte de Dados
 */
using AppMinhasCompras.Model;

/**
 * Classes da Bilioteca do SQLite para acesso aos dados e criação da estrutura da tabela.
 * Veja mais em: https://github.com/praeclarum/sqlite-net
 */
using SQLite;

/**
 * Classes como List. Veja minha aula sobre em: https://www.youtube.com/watch?v=m8rR1aLjrg0
 */
using System.Collections.Generic;

/**
 * Criação de Threads e Tarefas para execução assíncrona (sem congelar a tela do app) de ações.
 */
using System.Threading.Tasks; 

namespace AppMinhasCompras.Helper
{
    /**
     * Definição da classe SQLiteDatabaseHelper que funciona como uma abstração de acesso ao arquivo db3
     * do SQLite. A classe contém as informações de "conexão" e os métodos para realizar o CRUD (Create,
     * Read, Update e Delete).
     * Observe que na classe todos os métodos são Async, isso significa que todos são executados via Threads
     * o que, em teoria) não trava a interface do app enquanto os dados são lidos/gravados no arquivo db3.
     */
    public class SQLiteDatabaseHelper
    {
        /**
         * Campo da classe que armazena a "conexão" com o arquivo db3.
         * Isso siginifica que o arquivo db3 é aberto e armazenado aqui para que
         * essa classe possa usar os métodos da classe do SQLite para gravar
         * e ler dados do arquivo.
         */ 
        readonly SQLiteAsyncConnection _conn;


        /**
         * Método construtor da classe que recebe um parâmetro chamado path para
         * "conectar" ao arquivo db3.
         */ 
        public SQLiteDatabaseHelper(string path)
        {
            /**
             * Abrindo uma nova "conexão" com o arquivo db3 através do caminho recebido.
             * note a utilização da biblioteca SQLite "instalada" no projeto via pacote Nuget
             */  
            _conn = new SQLiteAsyncConnection(path);

            /**
             * Criação da tabela com base no Model Produto (mais detalhes no arquivo Produto.cs na pasta Model)
             * Note que apesar do Async na criação da tabela é chamado o método Wait() que define a espera
             * da criação da tabela (se ela ainda não existir) antes de efetuar as outras operações, por exemplo,
             * insert.
             */ 
            _conn.CreateTableAsync<Produto>().Wait();
        }


        /**
         * Método que faz a inseração de um novo registro na tabela. Veja que o método recebe uma Model
         * preenchida com os dados a serem inseridos. Observe que o método tem um retorno do tipo int 
         * (número de linhas inseridas) sendo executado via Task (tarefa sendo executada de forma assíncrona).
         */ 
        public Task<int> Insert(Produto p)
        {
            return _conn.InsertAsync(p);
        }


        /**
         * Método implementado com uso da estratégia de escrever o código SQL. Neste método podemos
         * ver a abstração que o SQLite faz, onde podemos digitar código SQL para manipulação do 
         * arquivo db3. O método também recebe uma model preenchida para atualizar no db3 e o retorno
         * em forma de Task é uma lista de todos os registros atualizados.
         */ 
        public Task<List<Produto>> Update(Produto p)
        {
            string sql = "UPDATE Produto SET Descricao=?, Quantidade=?, Preco=? WHERE id= ? ";
            return _conn.QueryAsync<Produto>(sql, p.Descricao, p.Quantidade, p.Preco, p.Id);
        }


        /**
         * Método que faz o retorno de todas as linhas contidas no arquivo db3 referentes 
         * a tabela Produto. Veja que o método executa a listagem de forma assíncrona.
         */ 
        public Task<List<Produto>> GetAll()
        {
            return _conn.Table<Produto>().ToListAsync();
        }


        /**
         * Método que remove um registro do arquivo db3 de forma assíncrona. Este método recebe
         * como parâmetro a Id do registro a ser removido. Observe o uso da LINQ no processo de
         * remoção. Para entender mais sobre LINQ veja essa aula: https://www.youtube.com/watch?v=m8rR1aLjrg0
         */
        public Task<int> Delete(int id)
        {
            return _conn.Table<Produto>().DeleteAsync(i => i.Id == id);
        }


        /**
         * Método para realizar uma busca na tabela com base uma string. O método recebe um
         * parâmetro do tipo string e por meio do SQL Like faz uma busca em um determinado campo
         * É retornada uma List de Produtos por meio de uma Task. A execução do SQL segue a mesma
         * linha utilizada no método update.
         * Para saber mais sobre SQL Like, veja: https://www.youtube.com/watch?v=t_aBO5PPpkQ&list=PLHVpcBDJr5dmEJ1dSRvi1N8I_ZFp-BdQO&index=9
         */
        public Task<List<Produto>> Search(string q)
        {
            string sql = "SELECT * FROM Produto WHERE Descricao LIKE '%" + q + "%' ";

            return _conn.QueryAsync<Produto>(sql);
        }
    }
}
