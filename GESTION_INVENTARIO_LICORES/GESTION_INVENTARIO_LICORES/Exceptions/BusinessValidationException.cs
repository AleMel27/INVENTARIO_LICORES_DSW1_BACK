namespace GESTION_INVENTARIO_LICORES.Exceptions
{
    public class BusinessValidationException : Exception
    {
        public BusinessValidationException(
            string message
        ) : base(message)
        {
        }
    }
}
