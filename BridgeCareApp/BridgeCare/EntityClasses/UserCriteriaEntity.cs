﻿ using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
 using System.Data.Entity;
 using BridgeCare.Models;

namespace BridgeCare.EntityClasses
{
    [Table("USER_CRITERIA")]
    public class UserCriteriaEntity
    {
        [Key]
        public string USERNAME { get; set; }
        public string CRITERIA { get; set; }
        public bool HAS_ACCESS { get; set; }

        public UserCriteriaEntity() { }

        public UserCriteriaEntity (UserCriteriaModel userCriteriaModel)
        {
            USERNAME = userCriteriaModel.Username;
            CRITERIA = userCriteriaModel.Criteria;
            HAS_ACCESS = userCriteriaModel.HasAccess;
        }

        public static void DeleteEntry(UserCriteriaEntity entity, BridgeCareContext db)
        {
            db.Entry(entity).State = EntityState.Deleted;
        }
    }
}
