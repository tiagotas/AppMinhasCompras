using AppMinhasCompras.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AppMinhasCompras.View
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Listagem : ContentPage
    {
        /**
         * A ObservableCollection é uma classe que armazena um array de objetos do tipo de Produto.
         * Utilizamos essa classe quando estamos apresentando um array de objetos ao usuário. Diferencial
         * dessa classe é que toda vez que um item é add, removido ou modificado no array de objetos a interface
         * gráfica também é atualizada. Assim as modificações feitas no array sempre estão na vista do usuário.
         */
        ObservableCollection<Produto> lista_produtos = new ObservableCollection<Produto>();


        /**
         * 
         */ 
        public Listagem()
        {
            InitializeComponent();

            /**
             * Referenciando que a a fonte itens (a serem mostrados ao usuário) a ListView é a ObservableCollection 
             * definida acima. Fazendo essa definição no construtor estamos amarrando a fonte de dados da ListView assim
             * que ela é criada.
             */
            lst_produtos.ItemsSource = lista_produtos;
        }


        /**
         * Tratamento do evento de clique no ToolBarItem que fará a navegação da tela de listagem 
         * até a leta de cadastro de novo produto. A navegação está envolvida em um try catch
         * e se algum problema acontecer a mensação da exceção será mostrada ao usuário via DisplayAlert
         */ 
        private void ToolbarItem_Clicked_Novo(object sender, EventArgs e)
        {
            try
            {
                Navigation.PushAsync(new NovoProduto());

            } catch(Exception ex)
            {
                DisplayAlert("Ops", ex.Message, "OK");
            }            
        }


        /**
         * Método que faz a soma dos itens da ObservableCollection, isto é,
         * a soma do subtotal (preco x quantidade) de cada um dos itens do array de objetos
         */
        private void ToolbarItem_Clicked_Somar(object sender, EventArgs e)
        {
            /**
             * Uso da LINQ do C# para fazer a soma de cada um dos itens do array de objetos.
             */ 
            double soma = lista_produtos.Sum(i => i.Preco * i.Quantidade);

            string msg = "O total da compra é: " + soma;

            DisplayAlert("Ops", msg, "OK");
        }


        /**
         * Método executado quando a página é exibida ao usuário.
         */ 
        protected override void OnAppearing()
        {
            /**
             * Se a ObservableCollection estiver vazia é executado para obter todas as linhas do db3
             */
            if (lista_produtos.Count == 0)
            {
                /**
                 * Inicializando a Thread que irá buscar o array de objetos no arquivo db3
                 * via classe SQLiteDatabaseHelper encapsulada na propriedade Database da
                 * classe App.
                 */ 
                System.Threading.Tasks.Task.Run(async () =>
                {
                    /**
                     * Retornando o array de objetos vindos do db3, foi usada uma variável tem do tipo
                     * List para que abaixo no foreach possamos percorrer a lista temporária e add
                     * os itens à ObservableCollection
                     */
                    List<Produto> temp = await App.Database.GetAll();

                    foreach (Produto item in temp)
                    {
                        lista_produtos.Add(item);
                    }

                    /**
                     * Após carregar os registros para a ObservableCollection removemos o loading da tela.
                     */
                    ref_carregando.IsRefreshing = false;
                });
            }                       
        }


        /**
         * Trata o evento Clicked do MenuItem da ViewCell.ContextActions perguntando ao usuário
         * se ele realmente deseja remover aquele item do arquivo db3
         */
        private async void MenuItem_Clicked(object sender, EventArgs e)
        {
            /**
             * Reconhecendo qual foi a linha do ListView que disparou o evento de exclusão.
             */ 
            MenuItem disparador = (MenuItem)sender;


            /**
             * Obtendo qual foi o produto que estava anexado no BindingContext
             */ 
            Produto produto_selecionado = (Produto)disparador.BindingContext;

            /**
             * Perguntando ao usuário se ele realmente deseja remover. Note o await para aguardar
             * a resposta do usuário antes de prosseguir com o código.
             */ 
            bool confirmacao = await DisplayAlert("Tem Certeza?", "Remover Item?", "Sim", "Não");

            if(confirmacao)
            {
                /**
                 * Removendo o registro do db3 via método Delete da SQLiteDatabaseHelper
                 */ 
                await App.Database.Delete(produto_selecionado.Id);

                /**
                 * Removendo o item da ObservableCollection também, que é automaticamente
                 * removida da visão do usuário na ListView também.
                 */
                lista_produtos.Remove(produto_selecionado);
            }
        }


        /**
         * Trata o evento TextChanged da SearchBar recebendo os novos valores digitados
         */
        private void txt_busca_TextChanged(object sender, TextChangedEventArgs e)
        {
            /**
             * Obtendo o valor que foi digitado no Search
             */ 
            string buscou = e.NewTextValue;

            System.Threading.Tasks.Task.Run(async () =>
            {
                List<Produto> temp = await App.Database.Search(buscou);

                /**
                 * Limpando a ObservableCollection antes de add os itens vindos da busca.
                 */
                lista_produtos.Clear();

                foreach (Produto item in temp)
                {
                    lista_produtos.Add(item);
                }

                ref_carregando.IsRefreshing = false;
            });
        }


        /**
         * Trata o evento ItemSelected da ListView navegando para a página de detalhes.
         */
        private void lst_produtos_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            /**
             * Forma contraída de definir o BindingContext da página EditarProduto como sendo o
             * Produto que foi selecionado na ListView (item da ListView) e em seguida já
             * redicionando na navegação.
             */ 
            Navigation.PushAsync(new EditarProduto
            {
                BindingContext = (Produto)e.SelectedItem
            });
        }
    }
}