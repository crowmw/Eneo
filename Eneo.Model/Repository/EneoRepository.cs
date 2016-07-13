namespace Eneo.Model.Repository
{
    public abstract class EneoRepository
    {
        protected EneoContext _eneoCtx;

        public EneoRepository(EneoContext context)
        {
            _eneoCtx = context;
        }
    }
}