using System;
using System.Collections.Generic;
using System.Text;
using AppliedResearchAssociates.iAM.DataPersistence.Models;

namespace AppliedResearchAssociates.iAM.DataPersistence.Repositories.Mongo
{
    class ScenarioRepository : GenericMongoRepository<Scenario>
    {
        public ScenarioRepository(MongoDriverContext context) : base(context)
        {

        }
    }

    public class MongoDriverContext
    {

    }
}
