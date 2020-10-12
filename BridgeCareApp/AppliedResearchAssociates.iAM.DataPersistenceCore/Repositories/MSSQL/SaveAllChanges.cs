namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL
{
    public class SaveAllChanges : ISaveChanges
    {
        protected IAMContext context;

        public SaveAllChanges(IAMContext context)
        {
            this.context = context;
        }

        public void SaveChanges()
        {
            context.SaveChanges();
        }
    }
}
