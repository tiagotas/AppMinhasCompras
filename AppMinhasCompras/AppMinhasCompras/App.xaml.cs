
/**
 * Using dos recursos utilizados nesta classe.
 */

using System;
using Xamarin.Forms;

using AppMinhasCompras.Helper; // Classe que gerencia o acesso ao SQLite
using System.IO; // Recurso da .NET para abstrair o acesso ao armazenamento interno do dispositivo.


namespace AppMinhasCompras
{
    public partial class App : Application
    {
        /**
         * Campo estático que contém a instância da classe que abstrai os métodos de gerenciamento
         * do SQLite. Leia mais sobre a classe no arquivo SQLiteDatabaseHelper.cs
         */ 
        static SQLiteDatabaseHelper database;


        /**
         * Propriedade que define a forma de acesso a instância de SQLiteDatabaseHelper. A propriedade
         * é somente leitura, isto é, não é possível atribuir um valor a este campo. No comento que
         * o campo é chamado uma instância da classe SQLiteDatabaseHelper é criada (implementação get).
         */ 
        public static SQLiteDatabaseHelper Database
        {
            get
            {
                /**
                 * Se o campo database for nulo, significa que ainda não foi atribuída uma instância de
                 * SQLiteDatabaseHelper a ele, então uma nova instância será criada e esta mesma será usada
                 * em todo tempo de execução do arquivo.
                 */ 
                if (database == null)
                {
                    /**
                     * Para criar uma instância de SQLiteDatabaseHelper devemos o caminho do arquivo db3
                     * (arquivo que contém as definições "DDL" e os dados propriamente ditos) no SQLite).
                     * Devemos notar que essa abstração é necessária pois estamos em uma ferramenta multiplataforma
                     * e isso significa que há um caminho diferente no Windows, Android e iOS e com o uso das
                     * classes do System.IO podemos abstrair esse caminho.
                     */ 
                    string path = Path.Combine(
                        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                        "arquivo.db3"
                    );


                    /**
                     * Criando uma instância de SQLiteDatabaBaseHelper como caminho até o arquivo db3 mencionado acima.
                     */ 
                    database = new SQLiteDatabaseHelper(path);
                }

                return database;
            }
        }

        public App()
        {
            InitializeComponent();

            /**
             * Habilitando o recurso de navegação entre páginas e definindo a página
             * de listagem (dentro da pasta View) como a tela inicial do App.
             */ 
            MainPage = new NavigationPage(new View.Listagem());
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
