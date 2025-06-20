namespace Fact.Data.Entities
{
    public class Cliente
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;


        public string Telefono { get; set; } // Added missing property  
        public string Direccion { get; set; } // Added missing property 
    }

}
