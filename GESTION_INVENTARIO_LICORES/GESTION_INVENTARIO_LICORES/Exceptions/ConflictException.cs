namespace GESTION_INVENTARIO_LICORES.Exceptions
{
    public class ConflictException : Exception
    {
        public ConflictException(
            string message
        ) : base(message)
        {
        }
    }
}
