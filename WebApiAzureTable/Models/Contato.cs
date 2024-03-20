using Azure;
using Azure.Data.Tables;

namespace WebApiAzureTable.Models
{
    public class Contato : ITableEntity //precisamos implementar essa interface

    {

        //crie suas proprias propriedades

        public string Nome { get; set; }
        public string Telefone { get; set; }
        public string Email { get; set; }
    
    //apague e coloque {get;set;}
          public string PartitionKey { get; set; } //nome da partição
        public string RowKey  {get; set; } //identificador unico -chave da linha
         public DateTimeOffset? Timestamp  {get; set; }
        public ETag ETag  {get; set; }

     
    }
}
