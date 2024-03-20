using Azure.Data.Tables;
using Microsoft.AspNetCore.Mvc;
using WebApiAzureTable.Models;

namespace WebApiAzureTable.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ContatoController : ControllerBase
    {
        private readonly string _connectionString;
        private readonly string _tableName;

        public ContatoController(IConfiguration configuration) //responsavel por obter arquivos JSON
        {
            _connectionString = configuration.GetValue<string>("SAConnectionString");
            _tableName = configuration.GetValue<string>("AzureTableName");
        }

        private TableClient GetTableClient()
        {
            var serviceClient = new TableServiceClient(_connectionString);//inicia serviço table service
            var tableCliente = serviceClient.GetTableClient(_tableName);//pega tabela do table story

            tableCliente.CreateIfNotExists();//se a tabela não existir ela vai criar
            return tableCliente;// retorna a propria referencia da tabela 
        }

        //metodo para inserir
        [HttpPost]
        public ActionResult Criar(Contato contato)
        {
            var tableCliente = GetTableClient();//estamos chamando o metodo acima criado.Pegando a referencia tabela no Azure

            contato.RowKey = Guid.NewGuid().ToString(); // a chave da minha string, sera um identificador global unico
            contato.PartitionKey = contato.RowKey; // nem sempre por via de regra PartitionKey sera o igual o RowKey

            tableCliente.UpsertEntity(contato); // vai substituir tabela se existir, caso contrario vai criar. Não tem metodo de existir e de criar. Se existir vai atualizar, senão vai criar 
            return Ok(contato);//retorna o contato 

        }

        [HttpPut("{id}")]//recebe id
        public IActionResult Atualizar(string id, Contato contato)//recebe id e contato com parmetro
        {
            var tableClient = GetTableClient();
            var contatoTable = tableClient.GetEntity<Contato>(id, id).Value;//armazenar o registro do Azure table

            contatoTable.Nome = contato.Nome; //registro q esta na table -- o que esta indo na requisição
            contatoTable.Email = contato.Email; //fazendo assim atualização
            contatoTable.Telefone = contato.Telefone;

            tableClient.UpsertEntity(contatoTable); // atualizar caso exista e caso não cria um novo contato
            return Ok();

        }

        //metodo de Listar todos os contatos criados
        [HttpGet("Listar")]
        public IActionResult ObterTodos()
        {
            var tableClient = GetTableClient();
            var contatos = tableClient.Query<Contato>().ToList();
            return Ok(contatos);
        }

        //metodo obter por nome
        [HttpGet("ObterPorNmome/{nome}")]

        public IActionResult ObterPorNome(string nome) 
        {
            var tableClient = GetTableClient();
            var contatos = tableClient.Query<Contato>(x => x.Nome == nome).ToList();
            return Ok(contatos);
        }

        //deletar contato
        [HttpDelete("{id}")]
        public IActionResult Deletar(string id)
        {
            var tableClient = GetTableClient();
            tableClient.DeleteEntity(id,id);
            return NoContent();
        }
    }
}
